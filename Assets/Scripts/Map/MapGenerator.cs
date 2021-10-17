using System.Collections;
using System.Collections.Generic;
using Map;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Tile ground1;
    [SerializeField] private Tile ground2;
    private GridScript _grid;

    private void Start()
    {
        _grid = GetComponentInParent<GridScript>();
    }

    public void GenerateMap()
    {
        var tilemap = GetComponent<Tilemap>();

        for (var x = 0; x < _grid.mapSize; x++)
        {
            for (var y = 0; y < _grid.mapSize; y++)
            {
                var position = new Vector3Int(x,y,0);
                var odd = (x + y) % 2 == 1;
                var tile = odd ? ground1 : ground2;
                tilemap.SetTile(position, tile);
            }
        }
    }
}
