using System;
using UnityEngine;
using FMODUnity;
using UnityEngine.Serialization;

public class FMODAudioManager : MonoBehaviour
{
    public static FMODAudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    //Parameters
    [SerializeField, Range(0, 2)] private int _surfaceType = 0;
    public int SurfaceType => _surfaceType;
    
    [SerializeField] private string _reverbZone = "Main";
    public string ReverbZone => _reverbZone;

    //Events
    [SerializeField] private EventReference _punch;
    public EventReference Punch => _punch;
    public EventReference Reverb => _reverb;
    [SerializeField] private EventReference _reverb;
    
    [SerializeField] private EventReference _braam;
    public EventReference Braam => _braam;

    [SerializeField] private EventReference _ground;
    public EventReference Ground => _ground;
}
