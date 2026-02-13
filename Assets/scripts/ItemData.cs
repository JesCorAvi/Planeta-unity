using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Mineral", menuName = "Inventario/Item Data")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string itemName;
    [SerializeField] private Color displayColor = Color.white;

    public string ItemName => itemName;
    public Color DisplayColor => displayColor;
}