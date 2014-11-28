using System;

namespace Xunit.Inject.Tests
{
    public class TestFixtureFactoryTests
    {
        [Fact]
        public void Should_create_instance_of_registered_type()
        {
            var sut = new TestFixtureFactory(typeof(object));

            var instance = sut.Create().Fixture;

            Assert.NotNull(instance);
            Assert.IsType<object>(instance);
        }

        [Fact]
        public void Should_create_instance_of_registered_type_with_constructor_parameters_by_passing_context_to_lambda()
        {
            var sut = new TestFixtureFactory(typeof(TypeWithConstructorParameters));
            sut.Configure(() => new object());

            var instance = sut.Create().Fixture as TypeWithConstructorParameters;

            Assert.NotNull(instance);
            Assert.NotNull(instance.ConstructorParameter);
        }

        [Fact]
        public void Should_reuse_created_instances_of_same_type_in_object_graph()
        {
            var sut = new TestFixtureFactory(typeof(A));
            sut.Configure(f => new B(f.Create(typeof(object))));
            sut.Configure(f => new C(f.Create(typeof(object))));
            sut.Configure(() => new object());

            var instance = (A)sut.Create().Fixture;

            Assert.Same(instance.B.O, instance.C.O);
        }

        [Fact]
        public void Should_dispose_disposables_when_fixture_scope_is_disposed()
        {
            var sut = new TestFixtureFactory(typeof(A));
            sut.Configure(f => new B(f.Create(typeof(object))));
            sut.Configure(f => new C(f.Create(typeof(object))));
            sut.Configure(() => new object());

            var scope = sut.Create();

            scope.Dispose();
            var fixture = scope.Fixture as A;
            Assert.True(fixture.C.WasDisposed);
        }

        private class TypeWithConstructorParameters
        {
            public readonly object ConstructorParameter;
            public TypeWithConstructorParameters(object a)
            {
                ConstructorParameter = a;
            }
        }

        private class A
        {
            public readonly B B;
            public readonly C C;

            public A(B b, C c)
            {
                B = b;
                C = c;
            }
        }

        private class B
        {
            public readonly object O;

            public B(object o)
            {
                O = o;
            }
        }

        private class C : IDisposable
        {
            public readonly object O;
            public bool WasDisposed;

            public C(object o)
            {
                O = o;
            }

            public void Dispose()
            {
                WasDisposed = true;
            }
        }
    }
}