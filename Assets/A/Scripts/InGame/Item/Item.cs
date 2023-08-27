using UnityEngine;

public class Item : MonoBehaviour
{
    protected virtual void OnGet()
    {
        gameObject.SetActive(false);
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider == null) return;

        if (collision.collider.CompareTag("Player"))
            OnGet();
    }
}