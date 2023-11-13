using UnityEngine;
using UnityEngine.Tilemaps;

namespace TendedTarsier
{
    [CreateAssetMenu(menuName = "GameplayConfig", fileName = "GameplayConfig", order = 0)]
    public class GameplayConfig : ScriptableObject
    {
        [field: SerializeField]
        public float MovementSpeed { get; set; }
    }
}