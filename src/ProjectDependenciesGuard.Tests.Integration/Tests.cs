using ProjectDependenciesGuard.Models;

namespace ProjectDependenciesGuard.Tests.Integration;

public class Tests
{
    private const string SlnFileName = "TestApp.sln";

    [Fact]
    public void Should_DetectDuplicateTransitiveDependencies()
    {
        var result = DependencyGuard.ProjectsAndPackagesInPath(@"..\..\..\..\..\..\TestApp", SlnFileName)
            .Should().NotHaveDuplicateTransitiveDependencies().GetResult();

        Assert.False(result.IsSuccessful);
        Assert.Equal(3, result.FailingCodeSets.Count);
        Assert.Equal(2, result.FailingCodeSets.Select(q => q.Name).Count(q => q == "TestNamespace.App1.Api"));
        Assert.Equal(1, result.FailingCodeSets.Select(q => q.Name).Count(q => q == "TestNamespace.App1.Infrastructure.Tests.Unit"));
    }

    [Fact]
    public void Should_DetectForbiddenUnitTestsDependencies()
    {
        Func<CodeSetDefinition, CodeSetDefinition, bool> allowedUnitTestProjectsDependenciesPredicate = (testedProject, dependency) => dependency.CodeSetType == CodeSetType.Package ||
            (dependency.CodeSetType == CodeSetType.Project && (dependency.Name + ".Tests.Unit" == testedProject.Name));

        var result = DependencyGuard.ProjectsAndPackagesInPath(@"..\..\..\..\..\..\TestApp", SlnFileName).That()
            .MatchPredicate(q => q.Name.EndsWith(".Tests.Unit") && q.CodeSetType == CodeSetType.Project)
            .Should().OnlyDependOnSetsMatchingPredicate(allowedUnitTestProjectsDependenciesPredicate).GetResult();

        Assert.False(result.IsSuccessful);
        Assert.Equal(1, result.FailingCodeSets.Count);
        Assert.Equal(1, result.FailingCodeSets.Select(q => q.Name).Count(q => q == "TestNamespace.App1.Infrastructure.Tests.Unit"));
    }

    [Fact]
    public void Should_CheckThatAllUnitTestsProjectsHaveReferenceToXUnit()
    {
        Func<CodeSetDefinition, CodeSetDefinition, bool> allowedUnitTestProjectsDependenciesPredicate = (testedProject, dependency) => dependency.CodeSetType == CodeSetType.Package ||
            (dependency.CodeSetType == CodeSetType.Project && (dependency.Name + ".Tests.Unit" == testedProject.Name));

        var result = DependencyGuard.ProjectsAndPackagesInPath(@"..\..\..\..\..\..\TestApp", SlnFileName).That()
            .MatchPredicate(q => q.Name.EndsWith(".Tests.Unit") && q.CodeSetType == CodeSetType.Project)
            .Should().DependOn("xunit", CodeSetType.Package).GetResult();

        Assert.True(result.IsSuccessful);
        Assert.Empty(result.FailingCodeSets);
    }
}
