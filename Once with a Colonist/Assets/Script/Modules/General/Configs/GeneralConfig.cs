using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "GeneralConfig", fileName = "GeneralConfig")]
public class GeneralConfig : ScriptableObject
{
    [field: SerializeField, Scene]
    public string MenuScene { get; set; }
    [field: SerializeField, Scene]
    public string GameplayScene { get; set; }
}