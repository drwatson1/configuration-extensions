using System;
using Xunit;
using FluentAssertions;
using Contrib.Extensions.Configuration.AutoBind;
using System.ComponentModel.DataAnnotations;

namespace AutoBind.Tests
{
    public class SectionNameResolverTests
    {
        class EmptyOption
        {
        }

        class ConstFieldOption
        {
            public const string SectionName = "ConstFieldOption_SectionName";
        }

        class ConstFieldOptionFromBase : ConstFieldOption
        { 
        }

        class ConstFieldOption_DifferentName
        {
            public const string SectionName_1 = "ConstFieldOption_SectionName";
        }

        class FieldOption
        {
            public string SectionName = "ConstFieldOption_SectionName";
        }

        class StaticReadonlyFieldOption
        {
            public static readonly string SectionName = "StaticReadonlyFieldOption_SectionName";
        }

        class StaticReadonlyFieldOption_DifferentName
        {
            public static readonly string SectionName_1 = "StaticReadonlyFieldOption_SectionName";
        }

        class StaticReadWriteFieldOption
        {
            public static string SectionName = "StaticReadWriteFieldOption_SectionName";
        }

        class StaticReadonlyPropertyOption
        {
            public static string SectionName { get; }  = "StaticReadonlyPropertyOption_SectionName";
        }

        class StaticReadonlyPropertyOptionFromBase : StaticReadonlyPropertyOption
        { 
        }

        class StaticReadonlyPropertyOption_DifferentName
        {
            public static string SectionName_1 { get; } = "StaticReadonlyPropertyOption_SectionName";
        }

        class StaticReadWritePropertyOption
        {
            public static string SectionName { get; set; } = "StaticReadWritePropertyOption_SectionName";
        }

        SectionNameResolver<TOption> CreateResolver<TOption>() where TOption: class
        {
            return new SectionNameResolver<TOption>();
        }

        [Fact]
        public void Resolve_WhenOptionIsEmptyClass_ShouldReturnClassName()
        {
            // class name
            var r = CreateResolver<EmptyOption>();

            var name = r.Resolve();
 
            name.Should().Be("EmptyOption");
        }

        [Fact]
        public void Resolve_WhenOptionClassContainsPublicConstFieldWithTheNameSectionName_ShouldReturnTheFieldValue()
        {
            // const field
            var r = CreateResolver<ConstFieldOption>();

            var name = r.Resolve();

            name.Should().Be("ConstFieldOption_SectionName");
        }

        [Fact]
        public void Resolve_WhenOptionClassContainsPublicConstFieldWithTheNameSectionNameFromBaseClass_ShouldReturnTheClassName()
        {
            var r = CreateResolver<ConstFieldOptionFromBase>();

            var name = r.Resolve();

            name.Should().Be("ConstFieldOptionFromBase");
        }

        [Fact]
        public void Resolve_WhenOptionClassContainsPublicConstFieldWithADifferentName_ShouldReturnTheClassName()
        {
            var r = CreateResolver<ConstFieldOption_DifferentName>();

            var name = r.Resolve();

            name.Should().Be("ConstFieldOption_DifferentName");
        }

        [Fact]
        public void Resolve_WhenOptionClassContainsPublicInstanceFieldWithTheNameSectionName_ShouldReturnClassName()
        {
            var r = CreateResolver<FieldOption>();

            var name = r.Resolve();

            name.Should().Be("FieldOption");
        }

        [Fact]
        public void Resolve_WhenOptionClassContainsPublicStaticReadonlyFieldWithTheNameSectionName_ShouldReturnTheFieldValue()
        {
            // static readonly field
            var r = CreateResolver<StaticReadonlyFieldOption>();

            var name = r.Resolve();

            name.Should().Be("StaticReadonlyFieldOption_SectionName");
        }

        [Fact]
        public void Resolve_WhenOptionClassContainsPublicStaticReadonlyFieldWithADifferentName_ShouldReturnTheClassName()
        {
            var r = CreateResolver<StaticReadonlyFieldOption_DifferentName>();

            var name = r.Resolve();

            name.Should().Be("StaticReadonlyFieldOption_DifferentName");
        }

        [Fact]
        public void Resolve_WhenOptionClassContainsPublicStaticReadWriteFieldWithTheNameSectionName_ShouldReturnTheClassName()
        {
            var r = CreateResolver<StaticReadWriteFieldOption>();

            var name = r.Resolve();

            name.Should().Be("StaticReadWriteFieldOption");
        }

        [Fact]
        public void Resolve_WhenOptionClassContainsPublicStaticReadonlyPropertyWithTheNameSectionName_ShouldReturnThePropertyValue()
        {
            // static readonly property
            var r = CreateResolver<StaticReadonlyPropertyOption>();

            var name = r.Resolve();

            name.Should().Be("StaticReadonlyPropertyOption_SectionName");
        }

        [Fact]
        public void Resolve_WhenOptionClassContainsPublicStaticReadonlyPropertyWithADifferentName_ShouldReturnTheClassName()
        {
            var r = CreateResolver<StaticReadonlyPropertyOption_DifferentName>();

            var name = r.Resolve();

            name.Should().Be("StaticReadonlyPropertyOption_DifferentName");
        }

        [Fact]
        public void Resolve_WhenOptionClassContainsPublicStaticReadonlyPropertyWithADifferentNameFromBaseClass_ShouldReturnTheClassName()
        {
            var r = CreateResolver<StaticReadonlyPropertyOptionFromBase>();

            var name = r.Resolve();

            name.Should().Be("StaticReadonlyPropertyOptionFromBase");
        }

        [Fact]
        public void Resolve_WhenOptionClassContainsPublicStaticReadwritePropertyWithTheNameSectionName_ShouldReturnTheClassName()
        {
            var r = CreateResolver<StaticReadWritePropertyOption>();

            var name = r.Resolve();

            name.Should().Be("StaticReadWritePropertyOption");
        }
    }
}
