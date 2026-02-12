using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI")]
    public TextMeshProUGUI inventoryText;

    // Diccionario para guardar los Items y sus cantidades
    private Dictionary<ItemData, int> inventory = new Dictionary<ItemData, int>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        InitializeInventory();
        UpdateUI();
    }

    void InitializeInventory()
    {
        // Carga todos los minerales creados en la carpeta Resources/Items
        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");

        foreach (var item in allItems)
        {
            if (!inventory.ContainsKey(item))
            {
                inventory.Add(item, 0);
            }
        }
    }

    public void AddItem(ItemData data, int amount)
    {
        if (data == null) return;

        if (!inventory.ContainsKey(data))
        {
            inventory.Add(data, 0);
        }

        inventory[data] += amount;
        UpdateUI();
    }

void UpdateUI()
    {
        if (inventoryText == null) return;

        string displayText = "<align=center><size=130%><b>MINERALES</b></size></align>\n\n";

        foreach (var entry in inventory.OrderBy(x => x.Key.itemName)) 
        {
            ItemData item = entry.Key;
            int count = entry.Value;

            string hexColor = "#" + ColorUtility.ToHtmlStringRGB(item.displayColor);


            displayText += $"<color={hexColor}>{item.itemName}</color> <pos=70%>{count}\n";
        }

        inventoryText.text = displayText;
    }
}