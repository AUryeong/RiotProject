using UnityEngine;

public class Item : MonoBehaviour
{
    protected virtual void OnGet()
    {
        gameObject.SetActive(false);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            OnGet();
    }
}