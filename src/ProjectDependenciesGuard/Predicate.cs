using ProjectDependenciesGuard.Models;
using QuikGraph;
using System;
using System.Linq;

namespace ProjectDependenciesGuard
{
    /// <summary>
    /// A set of predicates that can be applied to vertices of a graph of code sets.
    /// </summary>
    public sealed class Predicate
    {
        private readonly BidirectionalGraph<CodeSetDefinition, Edge<CodeSetDefinition>> _graph;

        internal Predicate(BidirectionalGraph<CodeSetDefinition, Edge<CodeSetDefinition>> graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Selects graph vertices for which given predicate function returns true.
        /// </summary>
        /// <param name="predicate">Predicate function.</param>
        /// <returns>Predicate result.</returns>
        public PredicateResult MatchPredicate(Func<CodeSetDefinition, bool> predicate)
        {
            return new PredicateResult(_graph, _graph.Vertices.Where(predicate));
        }
    }
}
