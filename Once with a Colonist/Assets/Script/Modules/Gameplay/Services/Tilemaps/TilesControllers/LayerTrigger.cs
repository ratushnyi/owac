using UnityEngine;

namespace TendedTarsier.Script.Modules.Gameplay.Services.Tilemaps.TilesControllers
{
    public class LayerTrigger : MonoBehaviour
    {
        [SerializeField]
        private string _layer;

        [SerializeField]
        private string _sortingLayer;

        private void OnTriggerExit2D(Collider2D other)
        {
            other.gameObject.layer = LayerMask.NameToLayer(_layer);

            other.gameObject.GetComponent<SpriteRenderer>().sortingLayerName = _sortingLayer;
            var srs = other.gameObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sr in srs)
            {
                sr.sortingLayerName = _sortingLayer;
            }
        }
    }
}