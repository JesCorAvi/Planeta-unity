using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Mineral", menuName = "Inventario/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Color displayColor = Color.white;
}