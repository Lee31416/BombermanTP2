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

        
        if (other.GetComponent<PlayerControl>() != null)
        {
            var player = other.GetComponent<PlayerControl>();
            player.Kill();
        }
        
        if (other.GetComponent<BreakableBlockScript>() != null)
        {
            var breakableBlock = other.GetComponent<BreakableBlockScript>();
            breakableBlock.RpcDestroyBlock();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdDestroyFire()
    {
        RpcDestroyFire();
    }
    
    [ClientRpc]
    private void RpcDestroyFire()
    {
        NetworkServer.Destroy(gameObject);
    }
    
    
}
