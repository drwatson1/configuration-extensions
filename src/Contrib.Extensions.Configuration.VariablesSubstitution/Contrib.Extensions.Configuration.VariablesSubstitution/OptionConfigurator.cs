using System;
using System.Linq;
using System.Reflection;

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
                var val = p.GetValue(option);
                if( val == null )
                {
                    continue;
                }
                if (val is string stringValue)
                {
                    var setMethod = p.SetMethod;
                    if (!setMethod.IsPublic)
                    {
                        continue;
                    }

                    UpdateStringValue(option, p, stringValue);
                }
                else if (val is object nestedOption)
                {
                    Configure(nestedOption);
                }
            }
        }

        private void UpdateStringValue<TOption>(TOption option, PropertyInfo p, string stringValue) where TOption : class
        {
            if (stringValue != null)
            {
                p.SetValue(option, Substitution.Substitute(stringValue));
            }
        }
    }
}
