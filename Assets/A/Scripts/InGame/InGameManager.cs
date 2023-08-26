using UnityEngine;

public class InGameManager : Singleton<InGameManager>
{
    private Camera mainCamera;
    private Vector3 cameraDistance;

    private bool isChangeSpeed;
    private bool isChangeStage;
    private bool isChangeHighLight;
    
    private bool isHighLighted;
    
    private float changeSpeedValue;
    
    private float speedChangePos;
    private float stageChangePos;
    private float highLightChangePos;
    
    [SerializeField] private GlobalObjectFogController fogController;
    
    private void Start()
    {
        mainCamera = Camera.main;
        cameraDistance = mainCamera.transform.position - Player.Instance.transform.position;
    }
    private void Update()
    {
        if (!Player.Instance.IsAlive) return;
        
        CameraMove();
        CheckChangeHighLight();
        CheckChangeSpeed();
        CheckChangeStage();
    }


    public void ChangeHighLight(float changeLength, bool isHighLight)
    {
        isChangeHighLight = true;
        isHighLighted = isHighLight;
        highLightChangePos = changeLength;
    }

    public void ChangeSpeedByBeatData(float changeLength, float speed)
    {
        isChangeSpeed = true;
        changeSpeedValue = speed;
        speedChangePos = changeLength;
    }

    public void ChangeStage(float changeLength)
    {
        isChangeStage = true;
        stageChangePos = changeLength;
    }

    public void ChangeThemeColor(ThemeColor themeColor, bool isHighLightSkip = false)
    {
        if (!isHighLightSkip && isHighLighted) return;
        
        fogController.mainColor = themeColor.mainColor;
        fogController.fogColor = themeColor.fogColor;
        
        var transEffect = PoolManager.Instance.Init("Trans Effect");
        transEffect.transform.position = Player.Instance.transform.position;
    }

    private void CheckChangeHighLight()
    {
        if (!isChangeHighLight || Player.Instance.transform.position.z < highLightChangePos) return;

        isChangeHighLight = false;
        
        var changeStage = TileManager.Instance.stageTileData;
        var themeColor = isHighLighted ? changeStage.highLightColor : changeStage.defaultColor;
        
        ChangeThemeColor(themeColor, true);
    }
    
    private void CheckChangeStage()
    {
        if (!isChangeStage || Player.Instance.transform.position.z < stageChangePos) return;

        isChangeStage = false;
        
        var changeStage = TileManager.Instance.stageTileData;
        var themeColor = changeStage.defaultColor;
        
        ChangeThemeColor(themeColor, true);
        
        SoundManager.Instance.PlaySound(TileManager.Instance.bgmData.bgmName, ESoundType.Bgm, 0.5f);
    }
    

    private void CheckChangeSpeed()
    {
        if (!isChangeSpeed || Player.Instance.transform.position.z < speedChangePos) return;
        
        isChangeSpeed = false;
        Player.Instance.speedAddValue += changeSpeedValue;
    }

    private void CameraMove()
    {
        mainCamera.transform.position = Player.Instance.transform.position + cameraDistance;
    }
}