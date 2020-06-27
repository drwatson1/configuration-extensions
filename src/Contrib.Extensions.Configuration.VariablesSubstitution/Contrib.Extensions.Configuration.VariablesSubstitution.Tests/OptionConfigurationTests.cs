using System;
using Xunit;

namespace Contrib.Extensions.Configuration.VariablesSubstitution.Tests
{
    public class OptionConfigurationTests
    {
        IOptionConfiguration CreateConfigurator()
        {
            return null;
        }

        [Fact]
        public void Configure_WhenOptionIsNull_ShouldNotThrow()
        {
            var conf = CreateConfigurator();
        }
    }
}
