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

        private void Start()
        {
            _grid = GetComponentInParent<GridScript>();
        }
        
        public void GenerateBreakableBlocks()
        {
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

            if (_grid.grid[x, y] == "[W]" || (x == 0 && y == 0))
            {
                return;
            }

            _grid.grid[x, y] = "[B]";
            _tilemap.SetTile(position, tile);
        }
    }
}
