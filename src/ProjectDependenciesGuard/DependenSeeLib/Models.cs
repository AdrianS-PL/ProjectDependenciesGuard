using System.Collections.Generic;

namespace DependenSee
{
    internal class DiscoveryResult
    {
        public List<Project> Projects { get; set; }
        public List<Package> Packages { get; set; }
        public List<Reference> References { get; set; }
    }

    internal class Project
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
    internal class Package
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    internal class Reference
    {
        public string From { get; set; }
        public string To { get; set; }
    }
}
