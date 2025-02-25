using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class Reverb : MonoBehaviour
{
    private string _reverbZone = "Main";
    private EventInstance _reverbInstance;
    private EventInstance _mainReverbInstance;
    private EventInstance _caveReverbInstance;
    private EventInstance _houseReverbInstance;

    private void Start()
    {
        _reverbZone = FMODAudioManager.instance.ReverbZone;
        _reverbInstance = RuntimeManager.CreateInstance(FMODAudioManager.instance.Reverb);
        _mainReverbInstance = RuntimeManager.CreateInstance(FMODAudioManager.instance.MainReverb);
        _caveReverbInstance = RuntimeManager.CreateInstance(FMODAudioManager.instance.CaveReverb);
        _houseReverbInstance = RuntimeManager.CreateInstance(FMODAudioManager.instance.HouseReverb);
        _reverbInstance.setParameterByNameWithLabel("ReverbZone", _reverbZone, false);
        _reverbInstance.start();
        _mainReverbInstance.start(); 
    }

    private void Update()
    {
        Debug.Log("Reverb Zone: " + _reverbZone);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _reverbZone = "Cave";
            _reverbInstance.setParameterByNameWithLabel("ReverbZone", _reverbZone, false);
            _mainReverbInstance.stop(STOP_MODE.ALLOWFADEOUT); 
            _caveReverbInstance.start();

        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _reverbZone = "Main";
            _reverbInstance.setParameterByNameWithLabel("ReverbZone", _reverbZone, false);
            _caveReverbInstance.stop(STOP_MODE.ALLOWFADEOUT); 
            _mainReverbInstance.start();
        }
    }
}