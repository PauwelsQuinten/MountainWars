using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelTrigger : MonoBehaviour
{
    public EventHandler TriggerEnter;
    public EventHandler TriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        TriggerEnter.Invoke(this.gameObject, EventArgs.Empty);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        TriggerExit.Invoke(this.gameObject, EventArgs.Empty);
    }
}
