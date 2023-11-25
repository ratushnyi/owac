using UnityEngine;

namespace TendedTarsier
{
    [CreateAssetMenu(menuName = "GameplayConfig", fileName = "GameplayConfig")]
    public class GameplayConfig : ScriptableObject
    {
        [field: SerializeField]
        public float MovementSpeed { get; set; }
    }

}