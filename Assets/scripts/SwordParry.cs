using System;
using UnityEngine;

public class SwordParry : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SwordSwing ghj = GetComponent<SwordSwing>();
        ghj.OnStartHit += OnStartHit_Performed;   
    }

    private void OnStartHit_Performed(object sender, EventArgs e)
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
