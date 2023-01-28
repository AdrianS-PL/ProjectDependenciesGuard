namespace ProjectDependenciesGuard.Models
{
    /// <summary>
    /// Describes code set test failure.
    /// </summary>
    public struct FailingCodeSet
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
        /// Reason why code set failed test.
        /// </summary>
        public string Reason { get; private set; }

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="codeSetType">Code set type.</param>
        /// <param name="name">Code set name.</param>
        /// <param name="reason">Reason why code set failed test.</param>
        public FailingCodeSet(CodeSetType codeSetType, string name, string reason)
        {
            CodeSetType = codeSetType;
            Name = name;
            Reason = reason;
        }
    }
}
