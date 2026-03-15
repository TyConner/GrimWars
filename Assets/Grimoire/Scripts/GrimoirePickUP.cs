using UnityEngine;

public class GrimoirePickUP : MonoBehaviour
{
    [SerializeField] GrimoireClassData Grimoire_To_PickUp;
    [SerializeField] AudioClip PickUpSFX;
    [Range(0f,1f)][SerializeField] float PickUPVol;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        iUseItems user = collision.gameObject.GetComponent<iUseItems>();
        if(user != null)
        {
            user.GrimoirePickup(Grimoire_To_PickUp);
            if(PickUpSFX != null)
            {
                AudioSource.PlayClipAtPoint(PickUpSFX, transform.position, PickUPVol);
            }
           
            Destroy(gameObject);
        }
    }

}
