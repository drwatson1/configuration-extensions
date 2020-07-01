using System.Linq;

namespace Contrib.Extensions.Configuration.VariablesSubstitution
{
    internal class OptionConfigurator : IOptionConfigurator
    {
        IVariablesSubstitution<string> Substitution { get; }

        public OptionConfigurator(IVariablesSubstitution<string> substitution)
        {
            Substitution = substitution;
        }

        public void Configure<TOption>(TOption option) where TOption : class
        {
            if (option == null)
            {
                return;
            }

            var props = option.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            var propsForUpdate = props.Where(x => x.CanRead && x.CanWrite);

            foreach (var p in propsForUpdate)
            {
                var setMethod = p.SetMethod;
                if (!setMethod.IsPublic)
                {
                    continue;
                }

                if (p.GetValue(option) is string value)
                {
                    if( value != null )
                    {
                        p.SetValue(option, Substitution.Substitute(value));
                    }
                }
                else if (p.GetValue(option) is object nestedOption)
                {
                    Configure(nestedOption);
                }
            }
        }
    }
}
