using ProjectDependenciesGuard.Models;
using QuikGraph;

namespace ProjectDependenciesGuard.Tests.Unit;

public class ConditionTests
{
    [Fact]
    public void NotHaveDuplicateTransitiveDependencies_Should_DetectTransitiveDependencies()
    {
        var vertices = new List<CodeSetDefinition>
        {
            new CodeSetDefinition("ProjectA", CodeSetType.Project, "ProjectA"),
            new CodeSetDefinition("ProjectB", CodeSetType.Project, "ProjectB"),
            new CodeSetDefinition("ProjectC", CodeSetType.Project, "ProjectC"),
            new CodeSetDefinition("PackageA", CodeSetType.Package, "PackageA"),
        };

        var edges = new List<Edge<CodeSetDefinition>>
        {
            new Edge<CodeSetDefinition>(vertices[0], vertices[1]),
            new Edge<CodeSetDefinition>(vertices[1], vertices[2]),
            new Edge<CodeSetDefinition>(vertices[0], vertices[2]),
            new Edge<CodeSetDefinition>(vertices[0], vertices[3]),
            new Edge<CodeSetDefinition>(vertices[1], vertices[3]),
        };

        var graph = new CodeSetsQuikGraph(false, vertices.Count, edges.Count);

        graph.AddVertexRange(vertices);
        graph.AddEdgeRange(edges);

        var condition = new Condition(graph, graph.Vertices);
        var result = condition.NotHaveDuplicateTransitiveDependencies().GetResult();

        Assert.False(result.IsSuccessful);
        Assert.Equal(2, result.FailingCodeSets.Count);
        Assert.Equal(2, result.FailingCodeSets.Select(q => q.Name).Count(q => q == vertices[0].Name));
    }

    [Fact]
    public void NotHaveDuplicateTransitiveDependencies_Should_ReturnSuccess_WhenNoTransitiveDependenciesDetected()
    {
        var vertices = new List<CodeSetDefinition>
        {
            new CodeSetDefinition("ProjectA", CodeSetType.Project, "ProjectA"),
            new CodeSetDefinition("ProjectB", CodeSetType.Project, "ProjectB"),
            new CodeSetDefinition("ProjectC", CodeSetType.Project, "ProjectC"),
            new CodeSetDefinition("PackageA", CodeSetType.Package, "PackageA"),
        };

        var edges = new List<Edge<CodeSetDefinition>>
        {
            new Edge<CodeSetDefinition>(vertices[0], vertices[1]),
            new Edge<CodeSetDefinition>(vertices[1], vertices[2]),
            new Edge<CodeSetDefinition>(vertices[1], vertices[3]),
        };

        var graph = new CodeSetsQuikGraph(false, vertices.Count, edges.Count);

        graph.AddVertexRange(vertices);
        graph.AddEdgeRange(edges);

        var condition = new Condition(graph, graph.Vertices);
        var result = condition.NotHaveDuplicateTransitiveDependencies().GetResult();

        Assert.True(result.IsSuccessful);
        Assert.Empty(result.FailingCodeSets);
    }

    [Fact]
    public void NotHaveDuplicateTransitiveDependencies_Should_AllowExceptions()
    {
        var vertices = new List<CodeSetDefinition>
        {
            new CodeSetDefinition("ProjectA", CodeSetType.Project, "ProjectA"),
            new CodeSetDefinition("ProjectB", CodeSetType.Project, "ProjectB"),
            new CodeSetDefinition("ProjectC", CodeSetType.Project, "ProjectC"),
            new CodeSetDefinition("PackageA", CodeSetType.Package, "PackageA"),
        };

        var edges = new List<Edge<CodeSetDefinition>>
        {
            new Edge<CodeSetDefinition>(vertices[0], vertices[1]),
            new Edge<CodeSetDefinition>(vertices[1], vertices[2]),
            new Edge<CodeSetDefinition>(vertices[0], vertices[2]),
            new Edge<CodeSetDefinition>(vertices[0], vertices[3]),
            new Edge<CodeSetDefinition>(vertices[1], vertices[3]),
        };

        var exceptions = new List<AllowedTransitiveDependency>
        {
            AllowedTransitiveDependency.AllowAnyForTarget(vertices[3].Name),
            AllowedTransitiveDependency.AllowForSourceAndTarget(vertices[0].Name, vertices[2].Name)
        };

        var graph = new CodeSetsQuikGraph(false, vertices.Count, edges.Count);
        graph.AddVertexRange(vertices);
        graph.AddEdgeRange(edges);

        var condition = new Condition(graph, graph.Vertices);
        var result = condition.NotHaveDuplicateTransitiveDependencies(exceptions).GetResult();

        Assert.True(result.IsSuccessful);
        Assert.Empty(result.FailingCodeSets);
    }

