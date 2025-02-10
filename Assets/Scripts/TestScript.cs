using UnityEngine;
using FMODUnity;
public class TestScript : MonoBehaviour
{
    [SerializeField] private EventReference braamSound;
    void Start()
    {
        AudioPlayer.instance.PlayOneShot(braamSound, transform.position);
    }

}
