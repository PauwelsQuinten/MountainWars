using UnityEngine;

public class SetCamToObject : MonoBehaviour
{
    void Start()
    {
        GameObject cam = GameObject.Find("Panel3Cam");
        cam.transform.parent = transform;
        cam.transform.localPosition = Vector3.back;
    }
}
