using TMPro;
using UnityEngine;

public class LobbyUIManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI runeText;

    private void Start()
    {
        Active(!GameManager.Instance.isGaming);
    }
    public void Active(bool isOn)
    {
        canvas.gameObject.SetActive(isOn);
        if (isOn)
        {
            runeText.text = GameManager.Instance.rune.ToString();
        }
    }
}