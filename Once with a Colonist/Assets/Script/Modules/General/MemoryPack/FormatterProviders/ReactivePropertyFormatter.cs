#nullable enable
using MemoryPack;
using UniRx;

namespace TendedTarsier
{
    public class ReactivePropertyFormatter<T> : MemoryPackFormatter<ReactiveProperty<T>>
    {
        public override void Serialize(ref MemoryPackWriter writer, ref ReactiveProperty<T>? value)
        {
            if (value == null)
            {
                writer.WriteNullObjectHeader();
                return;
            }

            var formatter = writer.GetFormatter<T>();
            T? serializedValue = value.Value;
            formatter.Serialize(ref writer, ref serializedValue);
        }

        public override void Deserialize(ref MemoryPackReader reader, ref ReactiveProperty<T>? value)
        {
            if (reader.PeekIsNull())
            {
                value = null;
                return;
            }

            var formatter = reader.GetFormatter<T>();
            T? deserializedValue = default;

            formatter.Deserialize(ref reader, ref deserializedValue);
            if (deserializedValue != null)
            {
                value = new ReactiveProperty<T>(deserializedValue);
            }
        }
    }
}