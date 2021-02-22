using System;
using Xunit;
using Contrib.Extensions.Configuration.VariablesSubstitution;
using FluentAssertions;
using FakeItEasy;
using System.Collections.Generic;
using System.Collections.ObjectModel;

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

		class StringArrayOption
		{
			public string[] Value { get; set; }
		}

		class ReadOnlyStringListOption
		{
			public IReadOnlyList<string> Value { get; set; } = new ReadOnlyCollection<string>(new string[] { "val1", "val2" });
		}

		class NetstedOptionsListOption
		{
			public IList<SimpleOption> Value { get; set; }
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

			public string Value { get; private set; } = "Private setter value";
		}

		class OptionWithNestedOptions
		{
			public class NestedOptions
			{
				public string Value { get; set; }
			}

			public NestedOptions Nested { get; set; }
		}

		class OptionWithDictionary
		{
			public Dictionary<string, string> Value { get; set; }
		}

		class OptionWithDictionaryOfObjects
		{
			public class ObjectWithString
			{
				public ObjectWithString(string value)
				{
					Value = value;
				}

				public string Value { get; set; }
			}

			public Dictionary<string, ObjectWithString> Value { get; set; }
		}

		class OptionWithReadOnlyDictionary
		{
			public ReadOnlyDictionary<string, string> Value { get; set; }
		}

		IVariablesSubstitution<string> CreateSubst(string value)
		{
			var subst = A.Fake<IVariablesSubstitution<string>>();
			A.CallTo(() => subst.Substitute(A<string>.Ignored)).Returns(value);

			return subst;
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
		public void Configure_WhenOptioNonPublicSettersProperty_ShouldNotSubstituteValue()
		{
			var substitutedValue = "SubstitutedValue";
			var conf = CreateConfigurator(CreateSubst(substitutedValue));

			var originalValue = "OriginalValue";
			var option = new OptionWithNonPublicSetter(originalValue);
			conf.Configure(option);

			option.Value.Should().Be(originalValue);
		}

		[Fact]
		public void Configure_WhenOptionHasPublicReadWriteProperty_ShouldSubstituteValue()
		{
			var substitutedValue = "SubstitutedValue";
			var conf = CreateConfigurator(CreateSubst(substitutedValue));

			var originalValue = "OriginalValue";
			var option = new SimpleOption() { Value1 = originalValue };

			conf.Configure(option);

			option.Value1.Should().Be(substitutedValue);
		}

		[Fact]
		public void Configure_WhenOptionHasNestedOption_ShouldSubstituteNestedValue()
		{
			var substitutedValue = "SubstitutedValue";
			var conf = CreateConfigurator(CreateSubst(substitutedValue));

			var originalValue = "OriginalValue";
			var option = new OptionWithNestedOptions() { Nested = new OptionWithNestedOptions.NestedOptions() { Value = originalValue } };

			conf.Configure(option);

			option.Nested.Value.Should().Be(substitutedValue);
		}

		[Fact]
		public void Configure_WhenOptionHasStringArrayProperty_ShouldSubstituteAllValues()
		{
			var substitutedValue = "SubstitutedValue";
			var conf = CreateConfigurator(CreateSubst(substitutedValue));

			var originalValue1 = "OriginalValue1";
			var originalValue2 = "OriginalValue2";
			var option = new StringArrayOption() { Value = new[] { originalValue1, originalValue2 } };

			conf.Configure(option);

			option.Value[0].Should().Be(substitutedValue);
			option.Value[1].Should().Be(substitutedValue);
		}

		[Fact]
		public void Configure_WhenOptionHasReadOnlyStringListProperty_ShouldNotSubstituteValues()
		{
			var substitutedValue = "SubstitutedValue";
			var conf = CreateConfigurator(CreateSubst(substitutedValue));

			var option = new ReadOnlyStringListOption();

			conf.Configure(option);

			option.Value[0].Should().NotBe(substitutedValue);
			option.Value[1].Should().NotBe(substitutedValue);
		}

		[Fact]
		public void Configure_WhenStringArrayPropertyHasNullValue_ShouldSubstituteAllNoneNullValues()
		{
			var substitutedValue = "SubstitutedValue";
			var conf = CreateConfigurator(CreateSubst(substitutedValue));

			var originalValue1 = "OriginalValue1";
			var originalValue2 = "OriginalValue2";
			var option = new StringArrayOption() { Value = new[] { originalValue1, null, originalValue2 } };

			conf.Configure(option);

			option.Value[0].Should().Be(substitutedValue);
			option.Value[1].Should().BeNull();
			option.Value[2].Should().Be(substitutedValue);
		}

		[Fact]
		public void Configure_WhenOptionHasNestedOptionsArrayProperty_ShouldSubstituteAllValuesInTheNestedOptions()
		{
			var substitutedValue = "SubstitutedValue";
			var conf = CreateConfigurator(CreateSubst(substitutedValue));

			var originalValue = "OriginalValue1";
			var option = new NetstedOptionsListOption() { Value = new[] { new SimpleOption() { Value1 = originalValue } } };

			conf.Configure(option);

			option.Value[0].Value1.Should().Be(substitutedValue);
		}

		[Fact]
		public void Configure_WhenOptionHasADictionaryProperty_ShouldSubstituteAllValuesInTheDictionary()
		{
			var substitutedValue = "SubstitutedValue";
			var conf = CreateConfigurator(CreateSubst(substitutedValue));

			var originalValue1 = "OriginalValue1";
			var originalValue2 = "OriginalValue2";
			var option = new OptionWithDictionary()
			{
				Value = new Dictionary<string, string>
				{ { "s1", originalValue1}, { "s2", originalValue2 } }
			};

			conf.Configure(option);

			option.Value["s1"].Should().Be(substitutedValue);
			option.Value["s2"].Should().Be(substitutedValue);
		}

		[Fact]
		public void Configure_WhenOptionHasAReadOnlyDictionaryProperty_ShouldNotSubstituteValuesInTheDictionary()
		{
			var substitutedValue = "SubstitutedValue";
			var conf = CreateConfigurator(CreateSubst(substitutedValue));

			var originalValue1 = "OriginalValue1";
			var originalValue2 = "OriginalValue2";
			var option = new OptionWithReadOnlyDictionary()
			{
				Value = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
				{ { "s1", originalValue1}, { "s2", originalValue2 } })
			};

			conf.Configure(option);

			option.Value["s1"].Should().Be(originalValue1);
			option.Value["s2"].Should().Be(originalValue2);
		}

		[Fact]
		public void Configure_WhenOptionHasADictionaryPropertyWithObjectValues_ShouldSubstituteValuesInTheDictionary()
		{
			var substitutedValue = "SubstitutedValue";
			var conf = CreateConfigurator(CreateSubst(substitutedValue));

			var originalValue1 = "OriginalValue1";
			var originalValue2 = "OriginalValue2";
			var option = new OptionWithDictionaryOfObjects()
			{
				Value = new Dictionary<string, OptionWithDictionaryOfObjects.ObjectWithString>
				{
					{ "s1", new OptionWithDictionaryOfObjects.ObjectWithString(originalValue1)},
					{ "s2", new OptionWithDictionaryOfObjects.ObjectWithString(originalValue2) }
				}
			};

			conf.Configure(option);

			option.Value["s1"].Value.Should().Be(substitutedValue);
			option.Value["s2"].Value.Should().Be(substitutedValue);
		}
	}
}
