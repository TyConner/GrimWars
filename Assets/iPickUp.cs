using UnityEngine;

public class iPickUp : MonoBehaviour
{
    public Items itemType;

    void OnTriggerEnter2D(Collider2D collider)
    {
        Inventory inventory = collider.GetComponent<Inventory>();

        if (inventory != null)
        {
            inventory.AddItem(itemType);
            Destroy(gameObject);
        }
    }
}
