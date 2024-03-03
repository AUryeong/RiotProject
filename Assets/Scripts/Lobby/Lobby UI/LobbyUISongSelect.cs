using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LobbyUISongSelect : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI songText;
    [FormerlySerializedAs("songIconGraident")]
    [SerializeField] private Image songIconGradient;
    [SerializeField] private Image songIcon;

    [SerializeField] private TextMeshProUGUI lastScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    public RectTransform RectTransform
    {
        get
        {
            if (rectTransfrom == null)
                rectTransfrom = GetComponent<RectTransform>();
            return rectTransfrom;
        }
    }

    private RectTransform rectTransfrom;

    public void Show(BgmData bgmData, Color color, int lastScore, int highScore)
    {
        if (songText.textInfo == null) return;

        songText.text = bgmData.bgmNickName;

        lastScoreText.text = lastScore.ToString();
        highScoreText.text = highScore.ToString();

        songText.ForceMeshUpdate(true);
        var charInfo = songText.textInfo.characterInfo[0];
        songIconGradient.rectTransform.localPosition = (charInfo.topLeft + charInfo.bottomLeft) / 2 + new Vector3(-30, 0, 0);

        songIcon.color = color;
    }
}
