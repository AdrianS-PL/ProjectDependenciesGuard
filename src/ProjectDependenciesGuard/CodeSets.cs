using ProjectDependenciesGuard.Models;
using QuikGraph;
using System.Collections.Generic;

namespace ProjectDependenciesGuard
{
    /// <summary>
    /// A graph of code sets that can have predicates and conditions applied to it.
    /// </summary>
    public sealed class CodeSets
    {
        private readonly BidirectionalGraph<CodeSetDefinition, Edge<CodeSetDefinition>> _graph;


        internal CodeSets(BidirectionalGraph<CodeSetDefinition, Edge<CodeSetDefinition>> graph)
        {
            _graph = graph;
        }

        /// <summary>
        /// Gets a cloned QuikGraph graph of code sets.
        /// </summary>
        /// <returns>Cloned QuikGraph graph of code sets.</returns>
        public BidirectionalGraph<CodeSetDefinition, Edge<CodeSetDefinition>> GetQuikGraph()
        {
            return _graph.Clone();
        }

        /// <summary>
        /// Gets all vertices of code sets graph.
        /// </summary>
        /// <returns>All vertices of code sets graph.</returns>
        public IEnumerable<CodeSetDefinition> GetCodeSetDefinitions()
        {
            return _graph.Vertices;
        }

        /// <summary>
        /// Allows graph vertices to be applied to a filter.
        /// </summary>
        /// <returns>A list of graph vertices (code sets) onto which you can apply a filter.</returns>
        public Predicate That()
        {
            return new Predicate(_graph);
        }

        /// <summary>
        /// Applies a set of conditions to the graph of code sets.
        /// </summary>
        /// <returns></returns>
        public Condition Should()
        {
            return new Condition(_graph, _graph.Vertices);
        }
    }
}
