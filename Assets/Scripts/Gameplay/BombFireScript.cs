using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class BombFireScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        print("Fire Trigger: " + other);
        var player = other.GetComponent<PlayerControl>();
        if (player == null) return;
        player.Kill();
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
