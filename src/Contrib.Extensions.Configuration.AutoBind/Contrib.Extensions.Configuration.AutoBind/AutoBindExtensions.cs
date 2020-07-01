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
        static IConfiguration Configuration;

        public static OptionsBuilder<TOption> AutoBind<TOption>(this OptionsBuilder<TOption> builder) where TOption : class
        {
            if( Configuration == null)
            {
                var provider = builder.Services.BuildServiceProvider();
                Configuration = provider.GetService<IConfiguration>();
            }

            builder.Bind(Configuration.GetSection(new SectionNameResolver<TOption>().Resolve()));

            return builder;
        }
    }
}
