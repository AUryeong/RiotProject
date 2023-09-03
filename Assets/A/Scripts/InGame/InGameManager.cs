using UnityEngine;

public class InGameManager : Singleton<InGameManager>
{
    private Camera mainCamera;
    private Vector3 cameraDistance;
    public InGameUIManager uiManager;

    public int Rune
    {
        get
        {
            return rune;
        }
        set
        {
            rune = value;
            uiManager.UpdateRune(rune);
        }
    }

    private int rune;
    
    private void Start()
    {
        mainCamera = Camera.main;
        cameraDistance = mainCamera.transform.position - Player.Instance.transform.position;
    }
    private void Update()
    {
        if (!Player.Instance.IsAlive && GameManager.Instance.isGaming) return;
        
        CameraMove();
    }

    private void CameraMove()
    {
        mainCamera.transform.position = Player.Instance.transform.position + cameraDistance;
    }
}