using System.Linq;
using System.Reflection;
using MemoryPack;

namespace TendedTarsier
{
    public static class MemoryPackExtensions
    {
        public static void PopulateObject(object targetObject, byte[] byteData)
        {
            var deserializedObject = MemoryPackSerializer.Deserialize(targetObject.GetType(), byteData);

            if (deserializedObject == null)
            {
                return;
            }
            
            var sourceProperties = deserializedObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var targetProperties = targetObject.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(t => t.CanWrite);

            foreach (var targetProperty in targetProperties)
            {
                var sourceProperty = sourceProperties.FirstOrDefault(p => p.Name == targetProperty.Name);
                if (sourceProperty != null && targetProperty.PropertyType == sourceProperty.PropertyType)
                {
                    var sourcesValue = sourceProperty.GetValue(deserializedObject);
                    targetProperty.SetValue(targetObject, sourcesValue);
                }
            }
        }
    }
}