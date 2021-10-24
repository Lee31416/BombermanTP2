using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

namespace Map
{
    public class BreakableBlockGenerator : MonoBehaviour
    {
        /*[SerializeField] private Tilemap _tilemap;
        [SerializeField] private Tile tile;*/
        [SerializeField] private GameObject _breakableBlockPrefab;
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
                    if (randomNb % 2 != 0) continue;
                    if (grid.grid[i, j] == "[W]" || grid.grid[i, j] == "[X]") continue;
                    
                    var x = i * 0.16f;
                    var y = j * 0.16f;
                    GenerateBreakableBlock(grid, x, y);
                    grid.grid[i, j] = "[B]";
                }
            }
        }

        private void GenerateBreakableBlock(GridScript grid, float x, float y)
        {
            var instance = Instantiate(_breakableBlockPrefab, new Vector3(x, y, 0), Quaternion.identity);
            NetworkServer.Spawn(instance);
        }
    }
}
