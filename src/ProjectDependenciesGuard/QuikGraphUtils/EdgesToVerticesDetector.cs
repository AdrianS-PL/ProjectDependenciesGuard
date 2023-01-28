using ProjectDependenciesGuard.Models;
using QuikGraph;
using System.Collections.Generic;
using System.Linq;

namespace ProjectDependenciesGuard.QuikGraphUtils
{
    internal class EdgesToVerticesDetector
    {
        private List<Edge<CodeSetDefinition>> _foundEdges = new List<Edge<CodeSetDefinition>>();
        private List<CodeSetDefinition> _targetVertices;

        public List<Edge<CodeSetDefinition>> FoundEdges => _foundEdges.ToList();


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
