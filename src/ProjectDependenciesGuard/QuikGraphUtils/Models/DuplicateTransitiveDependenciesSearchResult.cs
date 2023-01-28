using ProjectDependenciesGuard.Models;

namespace ProjectDependenciesGuard.QuikGraphUtils.Models
{
    internal struct DuplicateTransitiveDependenciesSearchResult
    {
        internal CodeSetDefinition ProjectWithDuplicateDependencies { get; private set; }
        internal CodeSetDefinition DuplicateDependence { get; private set; }
        internal CodeSetDefinition ProjectAlreadyProvidingDependence { get; private set; }

        internal DuplicateTransitiveDependenciesSearchResult(CodeSetDefinition projectWithDuplicateDependencies,
        CodeSetDefinition duplicateDependence,
        CodeSetDefinition projectAlreadyProvidingDependence)
        {
            ProjectWithDuplicateDependencies = projectWithDuplicateDependencies;
            DuplicateDependence = duplicateDependence;
            ProjectAlreadyProvidingDependence = projectAlreadyProvidingDependence;
        }
    }
}
