using TendedTarsier.Script.Modules.Gameplay.Character;
using UnityEngine;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Map.MapControllers
{
    public class LayerTrigger : MonoBehaviour
    {
        [SerializeField]
        private string _layer;

        [SerializeField]
        private string _sortingLayer;

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.isTrigger && other.CompareTag(GameplayInstaller.PlayerTag))
            {
                other.GetComponent<PlayerController>().ApplyLayer(_layer, _sortingLayer);
            }
        }
    }
}