using System;
using FMOD;
using FMOD.Studio;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AudioTestPlayer : MonoBehaviour
{
    private ATTRIBUTES_3D attributes;
    private EventInstance _braamInstance;
    private EventInstance _punchInstance;
    private GameObject _attachedGameObject;

    private void Awake()
    {
        _braamInstance = FMODUnity.RuntimeManager.CreateInstance(FMODAudioManager.instance.Braam);
        _punchInstance = FMODUnity.RuntimeManager.CreateInstance(FMODAudioManager.instance.Punch);
        _attachedGameObject = gameObject; // Reference to the GameObject the script is attached to
    }

    private void Start()
    {
        try
        {
            if (transform != null)
            {
                attributes = FMODUnity.RuntimeUtils.To3DAttributes(transform);
                FMODUnity.RuntimeManager.AttachInstanceToGameObject(_braamInstance, _attachedGameObject, true);
                FMODUnity.RuntimeManager.AttachInstanceToGameObject(_punchInstance, _attachedGameObject, true);

                // _braamInstance.set3DAttributes(attributes);
                // _braamInstance.start();
                
                _punchInstance.set3DAttributes(attributes);
                _punchInstance.start();
            }
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