using System;
using UnityEngine;
using FMODUnity;

public class FMODAudioManager : MonoBehaviour
{
    private static FMODAudioManager _instance;
    private static readonly object _lock = new object();

    public static FMODAudioManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = FindObjectOfType<FMODAudioManager>();
                        if (_instance == null)
                        {
                            GameObject singleton = new GameObject(typeof(FMODAudioManager).ToString());
                            _instance = singleton.AddComponent<FMODAudioManager>();
                            DontDestroyOnLoad(singleton);
                        }
                    }
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    //Parameters
    [SerializeField, Range(0, 2)] private int _surfaceType = 1;
    public int SurfaceType => _surfaceType;
    
    //Events
    [SerializeField] private EventReference _braam;
    public EventReference Braam => _braam;

    [SerializeField] private EventReference _ground;
    public EventReference Ground => _ground;
}
