using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private ItemData itemData;
    [SerializeField] private int quantity = 1;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) Collect();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) Collect();
    }

    void Collect()
    {
        if (InventoryManager.Instance != null && itemData != null)
        {
            InventoryManager.Instance.AddItem(itemData, quantity);
        }

        Destroy(gameObject);
    }
}