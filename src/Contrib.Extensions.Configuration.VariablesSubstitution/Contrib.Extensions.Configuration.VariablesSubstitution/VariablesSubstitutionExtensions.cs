using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;
using Contrib.Extensions.Configuration.VariablesSubstitution;

[assembly: InternalsVisibleTo("VariableSubstitution.Tests")]

namespace Contrib.Extensions.Configuration
{
    public static class VariablesSubstitutionExtensions
    {
        public static OptionsBuilder<TOption> SubstituteVariables<TOption>(this OptionsBuilder<TOption> builder) where TOption : class
        {
            builder.Services.TryAddSingleton<IVariablesSubstitution<string>, EnvironmentVariablesSubstitution>();
            builder.Services.TryAddSingleton<IOptionConfigurator, OptionConfigurator>();

            builder.Configure<IOptionConfigurator>((opt, conf) =>
            {
                conf.Configure(opt);
            });
            return builder;
        }
    }
}
