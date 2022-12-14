using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

/// <summary>
/// Handles player collecting relic and related stuff
/// 
/// Author: Tanat Boozayaangool
/// </summary>
public class Relic : NetworkBehaviour
{
    public Renderer relicRenderer;
    private bool hasBeenStolen = false;
    private List<Entrance> entrances;

    public Light spotLight;
    public ParticleSystem[] particles;

    public override void OnStartServer()
    {
        entrances = new List<Entrance>();

#if UNITY_IOS
        foreach (ParticleSystem p in particles)
        {
            if (p.lights.enabled)
            {
                ParticleSystem.LightsModule lights = p.lights;
                lights.enabled = false;
            }
        }
        spotLight.enabled = false;
#endif
    }

    public void AddEntrance(Entrance entrance)
    {
        entrances.Add(entrance);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!isServer || hasBeenStolen)
            return;

        if (other.gameObject.tag == "Player")
        {
            VRCombat combat = other.GetComponent<VRCombat>();
            if (!combat)
                combat = other.transform.parent.GetComponent<VRCombat>();
            //if (!combat.IsInvulnerable)
            //{
                hasBeenStolen = true;
                combat.GainRelic();
                RpcStealRelic();
            //}
        }
    }

    [ClientRpc]
    private void RpcStealRelic()
    {
        if (relicRenderer)
            relicRenderer.enabled = false;


        if (entrances != null)
        {
            foreach (Entrance e in entrances)
            {
                e.RpcActivate();
            }
        }

        foreach (ParticleSystem p in particles)
            p.Stop();

        spotLight.enabled = false;

        if (isServer)
            CanvasManager.Instance.SetMessage("A relic was stolen!");
        else CanvasManager.Instance.SetMessage("Stole a relic!");
    }
}