﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SlowTrap : TrapDefense
{
    [SyncVar]
    bool isActive = false;

    public override string TrapName
    {
        get
        {
            return "Slow Trap";
        }
    }

    private void Start()
    {
#if !UNITY_IOS
        GetComponent<Renderer>().enabled = false;
        foreach(Collider c in GetComponents<Collider>())
        {
            if (!c.isTrigger)
            {
                c.enabled = false;
            }
        }
#endif
    }

    public override void ToggleSelected()
    {
        base.ToggleSelected();

        Debug.Log("Slow Trap toggled");


        Renderer childRenderer = GetComponentInChildren<Renderer>();
        if (childRenderer)
            childRenderer.enabled = selected;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer) return;

        if (other.tag == "Player")
        {
            Debug.Log("Player enterred trigger area");

            if (!isActive)
                CmdTriggerTrap();

            if (isActive)
            {
                other.GetComponent<Movement>().IsSlowed = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isServer) return;

        if (other.tag == "Player" && isActive)
        {
            other.GetComponent<Movement>().IsSlowed = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isServer) return;

        if (collision.gameObject.tag == "Player" && !isActive) 
            CmdTriggerTrap();
    }

    [Command]
    private void CmdTriggerTrap()
    {
        isActive = true;
        GetComponentInChildren<Renderer>().enabled = true;
    }
}