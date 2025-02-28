using System;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using Debug = UnityEngine.Debug;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class Reverb : MonoBehaviour
{
    private string _reverbZone = "Main";
    private EventInstance _reverbInstance;
    private ATTRIBUTES_3D _attributes;

    private void Start()
    {
        _reverbZone = FMODAudioManager.instance.ReverbZone;
        _reverbInstance = RuntimeManager.CreateInstance(FMODAudioManager.instance.Reverb);
        _attributes = RuntimeUtils.To3DAttributes(transform);
        _reverbInstance.set3DAttributes(_attributes);
        _reverbInstance.setParameterByNameWithLabel("ReverbZone", _reverbZone, false);
        _reverbInstance.start();
    }

    private void Update()
    {
        // Ensure the reverb zone is updated in real-time
        _reverbInstance.setParameterByNameWithLabel("ReverbZone", _reverbZone, false);
        _attributes = RuntimeUtils.To3DAttributes(transform);
        _reverbInstance.set3DAttributes(_attributes);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _reverbZone = "Cave";
            UpdateReverbZone();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _reverbZone = "Main";
            UpdateReverbZone();
        }
    }

    private void UpdateReverbZone()
    {
        _reverbInstance.setParameterByNameWithLabel("ReverbZone", _reverbZone, false);

        // Check if the parameter was set correctly
        _reverbInstance.getParameterByName("ReverbZone", out float value);
    }

    private void OnDestroy()
    {
        if (_reverbInstance.isValid())
        {
            _reverbInstance.stop(STOP_MODE.ALLOWFADEOUT);
            _reverbInstance.release();
        }
    }
}