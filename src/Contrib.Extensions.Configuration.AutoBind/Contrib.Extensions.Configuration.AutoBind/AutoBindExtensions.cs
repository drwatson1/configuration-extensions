using Contrib.Extensions.Configuration.AutoBind;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AutoBind.Tests")]

namespace Contrib.Extensions.Configuration
{
    public static class AutoBindExtensions
    {
        public static OptionsBuilder<TOption> AutoBind<TOption>(this OptionsBuilder<TOption> builder, IConfiguration configuration) where TOption : class
            => builder.Bind(configuration.GetSection(new SectionNameResolver<TOption>().Resolve()));
    }
}
