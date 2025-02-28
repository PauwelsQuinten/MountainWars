using System;
using FMOD.Studio;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class Music : MonoBehaviour
{
    private EventInstance _musicInstance;
    private int _progress = 0;
    private float _threatLevel = 0.0f;

    private void Start()
    {
        try
        {
            _musicInstance = FMODUnity.RuntimeManager.CreateInstance(FMODAudioManager.instance.Music);
            _progress = FMODAudioManager.instance.Progress;
            _threatLevel = FMODAudioManager.instance.ThreatLevel;
            _musicInstance.start();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to start Music instance: {e.Message}");
        }
    }

    private void Update()
    {
        _progress = 2;
        _threatLevel = 0.0f;
        _musicInstance.setParameterByName("Progress", _progress, false);
        _musicInstance.setParameterByName("Threat Level", _threatLevel, false);

        Debug.Log(_progress);
    }

    private void OnDestroy()
    {
        if (_musicInstance.isValid())
        {
            _musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            _musicInstance.release();
        }
    }
}