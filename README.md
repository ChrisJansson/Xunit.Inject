Xunit.Inject
============

Dependency injection of test dependencies for xunit

Why
==
The conventional approach for sharing test fixture setup is by inheritance or IUseFixture, however both have their drawbacks. Inheritance can become unweildy when the inheritance chain gets to deep or simply that the is-a relashionship doesn't fit. The IUseFixture is setup once for the test class so the case when dependencies has to be cleared after each test is left unsolved.

How
==

Given a test fixture that takes a dependency
```csharp
public class TestClass 
{
  private IDependency _dependency;
  public TestClass(IDependency dependency)
  {
    _dependency = dependency;
  }
  
  [Fact]
  public void TestCase()
  {
    _dependency.Use();
  }
}
```

Configure InjectedTestClassCommand by inheriting from it and implementing the GetFactory method by either using the primitive TestFixtureFactory that is included or implement the ITestFixtureFactory using any other means like an IOC container.

```csharp
public class MyInjectedTestClassCommand : InjectedTestClassCommand
{
  protected override ITestFixtureFactory GetFactory(Type type)
  {
      var testFixtureFactory = new TestFixtureFactory(type);
      testFixtureFactory.Configure<IDependency>(x => new Dependency());
      return testFixtureFactory;
  }
}
```

Decorate the fixture with the RunWith attribute

```csharp
[RunWith(typeof(MyInjectedTestClassCommand))]
public class TestClass
{
...
}
```

Run your tests.

Cleanup
==
Dependencies that need cleanup just have to implement IDisposable as usual.
