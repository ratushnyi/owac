using UnityEngine;
using UnityEngine.Tilemaps;

namespace TendedTarsier
{
    [CreateAssetMenu(menuName = "GameplayConfig", fileName = "GameplayConfig")]
    public class GameplayConfig : ScriptableObject
    {
        [field: SerializeField]
        public float MovementSpeed { get; set; }
    }

}