using System;
using UnityEngine;
using FMODUnity;

public class FMODAudioManager : MonoBehaviour
{
    private static FMODAudioManager _instance;

    [Obsolete("Obsolete")]
    public static FMODAudioManager Instance
    {
        get
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
    // [Range(1, 5)] private int _checkpointNumber;
    // public int CheckpointNumber
    // {
    //     get { return _checkpointNumber; }
    // }

    //Intro
    [SerializeField] private EventReference _braam;
    public EventReference Braam
    {
        get { return _braam; }
    }

    //LetterChoice
    [SerializeField] private EventReference _wind;
    public EventReference Wind
    {
        get { return _wind; }
    }
}
