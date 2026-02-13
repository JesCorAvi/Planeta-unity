using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI inventoryText;

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

        foreach (var entry in inventory.OrderBy(x => x.Key.ItemName))
        {
            ItemData item = entry.Key;
            int count = entry.Value;

            string hexColor = "#" + ColorUtility.ToHtmlStringRGB(item.DisplayColor);

            displayText += $"<color={hexColor}>{item.ItemName}</color> <pos=70%>{count}\n";
        }

        inventoryText.text = displayText;
    }
}