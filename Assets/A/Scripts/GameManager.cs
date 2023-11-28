using DG.Tweening;
using InGame;
using Lobby;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public enum SceneLinkType
{
    Lobby,
    InGame
}

public class GameManager : Singleton<GameManager>
{
    public bool isGaming = true;

    private readonly Vector2 CAMERA_RENDER_SIZE = new(720, 1600);
    private const float UPSCALE_RATIO = 0.65f;

    public Camera mainCamera;
    [SerializeField] private Camera playerRenderCamera;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private Camera renderingCamera;

    [Space(10f)]
    [SerializeField] private RawImage rawImage;

    private RenderTexture renderTexture;

    [Space(20f)] [SerializeField] private Image blackFade;
    private const float BLACK_FADE_DURATION = 0.75f;

    [Space(20f)] public GlobalObjectFogController fogController;
    public PostProcessVolume postProcessVolume;
    public Light directionLight;

    protected override void OnCreated()
    {
        base.OnCreated();

        Application.targetFrameRate = 60;

        OnReset();
        ActiveSceneLink(SceneLinkType.Lobby);
    }

    protected override void OnReset()
    {
        SetResolution();

        foreach (var canvas in FindObjectsOfType<CanvasScaler>(true))
            canvas.referenceResolution = CAMERA_RENDER_SIZE;

        UpScaleSamplingSetting();
    }

    public void ActiveSceneLink(SceneLinkType type, System.Action action = null)
    {
        switch (type)
        {
            case SceneLinkType.Lobby:
                blackFade.gameObject.SetActive(true);
                blackFade.DOFade(1, BLACK_FADE_DURATION).OnComplete(() =>
                {
                    action?.Invoke();
                    
                    LobbyManager.Instance.Active();
                    InGameManager.Instance.DeActive();
                    blackFade.DOFade(0, BLACK_FADE_DURATION).OnComplete(() => blackFade.gameObject.SetActive(false));
                });
                break;
            case SceneLinkType.InGame:
                LobbyManager.Instance.DeActive();
                InGameManager.Instance.Active();
                break;
        }
    }

    private void UpScaleSamplingSetting()
    {
        if (renderTexture != null)
            renderTexture.Release();

        int setWidth = Mathf.CeilToInt(CAMERA_RENDER_SIZE.x);

        int deviceWidth = Screen.width;
        int deviceHeight = Screen.height;

        renderTexture = new RenderTexture((int)(UPSCALE_RATIO * setWidth), (int)((float)deviceHeight / deviceWidth * setWidth * UPSCALE_RATIO), 24,
            UnityEngine.Experimental.Rendering.DefaultFormat.HDR);
        renderTexture.Create();

        mainCamera.targetTexture = renderTexture;
        playerRenderCamera.targetTexture = renderTexture;

        rawImage.texture = renderTexture;
    }
    
    private void SetResolution()
    {
        int setWidth = Mathf.CeilToInt(CAMERA_RENDER_SIZE.x);
        int setHeight = Mathf.CeilToInt(CAMERA_RENDER_SIZE.y);

        int deviceWidth = Screen.width;
        int deviceHeight = Screen.height;

        Screen.SetResolution(setWidth, (int)((float)deviceHeight / deviceWidth * setWidth), true);

        float screenMultiplier = (float)setWidth / setHeight;
        float deviceMultiplier = (float)deviceWidth / deviceHeight;

        if (screenMultiplier < deviceMultiplier)
        {
            float newWidth = screenMultiplier / deviceMultiplier;
            renderingCamera.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
            uiCamera.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
            rawImage.uvRect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
        }
        else
        {
            float newHeight = deviceMultiplier / screenMultiplier;
            renderingCamera.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
            uiCamera.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
            rawImage.uvRect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
        }
    }
}