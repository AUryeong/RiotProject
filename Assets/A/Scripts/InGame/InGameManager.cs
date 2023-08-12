using UnityEngine;

public class InGameManager : Singleton<InGameManager>
{
    private Camera mainCamera;
    private Vector3 cameraDistance;
    public bool isAlice;
    public float alice;
    private bool aris;

    [SerializeField] GlobalObjectFogController obj2;
    private void Start()
    {
        mainCamera = Camera.main;
        cameraDistance = mainCamera.transform.position - Player.Instance.transform.position;
        SoundManager.Instance.PlaySound("Pricat", ESoundType.BGM, 0.5f);
    }
    private void Update()
    {
        CameraMove();
    }

    private void CameraMove()
    {
        if (Player.Instance.IsAlive)
        {
            mainCamera.transform.position = Player.Instance.transform.position + cameraDistance;
            if (isAlice && !aris && Player.Instance.transform.position.z >= alice)
            {
                obj2.mainColor = Color.red;
                obj2.fogColor = new Color(1,0.5f,0.5f);
                var obj = PoolManager.Instance.Init("Trans Effect");
                obj.transform.position = Player.Instance.transform.position;
                aris = true;
                SoundManager.Instance.PlaySound("Uragiri", ESoundType.BGM, 0.5f);
                Player.Instance.speed += 10;
            }
        }
    }
}