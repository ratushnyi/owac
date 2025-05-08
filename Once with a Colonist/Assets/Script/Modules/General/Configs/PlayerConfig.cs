using TendedTarsier.Script.Modules.General.Configs.Stats;
using UnityEngine;

namespace TendedTarsier.Script.Modules.General.Configs
{
    [CreateAssetMenu(menuName = "Config/PlayerConfig", fileName = "PlayerConfig")]
    public class PlayerConfig : ScriptableObject
    {
        [field: SerializeField]
        public StatFeeModel RunFee { get; set; }

        [field: SerializeField]
        public int WalkSpeed { get; set; } = 2;

        [field: SerializeField]
        public int RunSpeed { get; set; } = 4;

        [field: SerializeField]
        public int DropDistance { get; set; } = 2;
    }
}