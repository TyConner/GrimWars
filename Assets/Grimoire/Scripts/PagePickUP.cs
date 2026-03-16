using UnityEngine;

public class PagePickUp : MonoBehaviour
{
    [SerializeField] AudioClip PickUpSFX;
    [Range(0f, 1f)][SerializeField] float PickUPVol;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        iUseItems user = collision.gameObject.GetComponent<iUseItems>();
        if (user != null)
        {
            user.PagePickup();
            if (PickUpSFX != null)
            {
                AudioSource.PlayClipAtPoint(PickUpSFX, transform.position, PickUPVol);
            }

            Destroy(gameObject);
        }
    }
}
