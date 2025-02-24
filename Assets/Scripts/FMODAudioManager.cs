using System;
using UnityEngine;
using FMODUnity;

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
    
    //Events
    [SerializeField] private EventReference _braam;
    public EventReference Braam => _braam;

    [SerializeField] private EventReference _ground;
    public EventReference Ground => _ground;
}
