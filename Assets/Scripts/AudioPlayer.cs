using System;
using UnityEngine;
using FMODUnity;
public class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one AudioPlayer in the scene");
        }
        instance = this;
    }
    
    public void PlayOneShot(EventReference sound, Vector2 position)
    {
        RuntimeManager.PlayOneShot(sound, position);
    }

}