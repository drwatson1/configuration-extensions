using System.Linq;

namespace Contrib.Extensions.Configuration.AutoBind
{
    internal class SectionNameResolver<TOption> where TOption: class
    {
        const string FieldName = "SectionName";

        public string Resolve()
        {
            var fieldValue = typeof(TOption).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(x => x.FieldType == typeof(string) && x.Name == FieldName 
                    && (x.IsInitOnly || x.Attributes.HasFlag(System.Reflection.FieldAttributes.Literal))).Select(x => x.GetValue(null)).FirstOrDefault() as string;

            if (!string.IsNullOrWhiteSpace(fieldValue))
            {
                return fieldValue;
            }

            var propValue = typeof(TOption).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                .Where(x => x.PropertyType == typeof(string) && x.Name == FieldName && !x.CanWrite).Select(x => x.GetValue(null)).FirstOrDefault() as string;

            return propValue as string ?? typeof(TOption).Name;
        }
    }
}
