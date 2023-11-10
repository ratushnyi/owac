using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Create GameplayConfig", fileName = "GameplayConfig", order = 0)]
public class GameplayConfig : ScriptableObject
{
    [field: SerializeField]
    public float MovementSpeed { get; set; }

    [field: SerializeField]
    public TileBase PerformedTile { get; set; }
}