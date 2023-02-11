using DependenSee;
using ProjectDependenciesGuard.Models;
using QuikGraph;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProjectDependenciesGuard
{
    /// <summary>
    /// Creates a graph of code sets that can have predicates and conditions applied to it.
    /// </summary>
    public static class DependencyGuard
    {
        /// <summary>
        /// Creates a graph of code sets that can have predicates and conditions applied to it.
        /// </summary>
        /// <param name="pathToRootDir">Path to root directory of all csproj files.</param>
        /// <param name="existenceCheckFileName">If defined, method will check if this file exists in directory from <paramref name="pathToRootDir"/>.</param>
        /// <returns>A graph of code sets that can have predicates and conditions applied to it.</returns>
        /// <exception cref="ArgumentException">Thrown when file with name <paramref name="existenceCheckFileName"/> does not exist in directory <paramref name="pathToRootDir"/>.</exception>
        public static CodeSets ProjectsAndPackagesInPath(string pathToRootDir, string existenceCheckFileName = "")
        {
            if (!Path.IsPathRooted(pathToRootDir))
            {
                pathToRootDir = Path.GetFullPath(Directory.GetCurrentDirectory() + pathToRootDir);
            }
            
            if (!string.IsNullOrWhiteSpace(existenceCheckFileName) && !new DirectoryInfo(pathToRootDir).EnumerateFiles().Any(q => q.Name == existenceCheckFileName))
            {
                throw new ArgumentException($"File {existenceCheckFileName} does not exist in directory {pathToRootDir}");
            }


            var dependenseeService = new ReferenceDiscoveryService()
            {
                SourceFolder = pathToRootDir
            };

            var dependenseeResult = dependenseeService.Discover();

            var graph = PrepareGraph(dependenseeResult);

            return new CodeSets(graph);
        }

        private static BidirectionalGraph<CodeSetDefinition, Edge<CodeSetDefinition>> PrepareGraph(DiscoveryResult dependenseeResult)
        {
            var verticesList = new List<CodeSetDefinition>();
            verticesList.AddRange(dependenseeResult.Projects.Select(q => new CodeSetDefinition(q.Id, CodeSetType.Project, q.Name)));
            verticesList.AddRange(dependenseeResult.Packages.Select(q => new CodeSetDefinition(q.Id, CodeSetType.Package, q.Name)));

            var edgesList = new List<Edge<CodeSetDefinition>>();
            edgesList.AddRange(dependenseeResult.References.Select(q => new Edge<CodeSetDefinition>(
                verticesList.Single(r => r.Id == q.From), verticesList.Single(r => r.Id == q.To))));

            var graph = new BidirectionalGraph<CodeSetDefinition, Edge<CodeSetDefinition>>(false, verticesList.Count, edgesList.Count);

            graph.AddVertexRange(verticesList);
            graph.AddEdgeRange(edgesList);

            return graph;
        }
    }
}
