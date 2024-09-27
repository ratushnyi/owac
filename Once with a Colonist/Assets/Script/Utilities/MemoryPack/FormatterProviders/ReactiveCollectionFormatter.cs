#nullable enable
using MemoryPack;
using UniRx;

namespace TendedTarsier.Script.Utilities.MemoryPack.FormatterProviders
{
    public class ReactiveCollectionFormatter<TElement> : MemoryPackFormatter<ReactiveCollection<TElement>>
    {
        public override void Serialize(ref MemoryPackWriter writer, ref ReactiveCollection<TElement>? value)
        {
            if (value == null)
            {
                writer.WriteNullCollectionHeader();
                return;
            }

            var formatter = writer.GetFormatter<TElement?>();

            writer.WriteCollectionHeader(value.Count);
            foreach (var item in value)
            {
                TElement? serializedValue = item;
                formatter.Serialize(ref writer, ref serializedValue);
            }
        }

        public override void Deserialize(ref MemoryPackReader reader, ref ReactiveCollection<TElement>? value)
        {
            if (!reader.TryReadCollectionHeader(out var length))
            {
                value = default;
                return;
            }

            var formatter = reader.GetFormatter<TElement?>();
            var collection = new ReactiveCollection<TElement>();

            for (var i = 0; i < length; i++)
            {
                TElement? deserializedValue = default;
                formatter.Deserialize(ref reader, ref deserializedValue);
                if (deserializedValue != null)
                {
                    collection.Add(deserializedValue);
                }
            }

            value = collection;
        }
    }
}