using ProjectDependenciesGuard.Models;
using ProjectDependenciesGuard.QuikGraphUtils;
using ProjectDependenciesGuard.QuikGraphUtils.Models;
using QuikGraph;
using QuikGraph.Algorithms.Search;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectDependenciesGuard
{
    /// <summary>
    /// A set of conditions that can be applied to a graph of code sets.
    /// </summary>
    public sealed class Condition
    {
        private readonly BidirectionalGraph<CodeSetDefinition, Edge<CodeSetDefinition>> _graph;
        private readonly IEnumerable<CodeSetDefinition> _verticesToTest;

        internal Condition(BidirectionalGraph<CodeSetDefinition, Edge<CodeSetDefinition>> graph, IEnumerable<CodeSetDefinition> verticesToTest)
        {
            _graph = graph;
            _verticesToTest = verticesToTest;
        }

        /// <summary>
        /// Checks if filtered code sets have any duplicate transitive dependencies.
        /// </summary>
        /// <param name="allowed">List of allowed duplicate transient dependencies to ignore.</param>
        /// <returns>Condition result.</returns>
        public ConditionResult NotHaveDuplicateTransitiveDependencies(List<AllowedTransitiveDependency> allowed = null)
        {
            allowed ??= new List<AllowedTransitiveDependency>();

            var searchResults = new List<DuplicateTransitiveDependenciesSearchResult>();

            var candidateVertices = _graph.Vertices.Where(q => q.CodeSetType == CodeSetType.Project && _graph.OutEdges(q).Count() > 1 && _verticesToTest.Contains(q)).ToList();
            foreach (var projectVertice in candidateVertices)
            {
                var projectDirectDependenciesVertices = _graph.OutEdges(projectVertice).Select(q => q.Target).ToList();

                var edgesToDuplicateTransitiveDependencies = new List<Edge<CodeSetDefinition>>();

                foreach (var projectDirectDependencyVertice in projectDirectDependenciesVertices)
                {
                    var dfs = new DepthFirstSearchAlgorithm<CodeSetDefinition, Edge<CodeSetDefinition>>(_graph);
                    dfs.SetRootVertex(projectDirectDependencyVertice);
                    dfs.ProcessAllComponents = false;

                    var recorder = new EdgesToVerticesDetector(projectDirectDependenciesVertices);
                    dfs.TreeEdge += recorder.RecordTreeEdge;
                    dfs.Compute();
                    dfs.TreeEdge -= recorder.RecordTreeEdge;

                    edgesToDuplicateTransitiveDependencies.AddRange(recorder.FoundEdges);
                }

                var iterationResult = edgesToDuplicateTransitiveDependencies
                    .Where(edge => !allowed.Any(ruleException => ruleException.IsDependencyExceptional(projectVertice.Name, edge.Target.Name)))
                    .Select(q => new DuplicateTransitiveDependenciesSearchResult(projectVertice, q.Target, q.Source));

                searchResults.AddRange(iterationResult);
            }

            if (searchResults.Count == 0)
            {
                return new ConditionResult(TestResult.Success());
            }
            else
            {
                var testResult = TestResult.Failure(searchResults.Select(q => new FailingCodeSet(q.ProjectWithDuplicateDependencies.CodeSetType,
                    q.ProjectWithDuplicateDependencies.Name,
                    $"{q.ProjectWithDuplicateDependencies.CodeSetType} {q.ProjectWithDuplicateDependencies.Name} is referencing {q.DuplicateDependence.CodeSetType} {q.DuplicateDependence.Name} that is already referenced by its other reference: {q.ProjectAlreadyProvidingDependence.CodeSetType} {q.ProjectAlreadyProvidingDependence.Name}")).ToList());
                return new ConditionResult(testResult);
            }
        }

        /// <summary>
        /// Checks if filtered code sets only have direct dependencies for which given predicate function returns true.
        /// </summary>
        /// <param name="predicate">Predicate function taking code set under test and one of its dependencies as arguments and returning a bool value.</param>
        /// <returns>Condition result.</returns>
        public ConditionResult OnlyDependOnSetsMatchingPredicate(Func<CodeSetDefinition, CodeSetDefinition, bool> predicate)
        {
            var failures = new List<FailingCodeSet>();

            foreach (var testedCodeSet in _verticesToTest)
            {
                var dependencies = _graph.OutEdges(testedCodeSet).Select(q => q.Target).ToList();
                foreach (var dependency in dependencies)
                {
                    if (!predicate(testedCodeSet, dependency))
                    {
                        var failure = new FailingCodeSet(testedCodeSet.CodeSetType, testedCodeSet.Name,
                            $"{testedCodeSet.CodeSetType} {testedCodeSet.Name} has forbidden dependency on {dependency.CodeSetType} {dependency.Name}");
                        failures.Add(failure);
                    }
                }
            }

            if (failures.Count == 0)
            {
                return new ConditionResult(TestResult.Success());
            }
            else
            {
                return new ConditionResult(TestResult.Failure(failures));
            }
        }

        /// <summary>
        /// Checks if filtered code sets have a direct or indirect dependency on a code set specified by name and type.
        /// </summary>
        /// <param name="name">Code set name.</param>
        /// <param name="type">Code set type.</param>
        /// <returns>Condition result.</returns>
        public ConditionResult DependOn(string name, CodeSetType type)
        {
            var dependent = VerticesDependentOn(name, type);

            if (dependent.Count() == _verticesToTest.Count())
            {
                return new ConditionResult(TestResult.Success());
            }
            else
            {
                var failures = _verticesToTest.Except(dependent).Select(q => new FailingCodeSet(q.CodeSetType, q.Name,
                    $"{q.CodeSetType} {q.Name} has no required dependency on {type} {name}")).ToList();
                return new ConditionResult(TestResult.Failure(failures));
            }
        }

        /// <summary>
        /// Checks if filtered code sets have no direct or indirect dependency on a code set specified by name and type.
        /// </summary>
        /// <param name="name">Code set name.</param>
        /// <param name="type">Code set type.</param>
        /// <returns>Condition result.</returns>
        public ConditionResult NotDependOn(string name, CodeSetType type)
        {
            var dependent = VerticesDependentOn(name, type);

            if (dependent.Any())
            {
                return new ConditionResult(TestResult.Success());
            }
            else
            {
                var failures = dependent.Select(q => new FailingCodeSet(q.CodeSetType, q.Name,
                    $"{q.CodeSetType} {q.Name} has forbidden dependency on {type} {name}")).ToList();
                return new ConditionResult(TestResult.Failure(failures));
            }
        }

        private IEnumerable<CodeSetDefinition> VerticesDependentOn(string name, CodeSetType type)
        {
            var dependent = new List<CodeSetDefinition>();

            foreach (var testedCodeSet in _verticesToTest)
            {
                var dfs = new DepthFirstSearchAlgorithm<CodeSetDefinition, Edge<CodeSetDefinition>>(_graph);
                dfs.SetRootVertex(testedCodeSet);
                dfs.ProcessAllComponents = false;

                var recorder = new EdgesToTargetDetector(name, type);
                dfs.TreeEdge += recorder.RecordTreeEdge;
                dfs.Compute();
                dfs.TreeEdge -= recorder.RecordTreeEdge;

                if (recorder.FoundEdges.Any())
                    dependent.Add(testedCodeSet);
            }

            return dependent;
        }
    }
}
