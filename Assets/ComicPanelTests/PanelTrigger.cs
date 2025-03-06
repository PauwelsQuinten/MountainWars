using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PanelTrigger : MonoBehaviour
{
    public EventHandler TriggerEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag != "Player") return;
        TriggerEnter.Invoke(this.gameObject, EventArgs.Empty);
    }
}
