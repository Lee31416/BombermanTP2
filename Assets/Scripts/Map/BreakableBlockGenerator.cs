using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

namespace Map
{
    public class BreakableBlockGenerator : MonoBehaviour
    {
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private Tile tile;

        private Random _random = new Random();
        
        private void SetCornerBoundaries(GridScript grid)
        {
            //TODO Refactor this shit
            var max = grid.mapSize - 1;
            
            grid.grid[0, 0] = "[X]";
            grid.grid[0, 1] = "[X]";
            grid.grid[1, 0] = "[X]";
            
            grid.grid[0, max] = "[X]";
            grid.grid[0, max-1] = "[X]";
            grid.grid[1, max] = "[X]";
            
            grid.grid[max-1, max] = "[X]";
            grid.grid[max, max] = "[X]";
            grid.grid[max, max-1] = "[X]";
            
            grid.grid[max-1, 0] = "[X]";
            grid.grid[max, 0] = "[X]";
            grid.grid[max, 1] = "[X]";
        }

        public void GenerateBreakableBlocks()
        {
            var grid = transform.parent.GetComponent<GridScript>();
            SetCornerBoundaries(grid);
            
            for (var i = 0; i < grid.mapSize; i++)
            {
                for (var j = 0; j < grid.mapSize; j++)
                {
                    var randomNb = _random.Next(1, 3);
                    
                    if (randomNb % 2 == 0)
                    {
                        GenerateBreakableBlock(grid, i, j);
                    }
                }
            }
        }

        private void GenerateBreakableBlock(GridScript grid, int x, int y)
        {            
            if (grid.grid[x, y] == "[W]" || grid.grid[x, y] == "[X]")
            {
                return;
            }

            grid.grid[x, y] = "[B]";
            var position = new Vector3Int(x,y,0);
            _tilemap.SetTile(position, tile);
        }
    }
}
