using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class BreakableBlockScript : NetworkBehaviour
{

    public void DestroyBlock()
    {
        Destroy(gameObject);
        NetworkServer.Destroy(gameObject);
    }
}
