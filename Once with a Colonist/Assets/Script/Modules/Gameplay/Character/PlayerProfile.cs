using System;
using MemoryPack;
using UnityEngine;
using TendedTarsier.Script.Modules.General.Profile;

namespace TendedTarsier.Script.Modules.Gameplay.Character
{
    [MemoryPackable(GenerateType.VersionTolerant)]
    public partial class PlayerProfile : ProfileBase
    {
        public override string Name => "Player";
            
        [MemoryPackOrder(0)]
        public DateTime? StartDate { get; set; }
            
        [MemoryPackOrder(1)]
        public DateTime? LastSaveDate { get; set; }
        [MemoryPackOrder(2)]
        public Vector2 PlayerPosition { get; set; }

        public void OnSessionStarted()
        {
            StartDate ??= DateTime.UtcNow;
            LastSaveDate = DateTime.UtcNow;
            Save();
        }
        
        public void OnSessionEnded(Vector2 position)
        {
            PlayerPosition = position;
            LastSaveDate = DateTime.UtcNow;
            Save();
        }
    }
}