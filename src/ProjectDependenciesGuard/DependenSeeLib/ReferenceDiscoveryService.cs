using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace DependenSee
{
    internal class ReferenceDiscoveryService
    {
        public string SourceFolder { get; set; }
        public bool FollowReparsePoints { get; set; }
        public bool ThrowOnSkipReparsePoints { get; set; }
        /// <summary>
        /// Comma Separated list of folders (either absolute paths or relative to SourceFolder) to skip during scan, even if there are references to them from your projects. Wildcards not allowed.
        /// </summary>
        public string ExcludeFolders { get; set; }

        public DiscoveryResult Discover()
        {
            var result = new DiscoveryResult
            {
                Packages = new List<Package>(),
                Projects = new List<Project>(),
                References = new List<Reference>()
            };

            Discover(SourceFolder, result);
            return result;
        }

        private static string[] ParseStringToLowercaseStringArray(string list) =>
            string.IsNullOrWhiteSpace(list)
            ? Array.Empty<string>()
            : list.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim().ToLower())
                .ToArray();

        private void Discover(string folder, DiscoveryResult result)
        {
            var info = new DirectoryInfo(folder);

            var excludedByRule = GetFolderExclusionFor(info.FullName);
            if (excludedByRule != null)
            {
                return;
            }

            if (!info.Exists)
            {
                throw new FileNotFoundException($"Missing folder {folder}");
            }
            if (info.Attributes.HasFlag(FileAttributes.ReparsePoint) && !FollowReparsePoints)
            {
                if (ThrowOnSkipReparsePoints)
                    throw new ArgumentException("Reparse point found and FollowReparsePoints");
                else
                    return;
            }

            var projectFiles = Directory.EnumerateFiles(folder, "*.csproj")
                .Concat(Directory.EnumerateFiles(folder, "*.vbproj"));
            foreach (var file in projectFiles)
            {
                var id = file.Replace(SourceFolder, "");
                var name = Path.GetFileNameWithoutExtension(file);

                // add this project.
                if (!result.Projects.Any(p => p.Id == id))
                    result.Projects.Add(new Project
                    {
                        Id = id,
                        Name = name
                    });

                var (projects, packages) = DiscoverFileReferences(file);

                foreach (var project in projects)
                {
                    if (!result.Projects.Any(p => p.Id == project.Id)) result.Projects.Add(project);

                    result.References.Add(new Reference
                    {
                        From = id,
                        To = project.Id
                    });
                }

                foreach (var package in packages)
                {
                    if (!result.Packages.Any(p => p.Id == package.Id)) result.Packages.Add(package);

                    result.References.Add(new Reference
                    {
                        From = id,
                        To = package.Id
                    });
                }
            }
            var directories = Directory.EnumerateDirectories(folder);
            foreach (var directory in directories)
            {
                Discover(directory, result);
            }
        }

        private (List<Project> projects, List<Package> packages) DiscoverFileReferences(string path)
        {
            var xml = new XmlDocument();
            xml.Load(path);
            var basePath = new FileInfo(path).Directory.FullName;

            var projects = DiscoverProjectReferences(xml, basePath);
            var packages = DiscoverPackageReferences(xml);

            return (projects, packages);
        }

        private List<Package> DiscoverPackageReferences(XmlDocument xml)
        {
            // PackageReference = Nuget package
            // Reference = COM/DLL reference. These can have a child <HintPath>relative path to dll</HintPath>'
            // Reference also present for .NET Framework projects when they reference BCL assemblies, but these
            // do not include a HintPath
            var packageReferenceNodes = xml.SelectNodes("//*[local-name() = 'PackageReference' or local-name() = 'Reference']");
            var packages = new List<Package>();
            foreach (XmlNode node in packageReferenceNodes)
            {
                var packageName = node.Attributes["Include"]?.Value
                               ?? node.Attributes["Update"].Value;

                packages.Add(new Package
                {
                    Id = packageName,
                    Name = packageName
                });
            }
            return packages;
        }

        private string GetFolderExclusionFor(string fullFolderPath)
        {
            if (string.IsNullOrWhiteSpace(ExcludeFolders)) return null;

            var allRules = ExcludeFolders
                .Split(',')
                .Select(r => Path.IsPathRooted(r) ? r : Path.GetFullPath(r, SourceFolder))
                .Select(r => r.ToLower().Trim())
                .ToList();

            fullFolderPath = fullFolderPath.ToLower();
            return allRules.FirstOrDefault(r => fullFolderPath.StartsWith(r));
        }

        private List<Project> DiscoverProjectReferences(XmlDocument xml, string basePath)
        {
            var projectReferenceNodes = xml.SelectNodes("//*[local-name() = 'ProjectReference']");
            var projects = new List<Project>();

            foreach (XmlNode node in projectReferenceNodes)
            {
                var referencePath = node.Attributes["Include"].Value;
                var fullPath = Path.GetFullPath(referencePath, basePath);

                string filename = Path.GetFileNameWithoutExtension(fullPath);

                projects.Add(new Project
                {
                    Id = fullPath.Replace(SourceFolder, ""),
                    Name = filename
                });
            }
            return projects;
        }
    }
}
