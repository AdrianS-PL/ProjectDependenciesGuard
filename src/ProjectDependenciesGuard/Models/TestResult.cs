using System.Collections.Generic;
using System.Linq;

namespace ProjectDependenciesGuard.Models
{
    /// <summary>
    /// Defines a result from a test.
    /// </summary>
    public sealed class TestResult
    {
        private IReadOnlyList<FailingCodeSet> _failingCodeSets;

        private TestResult()
        {
        }

        /// <summary>
        /// Gets a flag indicating the success or failure of the test.
        /// </summary>
        public bool IsSuccessful { get; private set; }

        /// <summary>
        /// The list of test failure descriptions.
        /// </summary>
        public IReadOnlyList<FailingCodeSet> FailingCodeSets
        {
            get
            {
                return _failingCodeSets ?? new List<FailingCodeSet>();
            }
        }

        internal static TestResult Success()
        {
            return new TestResult
            {
                IsSuccessful = true
            };
        }

        internal static TestResult Failure(IReadOnlyList<FailingCodeSet> failingCodeSets)
        {
            return new TestResult
            {
                IsSuccessful = false,
                _failingCodeSets = failingCodeSets
            };
        }
    }
}
