using DG.Tweening;
using GoogleMobileAds.Api;
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

    private const float UPSCALE_RATIO = 0.65f;
    private static readonly Vector2 SCREEN_SIZE = new(1080, 1920);
    public Vector2 ScreenSize { get; private set; }

    [Space(10f)] public Camera mainCamera;
    [SerializeField] private Camera playerRenderCamera;
    [SerializeField] private Camera uiCamera;
    [SerializeField] private Camera renderingCamera;

    [Space(10f)] [SerializeField] private RawImage rawImage;

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
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            ActiveSceneLink(SceneLinkType.Lobby);
            return;
        }

        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            ActiveSceneLink(SceneLinkType.Lobby);
            AdMobManager.Instance.ShowBannerView();
        });
    }

    protected override void OnReset()
    {
        SetResolution();

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

        int deviceWidth = Screen.width;
        int deviceHeight = Screen.height;

        renderTexture = new RenderTexture((int)(UPSCALE_RATIO * deviceWidth), (int)(UPSCALE_RATIO * deviceHeight), 24,
            UnityEngine.Experimental.Rendering.DefaultFormat.HDR);
        renderTexture.Create();

        mainCamera.targetTexture = renderTexture;
        playerRenderCamera.targetTexture = renderTexture;

        rawImage.texture = renderTexture;
    }

    private void SetResolution()
    {
        Rect safeArea = Screen.safeArea;
        ScreenSize = new Vector2(Screen.width * (1 - safeArea.x), Screen.height * (1 - safeArea.y));

        int adSizeY = AdMobManager.Instance.GetADSizeY();

        float sizeXMultiplier = ScreenSize.x / SCREEN_SIZE.x;
        float sizeYMultiplier = ScreenSize.y / SCREEN_SIZE.y;

        float sizeMultiplier = Mathf.Min(sizeXMultiplier, sizeYMultiplier);

        foreach (var canvas in FindObjectsOfType<CanvasScaler>(true))
        {
            canvas.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            canvas.scaleFactor = 1.4f * sizeMultiplier;
        }

        float adHeight = adSizeY / ScreenSize.y;

        float height = Mathf.Min(safeArea.height / Screen.height, 1 - adHeight);
        var camRect = new Rect(safeArea.x, safeArea.y, safeArea.width / Screen.width, height);
        renderingCamera.rect = camRect;
        uiCamera.rect = camRect;
        rawImage.uvRect = camRect;
    }
}