    [Fact]
    public void DependOn_Should_Succeed_When_AllTestedCodeSetsHaveRequiredDependency()
    {
        var vertices = new List<CodeSetDefinition>
        {
            new CodeSetDefinition("ProjectA", CodeSetType.Project, "ProjectA"),
            new CodeSetDefinition("ProjectB", CodeSetType.Project, "ProjectB"),
            new CodeSetDefinition("ProjectC", CodeSetType.Project, "ProjectC")
        };

        var edges = new List<Edge<CodeSetDefinition>>
        {
            new Edge<CodeSetDefinition>(vertices[0], vertices[1]),
            new Edge<CodeSetDefinition>(vertices[1], vertices[2]),
        };

        var graph = new CodeSetsQuikGraph(false, vertices.Count, edges.Count);
        graph.AddVertexRange(vertices);
        graph.AddEdgeRange(edges);

        var condition = new Condition(graph, graph.Vertices.Except(new[] { vertices[2] }));
        var result = condition.DependOn(vertices[2].Name, vertices[2].CodeSetType).GetResult();

        Assert.True(result.IsSuccessful);
        Assert.Empty(result.FailingCodeSets);
    }

    [Fact]
    public void DependOn_Should_Fail_When_NotAllTestedCodeSetsHaveRequiredDependency()
    {
        var vertices = new List<CodeSetDefinition>
        {
            new CodeSetDefinition("ProjectA", CodeSetType.Project, "ProjectA"),
            new CodeSetDefinition("ProjectB", CodeSetType.Project, "ProjectB"),
            new CodeSetDefinition("ProjectC", CodeSetType.Project, "ProjectC"),
            new CodeSetDefinition("ProjectD", CodeSetType.Project, "ProjectD")
        };

        var edges = new List<Edge<CodeSetDefinition>>
        {
            new Edge<CodeSetDefinition>(vertices[0], vertices[1]),
            new Edge<CodeSetDefinition>(vertices[1], vertices[2]),
        };

        var graph = new CodeSetsQuikGraph(false, vertices.Count, edges.Count);
        graph.AddVertexRange(vertices);
        graph.AddEdgeRange(edges);

        var condition = new Condition(graph, graph.Vertices.Except(new[] { vertices[2] }));
        var result = condition.DependOn(vertices[2].Name, vertices[2].CodeSetType).GetResult();

        Assert.False(result.IsSuccessful);
        Assert.Equal(1, result.FailingCodeSets.Count);
        Assert.Equal(1, result.FailingCodeSets.Select(q => q.Name).Count(q => q == vertices[3].Name));
    }

    [Fact]
    public void NotDependOn_Should_Succeed_When_EveryTestedCodeSetDoesNotHaveForbiddenDependency()
    {
        var vertices = new List<CodeSetDefinition>
        {
            new CodeSetDefinition("ProjectA", CodeSetType.Project, "ProjectA"),
            new CodeSetDefinition("ProjectB", CodeSetType.Project, "ProjectB"),
            new CodeSetDefinition("ProjectC", CodeSetType.Project, "ProjectC"),
            new CodeSetDefinition("ProjectD", CodeSetType.Project, "ProjectD")
        };

        var edges = new List<Edge<CodeSetDefinition>>
        {
            new Edge<CodeSetDefinition>(vertices[0], vertices[1]),
            new Edge<CodeSetDefinition>(vertices[1], vertices[2]),
        };

        var graph = new CodeSetsQuikGraph(false, vertices.Count, edges.Count);
        graph.AddVertexRange(vertices);
        graph.AddEdgeRange(edges);

        var condition = new Condition(graph, graph.Vertices.Except(new[] { vertices[3] }));
        var result = condition.NotDependOn(vertices[3].Name, vertices[3].CodeSetType).GetResult();

        Assert.True(result.IsSuccessful);
        Assert.Empty(result.FailingCodeSets);
    }

    [Fact]
    public void NotDependOn_Should_Fail_When_AnyTestedCodeSetHasForbiddenDependency()
    {
        var vertices = new List<CodeSetDefinition>
        {
            new CodeSetDefinition("ProjectA", CodeSetType.Project, "ProjectA"),
            new CodeSetDefinition("ProjectB", CodeSetType.Project, "ProjectB"),
            new CodeSetDefinition("ProjectC", CodeSetType.Project, "ProjectC"),
            new CodeSetDefinition("ProjectD", CodeSetType.Project, "ProjectD")
        };

        var edges = new List<Edge<CodeSetDefinition>>
        {
            new Edge<CodeSetDefinition>(vertices[0], vertices[1]),
            new Edge<CodeSetDefinition>(vertices[1], vertices[2]),
            new Edge<CodeSetDefinition>(vertices[0], vertices[3]),
        };

        var graph = new CodeSetsQuikGraph(false, vertices.Count, edges.Count);
        graph.AddVertexRange(vertices);
        graph.AddEdgeRange(edges);

        var condition = new Condition(graph, graph.Vertices.Except(new[] { vertices[3] }));
        var result = condition.NotDependOn(vertices[3].Name, vertices[3].CodeSetType).GetResult();

        Assert.False(result.IsSuccessful);
        Assert.Equal(1, result.FailingCodeSets.Count);
        Assert.Equal(1, result.FailingCodeSets.Select(q => q.Name).Count(q => q == vertices[0].Name));
    }
}