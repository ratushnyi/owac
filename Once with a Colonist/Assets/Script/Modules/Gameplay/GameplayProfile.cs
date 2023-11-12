using System;
using MemoryPack;
using UnityEngine;

namespace TendedTarsier
{
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial class GameplayProfile : ProfileBase
    {
        public override string Name => "Gameplay";

        [MemoryPackOrder(0)]
        public DateTime? StartDate { get; set; }
        [MemoryPackOrder(1)]
        public Vector2 PlayerPosition { get; set; }

    }
}