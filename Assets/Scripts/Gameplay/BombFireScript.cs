using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Player;
using UnityEngine;

public class BombFireScript : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isServer) return;
        
        var player = other.GetComponent<PlayerControl>();
        if (player == null) return;
        player.Kill();
    }
}
