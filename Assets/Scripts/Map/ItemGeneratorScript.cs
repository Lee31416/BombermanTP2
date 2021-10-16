using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

namespace Map
{
    public class ItemGeneratorScript : MonoBehaviour
    {
        private Random _random = new Random();
        [SerializeField] private GameObject fire;
        [SerializeField] private GameObject bomb;
        [SerializeField] private GameObject rollerblade;
        [SerializeField] private Tile fireTile;
        [SerializeField] private Tile bombTile;
        [SerializeField] private Tile rollerTile;
        [SerializeField] private Tilemap _tilemap;
        [SerializeField] private GridScript _grid;
        
        private void Start()
        {
            for (var i = 0; i < _grid.itemCount / 3;)
            {
                var gridX =  _random.Next(0, _grid.mapSize);
                var gridY = _random.Next(0, _grid.mapSize);

                if (_grid.grid[gridX, gridY] == "[W]" || (gridX == 0 && gridY == 0) || _grid.grid[gridX, gridY] == "[I]")
                {
                    continue;
                }
                
                var x = gridX * 0.16f;
                var y = gridY * 0.16f;

                _grid.grid[gridX, gridY] = "[I]";
                GenerateFire(x, y);
                i++;
            }
            
            for (var i = 0; i < _grid.itemCount / 3;)
            {
                var gridX =  _random.Next(0, _grid.mapSize);
                var gridY = _random.Next(0, _grid.mapSize);

                if (_grid.grid[gridX, gridY] == "[W]" || (gridX == 0 && gridY == 0) || _grid.grid[gridX, gridY] == "[I]")
                {
                    continue;
                }
                
                var x = gridX * 0.16f;
                var y = gridY * 0.16f;

                _grid.grid[gridX, gridY] = "[I]";
                GenerateRollerblade(x, y);
                i++;
            }
            
            for (var i = 0; i < _grid.itemCount / 3;)
            {
                var gridX =  _random.Next(0, _grid.mapSize);
                var gridY = _random.Next(0, _grid.mapSize);

                if (_grid.grid[gridX, gridY] == "[W]" || (gridX == 0 && gridY == 0) || _grid.grid[gridX, gridY] == "[I]")
                {
                    continue;
                }
                
                var x = gridX * 0.16f;
                var y = gridY * 0.16f;

                _grid.grid[gridX, gridY] = "[I]";
                GenerateBomb(x, y);
                i++;
            }
        }

        private void GenerateFire(float x, float y)
        {
            Instantiate(fire, new Vector3(x, y, 0), Quaternion.identity);
            /*var position = new Vector3Int(x,y,0);

            if (_grid.grid[x, y] == "[W]" || (x == 0 && y == 0))
            {
                return;
            }

            _grid.grid[x, y] = "[B]";
            _tilemap.SetTile(position, fireTile);*/
        }
        
        private void GenerateRollerblade(float x, float y)
        {
            Instantiate(rollerblade, new Vector3(x, y, 0), Quaternion.identity);
        }
        
        private void GenerateBomb(float x, float y)
        {
            Instantiate(bomb, new Vector3(x, y, 0), Quaternion.identity);
        }
        
    }
}
