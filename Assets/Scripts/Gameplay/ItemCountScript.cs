using System.Collections;
using System.Collections.Generic;
using Player;
using TMPro;
using UnityEngine;

public class ItemCountScript : MonoBehaviour
{
    private PlayerControl _player;
    public PlayerControl target
    {
        get => _player;
        set => _player = value;
    }
    
    [SerializeField] private TextMeshProUGUI _text;

    private void FixedUpdate()
    {
        if (_player == null) return;

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
