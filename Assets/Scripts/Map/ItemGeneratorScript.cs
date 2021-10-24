using Mirror;
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
        
        public void GenerateItemAtRandom()
        {
            var grid = transform.parent.GetComponent<GridScript>();
            HandleRandomItemGeneration(grid, "Fire");
            HandleRandomItemGeneration(grid, "Bomb");
            HandleRandomItemGeneration(grid, "Rollerblade");
        }

        private void HandleRandomItemGeneration(GridScript grid, string type)
        {
            for (var i = 0; i < grid.itemCount / 3;)
            {
                var gridX =  _random.Next(0, grid.mapSize);
                var gridY = _random.Next(0, grid.mapSize);

                if (grid.grid[gridX, gridY] == "[W]" || (gridX == 0 && gridY == 0) || grid.grid[gridX, gridY] == "[I]")
                {
                    continue;
                }
                
                var x = gridX * 0.16f;
                var y = gridY * 0.16f;

                grid.grid[gridX, gridY] = "[I]";

                switch (type)
                {
                    case "Bomb":
                        GenerateBomb(x, y);
                        break;
                    case "Fire":
                        GenerateFire(x, y);
                        break;
                    case "Rollerblade":
                        GenerateRollerblade(x, y);
                        break;
                }

                i++;
            }
        }

        private void GenerateFire(float x, float y)
        {
            var instance = Instantiate(fire, new Vector3(x, y, 0), Quaternion.identity);
            NetworkServer.Spawn(instance);
        }
        
        private void GenerateRollerblade(float x, float y)
        {
            var instance = Instantiate(rollerblade, new Vector3(x, y, 0), Quaternion.identity);
            NetworkServer.Spawn(instance);
        }
        
        private void GenerateBomb(float x, float y)
        {
            var instance = Instantiate(bomb, new Vector3(x, y, 0), Quaternion.identity);
            NetworkServer.Spawn(instance);
        }
        
    }
}
