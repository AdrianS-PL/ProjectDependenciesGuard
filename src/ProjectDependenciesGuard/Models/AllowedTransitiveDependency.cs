namespace ProjectDependenciesGuard.Models
{
    /// <summary>
    /// Defines an allowed transitive dependency.
    /// </summary>
    public class AllowedTransitiveDependency
    {
        private readonly string _allowedSourceName;
        private readonly string _allowedTargetName;

        private AllowedTransitiveDependency(string allowedSourceName, string allowedTargetName)
        {
            _allowedSourceName = allowedSourceName;
            _allowedTargetName = allowedTargetName;
        }

        /// <summary>
        /// Gets a value indicating whether the dependency from source code set to target code set is accepted.
        /// </summary>
        /// <param name="sourceName"></param>
        /// <param name="targetName"></param>
        /// <returns>true if dependency is allowed; otherwise, false.</returns>
        public bool IsDependencyExceptional(string sourceName, string targetName)
        {
            return (_allowedSourceName == null || _allowedSourceName == sourceName) &&
                (_allowedTargetName == null || _allowedTargetName == targetName);
        }

        /// <summary>
        /// Create a new instance defining acceptance of any transitive dependency from source code set.
        /// </summary>
        /// <param name="allowedSourceName">Name of source code set.</param>
        /// <returns>New instance.</returns>
        public static AllowedTransitiveDependency AllowAnyForSource(string allowedSourceName)
        {
            return new AllowedTransitiveDependency(allowedSourceName, null);
        }

        /// <summary>
        /// Create a new instance defining acceptance of any transitive dependency to target code set.
        /// </summary>
        /// <param name="allowedTargetName">Name of target code set.</param>
        /// <returns>New instance.</returns>
        public static AllowedTransitiveDependency AllowAnyForTarget(string allowedTargetName)
        {
            return new AllowedTransitiveDependency(null, allowedTargetName);
        }

        /// <summary>
        /// Create a new instance defining acceptance of specific transitive dependency from source code set to target code set.
        /// </summary>
        /// <param name="allowedSourceName">Name of source code set.</param>
        /// <param name="allowedTargetName">Name of target code set.</param>
        /// <returns>New instance.</returns>
        public static AllowedTransitiveDependency AllowForSourceAndTarget(string allowedSourceName, string allowedTargetName)
        {
            return new AllowedTransitiveDependency(allowedSourceName, allowedTargetName);
        }
    }
}
