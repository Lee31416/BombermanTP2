using UnityEngine;
using UnityEngine.Tilemaps;

namespace Map
{
    public class WallGenerator : MonoBehaviour
    {
        [SerializeField] private Tile wall;

        public void GenerateWalls()
        {
            var tilemap = GetComponent<Tilemap>();
            var grid = transform.parent.GetComponent<GridScript>();
        
            GenerateOutsideWalls(grid, tilemap);

            for (var x = 0; x < grid.mapSize; x++)
            {
                for (var y = 0; y < grid.mapSize; y++)
                {
                    GenerateInsideWall(grid, tilemap, x, y);
                }
            }
        }

        private void GenerateInsideWall(GridScript grid, Tilemap tilemap, int x, int y)
        {
            var position = new Vector3Int(x,y,0);

            if (x == 0 || y == 0 && x == grid.mapSize || y == grid.mapSize)
            {
                return;
            }

            if (x % 2 != 1 || y % 2 != 1) return;
            tilemap.SetTile(position, wall);
            grid.grid[x, y] = "[W]";
        }

        private void GenerateOutsideWalls(GridScript grid, Tilemap tilemap)
        {
            var tileCount = grid.mapSize + 2;

            for (var x = 0; x < tileCount; x++)
            {
                for (var y = 0; y < tileCount; y++)
                {
                    if ((x != 0 && y != 0) && (x != grid.mapSize + 1 && y != grid.mapSize + 1)) continue;
                    var position = new Vector3Int(x - 1, y - 1, 0);
                    tilemap.SetTile(position, wall);
                }
            }
        }
    }
}
