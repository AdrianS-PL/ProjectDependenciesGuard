using ProjectDependenciesGuard.Models;

namespace ProjectDependenciesGuard
{
    /// <summary>
    /// Predicate and condition combination that can have executors (i.e. GetResult()) applied to them.
    /// </summary>
    public sealed class ConditionResult
    {
        private readonly TestResult _testResult;

        internal ConditionResult(TestResult testResult)
        {
            _testResult = testResult;
        }

        /// <summary>
        /// Returns an indication of whether the graph satisfies the conditions. Predicates are applied to filter vertices.
        /// </summary>
        /// <returns>An indication of whether the conditions are true, along with a list of code sets failing the check if they are not.</returns>
        public TestResult GetResult()
        {
            return _testResult;
        }
    }
}
