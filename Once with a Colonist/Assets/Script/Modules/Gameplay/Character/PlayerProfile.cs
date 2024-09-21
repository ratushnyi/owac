using System;
using MemoryPack;
using UnityEngine;

namespace TendedTarsier.Script.Modules.Gameplay.Character
{
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial class PlayerProfile : ProfileBase
    {
        public override string Name => "Player";
            
        [MemoryPackOrder(0)]
        public DateTime? StartDate { get; set; }
        [MemoryPackOrder(1)]
        public Vector2 PlayerPosition { get; set; }
    }
}