using UnityEngine;

public class RecordButton : GorillaPressableButton
{
    public RecordManager recordManager;
    public RecordManager.RecordButtonSetting currentSetting = RecordManager.RecordButtonSetting.Record;

    public override void ButtonActivation()
    {
        switch (currentSetting)
        {
            case RecordManager.RecordButtonSetting.Record:
                recordManager.StartRecording();
                break;
            case RecordManager.RecordButtonSetting.Stop:
                recordManager.StopRecording();
                break;
            case RecordManager.RecordButtonSetting.Play:
                recordManager.PlayRecording();
                break;
            case RecordManager.RecordButtonSetting.RecordAndPlay:
                recordManager.StartRecordingAndPlaying();
                break;
        }
    }
}
