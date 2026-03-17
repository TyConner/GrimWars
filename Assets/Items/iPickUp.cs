using UnityEngine;

public class iPickUp : MonoBehaviour
{
    public Items itemType;

    void Start()
    {

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Inventory inventory = collider.GetComponent<Inventory>();
        PlayerController player = collider.GetComponent<PlayerController>();

        if (inventory != null)
        {
            inventory.AddItem(itemType);

            if (player != null)
            {
                player.PlayPickupSound();
            }

            Destroy(gameObject);
        }
    }
}
