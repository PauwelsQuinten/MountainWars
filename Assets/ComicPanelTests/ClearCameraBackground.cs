using UnityEngine;

public class ClearCameraBackground : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<Camera>().backgroundColor = new Color(0, 0, 0, 0);
    }
}
