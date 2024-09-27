using Cysharp.Threading.Tasks;

namespace TendedTarsier.Script.Modules.Gameplay
{
    public interface IPerformable
    {
        UniTask<bool> Perform();
    }
}