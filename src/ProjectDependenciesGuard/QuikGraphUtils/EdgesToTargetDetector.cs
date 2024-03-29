﻿using ProjectDependenciesGuard.Models;
using QuikGraph;
using System.Collections.Generic;
using System.Linq;

namespace ProjectDependenciesGuard.QuikGraphUtils
{
    internal class EdgesToTargetDetector
    {
        private readonly string _targetName;
        private readonly CodeSetType _targetType;

        private readonly List<Edge<CodeSetDefinition>> _foundEdges = new List<Edge<CodeSetDefinition>>();
        public IEnumerable<Edge<CodeSetDefinition>> FoundEdges => _foundEdges;

        public EdgesToTargetDetector(string targetName, CodeSetType targetType)
        {
            _targetName = targetName;
            _targetType = targetType;
        }

        public void RecordTreeEdge(Edge<CodeSetDefinition> edge)
        {
            if (edge.Target.Name == _targetName && edge.Target.CodeSetType == _targetType)
                _foundEdges.Add(edge);
        }
    }
}
