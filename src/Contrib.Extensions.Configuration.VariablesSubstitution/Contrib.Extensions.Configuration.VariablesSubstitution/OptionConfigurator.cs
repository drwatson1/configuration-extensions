using System;
using System.Collections;
using System.Collections.Generic;
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
                else if (val is IList listValue)
                {
                    UpdateList(listValue);
                }
                else if (val is object nestedOption)
                {
                    Configure(nestedOption);
                }
            }
        }

        private void UpdateList(IList list)
        {
            if( list.IsReadOnly )
            {
                return;
            }

            if ( list.GetType().GetElementType() == typeof(string) )
            {
                var stringList = list as IList<string>;
                for (int i = 0; i < stringList.Count; ++i)
                {
                    if(stringList[i] == null)
                    {
                        continue;
                    }

                    stringList[i] = Substitution.Substitute(stringList[i]);
                }
            }
            else if( !list.GetType().GetElementType().IsValueType )
            {
                foreach(var nestedOption in list)
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
