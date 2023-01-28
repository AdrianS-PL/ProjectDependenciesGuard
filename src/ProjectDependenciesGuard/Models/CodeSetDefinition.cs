namespace ProjectDependenciesGuard.Models
{
    /// <summary>
    /// Defines a code set.
    /// </summary>
    public struct CodeSetDefinition
    {
        /// <summary>
        /// Code set type.
        /// </summary>
        public CodeSetType CodeSetType { get; private set; }
        /// <summary>
        /// Code set name.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Id created by Dependensee.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="id">Id created by Dependensee.</param>
        /// <param name="codeSetType">Code set type.</param>
        /// <param name="name">Code set name.</param>
        public CodeSetDefinition(string id, CodeSetType codeSetType, string name)
        {
            Id = id;
            CodeSetType = codeSetType;
            Name = name;
        }
    }
}
