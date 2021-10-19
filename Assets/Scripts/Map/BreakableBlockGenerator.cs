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
        private GridScript _grid;
        private Random _random = new Random();
        
        private void SetCornerBoundaries()
        {
            //TODO Refactor this shit
            var max = _grid.mapSize - 1;
            
            _grid.grid[0, 0] = "[X]";
            _grid.grid[0, 1] = "[X]";
            _grid.grid[1, 0] = "[X]";
            
            _grid.grid[0, max] = "[X]";
            _grid.grid[0, max-1] = "[X]";
            _grid.grid[1, max] = "[X]";
            
            _grid.grid[max-1, max] = "[X]";
            _grid.grid[max, max] = "[X]";
            _grid.grid[max, max-1] = "[X]";
            
            _grid.grid[max-1, 0] = "[X]";
            _grid.grid[max, 0] = "[X]";
            _grid.grid[max, 1] = "[X]";
        }

        public void GenerateBreakableBlocks(GridScript grid)
        {
            _grid = grid;
            SetCornerBoundaries();
            
            for (var i = 0; i < _grid.mapSize; i++)
            {
                for (var j = 0; j < _grid.mapSize; j++)
                {
                    var randomNb = _random.Next(1, 3);
                    
                    if (randomNb % 2 == 0)
                    {
                        GenerateBreakableBlock(i, j);
                    }
                }
            }
        }

        private void GenerateBreakableBlock(int x, int y)
        {
            var position = new Vector3Int(x,y,0);

            if (_grid.grid[x, y] == "[W]" || _grid.grid[x, y] == "[X]")
            {
                return;
            }

            _grid.grid[x, y] = "[B]";
            _tilemap.SetTile(position, tile);
        }
    }
}
