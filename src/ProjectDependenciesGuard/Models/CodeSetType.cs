namespace ProjectDependenciesGuard.Models
{
    /// <summary>
    /// Defines code set type.
    /// </summary>
    public enum CodeSetType
    {
        /// <summary>
        /// Code set is a csproj project.
        /// </summary>
        Project,
        /// <summary>
        /// Code set is a NuGet package.
        /// </summary>
        Package
    }
}
