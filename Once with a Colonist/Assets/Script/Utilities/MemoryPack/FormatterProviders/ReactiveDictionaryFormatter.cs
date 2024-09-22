#nullable enable
using MemoryPack.Formatters;
using UniRx;

namespace TendedTarsier
{
#pragma warning disable CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.
    public class ReactiveDictionaryFormatter<TKey, TValue> : GenericDictionaryFormatterBase<ReactiveDictionary<TKey, TValue>, TKey, TValue>
#pragma warning restore CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.
        where TKey : notnull
    {
        protected override ReactiveDictionary<TKey, TValue> CreateDictionary()
        {
            return new ReactiveDictionary<TKey, TValue>();
        }
    }
}