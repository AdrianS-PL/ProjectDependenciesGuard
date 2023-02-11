using ProjectDependenciesGuard.Models;
using QuikGraph;
using System.Collections.Generic;
using System.Linq;

namespace ProjectDependenciesGuard.QuikGraphUtils
{
    internal class EdgesToVerticesDetector
    {
        private readonly List<Edge<CodeSetDefinition>> _foundEdges = new List<Edge<CodeSetDefinition>>();
        private readonly List<CodeSetDefinition> _targetVertices;

        public IEnumerable<Edge<CodeSetDefinition>> FoundEdges => _foundEdges;


        public EdgesToVerticesDetector(IEnumerable<CodeSetDefinition> targetVertices)
        {
            _targetVertices = targetVertices.ToList();
        }

        public void RecordTreeEdge(Edge<CodeSetDefinition> edge)
        {
            if (_targetVertices.Contains(edge.Target))
                _foundEdges.Add(edge);
        }
    }
}
