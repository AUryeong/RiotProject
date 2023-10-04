using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyUIStageSlot : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI nickNameText;

        [SerializeField] private List<Image> bgmSlots;
        [SerializeField] private List<Image> bgmSlotTexts;
    }
}