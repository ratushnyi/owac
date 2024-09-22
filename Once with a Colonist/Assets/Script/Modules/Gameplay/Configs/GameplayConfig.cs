using UnityEngine;

namespace TendedTarsier.Script.Modules.Gameplay.Configs
{
    [CreateAssetMenu(menuName = "Config/GameplayConfig", fileName = "GameplayConfig")]
    public class GameplayConfig : ScriptableObject
    {
        [field: SerializeField]
        public float MovementSpeed { get; set; } = 2;

        [field: SerializeField]
        public int DropDistance { get; set; } = 5;
    }

}