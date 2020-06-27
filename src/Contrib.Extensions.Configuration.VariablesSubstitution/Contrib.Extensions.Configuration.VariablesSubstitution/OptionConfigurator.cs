using System;
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

            var propsForUpdate = props.Where(x => x.CanRead && x.CanWrite && x.PropertyType == typeof(string));

            foreach (var p in propsForUpdate)
            {
                var setMethod = p.SetMethod;
                if( !setMethod.IsPublic )
                {
                    continue;
                }

                var value = p.GetValue(option) as string;
                if (value != null)
                {
                    p.SetValue(option, Substitution.Substitute(value));
                }
            }
        }
    }
}
