using System;
using Xunit;
using Contrib.Extensions.Configuration.VariablesSubstitution;
using FluentAssertions;
using FakeItEasy;

namespace VariableSubstitution.Tests
{
    public class OptionConfigurationTests
    {
        class SimpleOption
        {
            public string Value1 { get; set; }
            public string Value2 { get; set; }
            public string ReadOnlyValue { get; } = "Read-only value";
        }

        class OptionWithReadOnlyValue
        {
            public string ReadOnlyValue { get; } = "Read-only value";
        }


        class OptionWithNonPublicSetter
        {
            public OptionWithNonPublicSetter(string value)
            {
                Value = value;
            }

            public string Value { get; private set;  } = "Private setter value";
        }

        IOptionConfigurator CreateConfigurator(IVariablesSubstitution<string> subst = null)
        {
            if (subst == null)
            {
                subst = A.Fake<IVariablesSubstitution<string>>();
                A.CallTo(() => subst.Substitute(A<string>.Ignored)).ReturnsLazily((string value) => value);
            }
            return new OptionConfigurator(subst);
        }

        [Fact]
        public void Configure_WhenOptionIsNull_ShouldNotThrow()
        {
            var conf = CreateConfigurator();

            Action configure = () => conf.Configure<SimpleOption>(null);

            configure.Should().NotThrow();
        }

        [Fact]
        public void Configure_WhenOptionHasNullValue_ShouldNotThrow()
        {
            var subst = A.Fake<IVariablesSubstitution<string>>();
            A.CallTo(() => subst.Substitute(A<string>.Ignored)).Throws<NullReferenceException>();
            var conf = CreateConfigurator(subst);

            Action configure = () => conf.Configure<SimpleOption>(new SimpleOption());

            configure.Should().NotThrow();
        }

        [Fact]
        public void Configure_WhenOptionHasReadOnlyProperty_ShouldNotThrow()
        {
            var subst = A.Fake<IVariablesSubstitution<string>>();
            A.CallTo(() => subst.Substitute(A<string>.Ignored)).Throws<NullReferenceException>();
            var conf = CreateConfigurator(subst);

            Action configure = () => conf.Configure(new OptionWithReadOnlyValue());

            configure.Should().NotThrow();
        }

        [Fact]
        public void Configure_WhenCalled_ShouldNotChangePropertiesWithNonPublicSetters()
        {
            var substitutedValue = "SubstitutedValue";
            var subst = A.Fake<IVariablesSubstitution<string>>();
            A.CallTo(() => subst.Substitute(A<string>.Ignored)).Returns(substitutedValue);
            var conf = CreateConfigurator(subst);

            var originalValue = "OriginalValue";
            var option = new OptionWithNonPublicSetter(originalValue);
            conf.Configure(option);

            option.Value.Should().Be(originalValue);
        }

        [Fact]
        public void Configure_WhenCalled_ShouldSubstituteValue()
        {
            var substitutedValue = "SubstitutedValue";
            var subst = A.Fake<IVariablesSubstitution<string>>();
            A.CallTo(() => subst.Substitute(A<string>.Ignored)).Returns(substitutedValue);

            var conf = CreateConfigurator(subst);

            var originalValue = "OriginalValue";
            var option = new SimpleOption() { Value1 = originalValue };

            conf.Configure(option);

            option.Value1.Should().Be(substitutedValue);
        }
    }
}
