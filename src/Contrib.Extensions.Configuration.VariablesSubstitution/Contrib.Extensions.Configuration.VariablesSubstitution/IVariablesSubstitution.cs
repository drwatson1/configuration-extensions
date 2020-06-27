namespace Contrib.Extensions.Configuration.VariablesSubstitution
{
    public interface IVariablesSubstitution<T>
    {
        T Substitute(T value);
    }
}
