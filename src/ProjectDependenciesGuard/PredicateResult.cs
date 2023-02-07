using ProjectDependenciesGuard.Models;
using QuikGraph;
using System.Collections.Generic;

namespace ProjectDependenciesGuard
{
    /// <summary>
    /// A graph of code sets and a list of its vertices that match specified predicate. They can have conditions applied to them.
    /// </summary>
    public sealed class PredicateResult
    {
        private readonly BidirectionalGraph<CodeSetDefinition, Edge<CodeSetDefinition>> _graph;
        private readonly IEnumerable<CodeSetDefinition> _verticesToTest;

        internal PredicateResult(BidirectionalGraph<CodeSetDefinition, Edge<CodeSetDefinition>> graph, IEnumerable<CodeSetDefinition> verticesToTest)
        {
            _graph = graph;
            _verticesToTest = verticesToTest;
        }

        /// <summary>
        /// Applies a set of conditions to the graph of code sets.
        /// </summary>
        /// <returns></returns>
        public Condition Should()
        {
            return new Condition(_graph, _verticesToTest);
        }

        /// <summary>
        /// Gets all vertices of code sets graph that match specified predicate.
        /// </summary>
        /// <returns>All vertices of code sets graph that match specified predicate.</returns>
        public IEnumerable<CodeSetDefinition> GetCodeSetDefinitions()
        {
            return _verticesToTest;
        }
    }
}