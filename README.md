# ProjectDependenciesGuard

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=AdrianS-PL_ProjectDependenciesGuard&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=AdrianS-PL_ProjectDependenciesGuard)

A fluent API for .NET Standard 2.1 that can be used in unit tests to:
- Enforce project and package dependencies rules.
- Find duplicate transitive dependencies.

# Usage

```csharp
// Projects found in directory and subdirectories should not have transitive dependencies. Since relative path is used, existence of file TestApp.sln is checked to ensure resulting rooted path is correct. 
var result = DependencyGuard.ProjectsAndPackagesInPath(@"..\..\..\..\..\..\TestApp", "TestApp.sln")
    .Should()
    .NotHaveDuplicateTransitiveDependencies()
    .GetResult();

// Projects found in directory and subdirectories that have a name containing ".Contract." should only depend on packages
var result = DependencyGuard.ProjectsAndPackagesInPath(@"..\..\..\..\..\..\TestApp")
    .That()
    .MatchPredicate(q => q.Name.Contains(".Contract.") && q.CodeSetType == CodeSetType.Project)
    .Should()
    .OnlyDependOnSetsMatchingPredicate((testedProject, dependency) => dependency.CodeSetType == CodeSetType.Package).GetResult();
```

# Rationale

This project was inspired by a session of code refactoring where I found and removed dozens of duplicate transitive dependencies between projects. Before starting this project I examined several existing options.

I was looking for a solution that could:
- Examine dependencies declared in csproj files.
- Find duplicate transitive dependencies between solution projects and packages.
- Enforce rules for project and package dependencies. For example check if projects with name containing string ".Contract." have no dependency on other projects in solution.

Existing options examined:
- [NDepend](https://www.ndepend.com/)
  - Looks at compiled code structure, not declared dependencies.
- [Snitch](https://github.com/spectresystems/snitch)
  - Looks only at NuGet packages. Dependencies on projects are not considered.
- [NetArchTest](https://github.com/BenMorris/NetArchTest)
  - Looks at compiled code structure, not declared dependencies.
- [DependenSee](https://github.com/madushans/DependenSee)
  - Designed for visualization of declared dependencies as a graph. Not for enforcing rules.

Since I have not found what I was looking for, I decided to create my own solution. For references discovery I used code from DependenSee project. Desing of fluent API was heavily influenced by NetArchTest project.

# Limitations

- Currently only traverses csproj and vbproj files. No other file types are supported.

# Powered by
- Reference discovery service from [DependenSee](https://github.com/madushans/DependenSee)
- Heavily influenced by [NetArchTest](https://github.com/BenMorris/NetArchTest)