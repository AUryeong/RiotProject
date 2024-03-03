using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEngine;

[System.Serializable]
public class ScreenShotData
{
    public string name;
    public int width;
    public int height;
}
public class Recorder : MonoBehaviour
{
    private RecorderController recorderController;
    private RecorderControllerSettings recorderSettings;
    private ImageRecorderSettings recorderImageSettings;

    [SerializeField] private List<ScreenShotData> screenShotDataList;

    private void Setting(ScreenShotData data)
    {
        if (recorderController == null)
        { 
            recorderSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            recorderController = new RecorderController(recorderSettings);

            recorderImageSettings = ScriptableObject.CreateInstance<ImageRecorderSettings>();
            recorderImageSettings.Enabled = true;
            recorderImageSettings.OutputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;
            recorderImageSettings.CaptureAlpha = false;

            recorderSettings.AddRecorderSettings(recorderImageSettings);
        }
        string currentTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
        var output = Path.Combine(Application.dataPath, "../", "Screenshot");

        recorderImageSettings.name = data.name;
        recorderImageSettings.OutputFile = Path.Combine(output, $"{data.name}_{currentTime}");
        recorderImageSettings.imageInputSettings = new GameViewInputSettings()
        {
            OutputWidth = data.width,
            OutputHeight = data.height
        };
        recorderSettings.SetRecordModeToSingleFrame(0);
    }

    private void OnGUI()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(Capture());
        }
    }

    private IEnumerator Capture()
    {
        foreach (var data in screenShotDataList)
        {
            Setting(data);
            recorderController.PrepareRecording();
            recorderController.StartRecording();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
