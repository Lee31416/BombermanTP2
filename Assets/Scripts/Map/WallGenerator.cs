using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{
    public class WallGenerator : MonoBehaviour
    {
        [SerializeField] private Tile wall;
        [SerializeField] private GridScript _grid;
    
        private void Start()
        {
            var tilemap = GetComponent<Tilemap>();
        
            GenerateOutsideWalls(tilemap);

            for (var x = 0; x < _grid.mapSize; x++)
            {
                for (var y = 0; y < _grid.mapSize; y++)
                {
                    GenerateInsideWall(tilemap, x, y);
                }
            }
        }

        private void GenerateInsideWall(Tilemap tilemap, int x, int y)
        {
            var position = new Vector3Int(x,y,0);

            if (x == 0 || y == 0 && x == _grid.mapSize || y == _grid.mapSize)
            {
                return;
            }

            if (x % 2 == 1 && y % 2 == 1)
            {
                tilemap.SetTile(position, wall);
                _grid.grid[x, y] = "[W]";
            }
        
        }

        private void GenerateOutsideWalls(Tilemap tilemap)
        {
            var tileCount = _grid.mapSize + 2;

            for (var x = 0; x < tileCount; x++)
            {
                for (var y = 0; y < tileCount; y++)
                {
                    if ((x == 0 || y == 0) || (x == _grid.mapSize + 1 || y == _grid.mapSize + 1))
                    {
                        var position = new Vector3Int(x - 1, y - 1, 0);
                        tilemap.SetTile(position, wall);
                    }
                }
            }
        }
    }
}
