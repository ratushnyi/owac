using Cysharp.Threading.Tasks;
using TendedTarsier.Script.Modules.General;
using UnityEngine;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject
{
    public abstract class DeviceMapObject : MapObjectBase, IPerformable
    {
        [SerializeField]
        private Transform _progressBarContainer;
        public Transform ProgressBarContainer => _progressBarContainer;

        private void Start()
        {
            tag = GeneralConstants.DeviceTag;
        }

        public abstract UniTask<bool> Perform();
        
        protected override void OnNetworkInitialized()
        {
        }
    }
}