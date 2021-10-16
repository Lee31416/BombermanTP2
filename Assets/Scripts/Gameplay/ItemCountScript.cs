using System.Collections;
using System.Collections.Generic;
using Player;
using TMPro;
using UnityEngine;

public class ItemCountScript : MonoBehaviour
{
    [SerializeField] private PlayerControl _player;
    [SerializeField] private TextMeshProUGUI _text;

    void Update()
    {
        if (name == "FireCount")
        {
            _text.text = _player.firepowerCount.ToString();
        }
        if (name == "RollerbladeCount")
        {
            _text.text = _player.rollerbladeCount.ToString();
        }
        if (name == "BombCount")
        {
            _text.text = _player.bombCount.ToString();
        }
    }
}
