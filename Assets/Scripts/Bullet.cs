﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (!isServer)
            return;

        CameraAvatar avatar = collision.gameObject.GetComponent<CameraAvatar>();
        Combat combat = collision.gameObject.GetComponent<Combat>();

        if (avatar)
        {
            avatar.rootPlayer.TakeDamage();
            Destroy(gameObject);
        } else if (combat)
        {
            combat.TakeDamage();
            Destroy(gameObject);
        }

        ////gets Combat from the colliding gameObject
        //Combat combat = collision.gameObject.GetComponent<Combat>();

        ////if it's the AR object try the gameobject from root
        //if (!combat)
        //{
        //    //get root
        //    CameraAvatar arChar = collision.gameObject.GetComponent<CameraAvatar>();
        //    if (arChar)
        //        combat = arChar.rootPlayer.GetComponent<Combat>();
        //}

        ////if there's combat
        //if (combat)
        //{
        //    combat.TakeDamage();
        //    Destroy(gameObject);
        //}
    }
}
