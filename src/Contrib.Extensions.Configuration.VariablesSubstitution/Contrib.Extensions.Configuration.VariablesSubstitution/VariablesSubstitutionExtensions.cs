using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("VariableSubstitution.Tests")]

namespace Contrib.Extensions.Configuration.VariablesSubstitution
{
    public static class VariablesSubstitutionExtensions
    {
        public static OptionsBuilder<TOption> SubstituteVariables<TOption>(this OptionsBuilder<TOption> builder) where TOption : class
        {
            builder.Configure<IOptionConfigurator, IServiceCollection>((opt, conf, services) =>
            {
                services.TryAddSingleton<IOptionConfigurator, OptionConfigurator>();

                conf.Configure(opt);
            });
            return builder;
        }
    }
}
