using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    protected override bool IsDontDestroying => true;

    protected override void OnCreated()
    {
        base.OnCreated();
        foreach (var cam in Camera.allCameras)
        {
            SetResolution(cam);
        }
    }

    private void SetResolution(Camera changeCamera)
    {
        if (changeCamera == null) return;
        int setWidth = 720;
        int setHeight = 1600;

        int deviceWidth = Screen.width;
        int deviceHeight = Screen.height;

        Screen.SetResolution(setWidth, (int)(((float)deviceHeight / deviceWidth) * setWidth), true);

        float screenMultiplier = (float)setWidth / setHeight;
        float deviceMultiplier = (float)deviceWidth / deviceHeight;

        if (screenMultiplier < deviceMultiplier)
        {
            float newWidth = screenMultiplier / deviceMultiplier;
            changeCamera.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
        }
        else
        {
            float newHeight = deviceMultiplier / screenMultiplier;
            changeCamera.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
        }
    }
}