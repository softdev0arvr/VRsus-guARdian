using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PraticeArea : NetworkBehaviour
{
    public Transform spawnPos;
    public GameObject[] planes;
    private EnvironmentCreation env;
    private PracticeEntrance entrance;
    private SampleRelic[] relics;
    // Use this for initialization

    void Start()
    {
        entrance = GetComponentInChildren<PracticeEntrance>();
        relics = GetComponentsInChildren<SampleRelic>();
        if (!isServer)
        {
            List<Vector3> list = Utility.CombinePolygons(Utility.CreateVerticesFromPlane(planes[0]),
                Utility.CreateVerticesFromPlane(planes[1]), 0.005f);
            for (int i = 2; i < planes.Length; i++)
            {
                list = Utility.CombinePolygons(list, Utility.CreateVerticesFromPlane(planes[i]), 0.005f);
            }

            for (int i = 0; i < list.Count; i++)
            {
                Vector3 vec = list[i];
                vec.y = 0f;// transform.position.y;
                list[i] = vec;
            }

            env = GetComponent<EnvironmentCreation>();
            env.boundary = list;
            env.CreateTerrain();
        }
    }

    [ClientRpc]
    public void RpcActivateEntrance(Vector3 spawnPos)
    {
        if (isServer) return;
        entrance.Activate(spawnPos, this);
    }

    public void ResetPracticeArea()
    {
        if (isServer) return;
        entrance.Deactivate();

        foreach(SampleRelic r in relics)
        {
            r.ResetPractice();
        }
    }
}
