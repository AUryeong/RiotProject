using UnityEngine;

public class InGameManager : Singleton<InGameManager>
{
    private Camera mainCamera;
    private Vector3 cameraDistance;

    private bool isChangeStage;
    private float stageChangeLength;
    private float beatLength;

    [SerializeField] private GlobalObjectFogController fogController;
    
    private void Start()
    {
        mainCamera = Camera.main;
        cameraDistance = mainCamera.transform.position - Player.Instance.transform.position;
    }
    private void Update()
    {
        CameraMove();
        CheckChangeStage();
    }

    public void ChangeStage(float changeLength, float beatDataLength)
    {
        isChangeStage = true;
        stageChangeLength = changeLength;
        beatLength = beatDataLength;
    }

    private void CheckChangeStage()
    {
        if (!isChangeStage || Player.Instance.transform.position.z < stageChangeLength) return;
        
        isChangeStage = false;
        
        var changeStage = TileManager.Instance.stageTileData;
            
        fogController.mainColor = changeStage.mainColor;
        fogController.fogColor = changeStage.fogColor;
                
        var transEffect = PoolManager.Instance.Init("Trans Effect");
        transEffect.transform.position = Player.Instance.transform.position;
                
        SoundManager.Instance.PlaySound(TileManager.Instance.bgmData.bgmName, ESoundType.Bgm, 0.5f);
        SoundManager.Instance.GetAudioSource(ESoundType.Bgm).time = beatLength;
        Player.Instance.speedAddValue = changeStage.speedAdder;
    }

    private void CameraMove()
    {
        if (!Player.Instance.IsAlive) return;
        
        mainCamera.transform.position = Player.Instance.transform.position + cameraDistance;
    }
}