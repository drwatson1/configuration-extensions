namespace Contrib.Extensions.Configuration.VariablesSubstitution
{
    internal interface IOptionConfigurator
    {
        void Configure<TOption>(TOption option) where TOption : class;
    }
}
