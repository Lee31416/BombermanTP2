using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BreakableBlockScript : NetworkBehaviour
{

    [ClientRpc]
    public void RpcDestroyBlock()
    {
        Destroy(gameObject);
        NetworkServer.Destroy(gameObject);
    }
}
