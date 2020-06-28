using System;

namespace Contrib.Extensions.Configuration.VariablesSubstitution
{
    internal class EnvironmentVariablesSubstitution : IVariablesSubstitution<string>
    {
        public string Substitute(string value)
            => value == null
                ? null
                : Environment.ExpandEnvironmentVariables(value);
    }
}
