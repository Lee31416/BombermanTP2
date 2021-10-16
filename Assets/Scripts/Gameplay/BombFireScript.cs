using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class BombFireScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        print("Fire Trigger: " + other.name);

        if (other.name == "Player")
        {
            var player = other.GetComponent<PlayerControl>();
            player.Kill();
        }
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
}
