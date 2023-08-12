using UnityEngine;

public class Rune : MonoBehaviour
{
    public const float RUNE_HP_HEAL_VALUE = 2;
    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            Player.Instance.Hp += RUNE_HP_HEAL_VALUE;
        }
    }
}