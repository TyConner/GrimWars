using UnityEngine;

public class trapScript : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        ITakeDamage target = other.GetComponent<ITakeDamage>();
        if (target != null && other.GetComponent<PlayerController>() != null)
        {
            target.TakeDamage(damage);
        }
    }
}
