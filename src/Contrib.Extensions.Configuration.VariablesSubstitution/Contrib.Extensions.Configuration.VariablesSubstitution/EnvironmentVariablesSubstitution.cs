using System;

namespace Contrib.Extensions.Configuration.VariablesSubstitution
{
    internal class EnvironmentVariablesSubstitution : IVariablesSubstitution<string>
    {
        public string Substitute(string value)
        {
            if (value == null)
            {
                return null;
            }

            return Environment.ExpandEnvironmentVariables(value);
        }
    }
}
