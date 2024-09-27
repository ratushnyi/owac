using Cysharp.Threading.Tasks;
using TendedTarsier.Script.Modules.General;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map.MapObject
{
    public abstract class DeviceMapObject : MapObjectBase, IPerformable
    {
        private void Start()
        {
            tag = GeneralConstants.DeviceTag;
        }

        public abstract UniTask<bool> Perform();
    }
}