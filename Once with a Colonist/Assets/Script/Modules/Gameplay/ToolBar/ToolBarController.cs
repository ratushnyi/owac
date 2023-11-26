using UnityEngine;
using UnityEngine.UI;

namespace TendedTarsier
{
    public class ToolBarController : MonoBehaviour
    {
        [field: SerializeField]
        public Button MenuButton { get; set; }
        [field: SerializeField]
        public InventoryCellView CurrentInstrument { get; set; }
    }
}
