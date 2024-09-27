using TendedTarsier.Script.Modules.General.Panels;
using UnityEngine;

namespace TendedTarsier.Script.Modules.Gameplay.Panels.HUD
{
    public class InputPanel : PanelBase
    {
        public override bool ShowInstantly => Application.isMobilePlatform || Application.isEditor;
    }
}