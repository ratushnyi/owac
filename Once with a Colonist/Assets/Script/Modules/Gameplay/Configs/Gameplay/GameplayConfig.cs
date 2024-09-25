using UnityEngine;

namespace TendedTarsier.Script.Modules.Gameplay.Configs.Gameplay
{
    [CreateAssetMenu(menuName = "Config/GameplayConfig", fileName = "GameplayConfig")]
    public class GameplayConfig : ScriptableObject
    {
        [field: SerializeField]
        public float CameraLerpSpeed { get; set; } = 2f;

        [field: SerializeField]
        public Vector2 CameraBuffer { get; set; } = new(4, 2);

        [field: SerializeField]
        public float MovementSpeed { get; set; } = 2;

        [field: SerializeField]
        public int DropDistance { get; set; } = 5;

        [field: SerializeField]
        public int EnergyRecoveryRate { get; set; } = 10;
    }
}