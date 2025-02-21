using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioTestPlayer : MonoBehaviour
{
    private EventInstance _braamInstance;

    private void Start()
    {
        try
        {
            _braamInstance = RuntimeManager.CreateInstance(FMODAudioManager.Instance.Braam);
            _braamInstance.start();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to start Braam instance: {e.Message}");
        }
    }
 
    private void OnDestroy()
    {
        if (_braamInstance.isValid())
        {
            _braamInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _braamInstance.release();
        }
    }
}
