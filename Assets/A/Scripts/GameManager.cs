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

    private readonly Vector2 CAMERA_RENDER_SIZE = new(720, 1600 - AD_SIZE_Y);
    private const int AD_SIZE_Y = 160;
    private const float UPSCALE_RATIO = 0.65f;

    [Space(10f)]
    public Camera mainCamera;
    [SerializeField] private Camera playerRenderCamera;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private Camera renderingCamera;

    [SerializeField] private Camera adCamera;
    [SerializeField] private Canvas adCanvas;

    [Space(10f)]
    [SerializeField] private RawImage rawImage;

    private RenderTexture renderTexture;

    [Space(20f)]
    [SerializeField] private Image blackFade;
    private const float BLACK_FADE_DURATION = 0.75f;

    [Space(20f)]
    public GlobalObjectFogController fogController;
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
        {
            if (adCanvas == canvas)
            {
                canvas.referenceResolution = CAMERA_RENDER_SIZE + new Vector2(0, AD_SIZE_Y);
                continue;
            }

            canvas.referenceResolution = CAMERA_RENDER_SIZE;
        }

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
        int setHeight = Mathf.CeilToInt(CAMERA_RENDER_SIZE.y) + AD_SIZE_Y;

        int deviceWidth = Screen.width;
        int deviceHeight = Screen.height;

        Screen.SetResolution(setWidth, (int)((float)deviceHeight / deviceWidth * setHeight), true);

        float screenMultiplier = (float)setWidth / setHeight;
        float deviceMultiplier = (float)deviceWidth / deviceHeight;
        float adHeight = AD_SIZE_Y / (float)setHeight;

        if (screenMultiplier < deviceMultiplier)
        {
            float newWidth = screenMultiplier / deviceMultiplier;
            float newHeight = 1 - adHeight;
            renderingCamera.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, newHeight);
            uiCamera.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, newHeight);
            adCamera.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1);
            rawImage.uvRect = new Rect((1f - newWidth) / 2f, 0f, newWidth, newHeight);
        }
        else
        {
            float newHeight = deviceMultiplier / screenMultiplier;
            adCamera.rect = new Rect(0f, 0, 1f, newHeight);
            renderingCamera.rect = new Rect(0f, 0, 1f, newHeight - adHeight);
            uiCamera.rect = new Rect(0f, 0, 1f, newHeight - adHeight);
            rawImage.uvRect = new Rect(0f, 0, 1f, newHeight - adHeight);
        }
    }
}