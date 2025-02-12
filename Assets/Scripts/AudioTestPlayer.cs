using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class AudioTestPlayer : MonoBehaviour
{
    private FMOD.Studio.EventInstance _braamInstance;
    private void Start()
    {
        _braamInstance = RuntimeManager.CreateInstance(FMODAudioManager.Instance.Braam);
        _braamInstance.start();
    }
}
