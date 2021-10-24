using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Player;
using UnityEngine;

public class ItemScript : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerControl>();
        switch (name)
        {
            case "FireItem(Clone)":
                if (player.firepowerCount <= 9)
                {
                    player.firepowerCount++;
                }
                break;
            case "BombItem(Clone)":
                if (player.bombCount <= 9)
                {
                    player.bombCount++;
                }
                break;
            case "RollerbladeItem(Clone)":
                if (player.rollerbladeCount <= 9)
                {
                    player.rollerbladeCount++;
                }
                break;
        }
        
        Destroy(gameObject);
        NetworkServer.Destroy(gameObject);
    }
}
