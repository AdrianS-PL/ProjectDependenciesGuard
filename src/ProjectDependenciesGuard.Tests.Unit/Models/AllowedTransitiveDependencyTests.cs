using ProjectDependenciesGuard.Models;

namespace ProjectDependenciesGuard.Tests.Unit.Models;

public class AllowedTransitiveDependencyTests
{
    [Theory]
    [InlineData("ProjectA", "ProjectB", true)]
    [InlineData("ProjectB", "ProjectA", false)]
    [InlineData("ProjectC", "ProjectB", false)]
    [InlineData("ProjectA", "ProjectC", false)]
    public void Should_AcceptOnlySpecificDependency_When_BothSourceAndTargetDefined(string sourceNameToCheck, string targetNameToCheck, bool expectedResult)
    {
        string sourceName = "ProjectA";
        string targetName = "ProjectB";

        var allowed = AllowedTransitiveDependency.AllowForSourceAndTarget(sourceName, targetName);

        Assert.Equal(expectedResult, allowed.IsDependencyExceptional(sourceNameToCheck, targetNameToCheck));
    }

    [Theory]
    [InlineData("ProjectA", "ProjectB", true)]
    [InlineData("ProjectB", "ProjectA", false)]
    [InlineData("ProjectC", "ProjectB", true)]
    [InlineData("ProjectA", "ProjectC", false)]
    public void Should_AcceptAnyForTarget_When_OnlyTargetDefined(string sourceNameToCheck, string targetNameToCheck, bool expectedResult)
    {
        string targetName = "ProjectB";

        var allowed = AllowedTransitiveDependency.AllowAnyForTarget(targetName);

        Assert.Equal(expectedResult, allowed.IsDependencyExceptional(sourceNameToCheck, targetNameToCheck));
    }

    [Theory]
    [InlineData("ProjectA", "ProjectB", true)]
    [InlineData("ProjectB", "ProjectA", false)]
    [InlineData("ProjectC", "ProjectB", false)]
    [InlineData("ProjectA", "ProjectC", true)]
    public void Should_AcceptAnyForSource_When_OnlySourceDefined(string sourceNameToCheck, string targetNameToCheck, bool expectedResult)
    {
        string sourceName = "ProjectA";

        var allowed = AllowedTransitiveDependency.AllowAnyForSource(sourceName);

        Assert.Equal(expectedResult, allowed.IsDependencyExceptional(sourceNameToCheck, targetNameToCheck));
    }
}
