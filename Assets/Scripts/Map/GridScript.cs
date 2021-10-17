using System.Collections.Generic;
using System.Numerics;
using Mirror;
using UnityEngine;
using UnityEngine.Tilemaps;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Map
{
    public class GridScript : NetworkBehaviour
    {
        public int mapSize;
        public int itemCount;
        public Tile destroyableTile;
        public Tile wallTile;
        public Tilemap _wallTilemap;
        public string[,] grid;

        [SerializeField] private GameObject _fireUp;
        [SerializeField] private GameObject _fireDown;
        [SerializeField] private GameObject _fireLeft;
        [SerializeField] private GameObject _fireRight;
        [SerializeField] private GameObject _fireEndUp;
        [SerializeField] private GameObject _fireEndDown;
        [SerializeField] private GameObject _fireEndLeft;
        [SerializeField] private GameObject _fireEndRight;
        
        private Dictionary<string, GameObject> fireEnds = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> fireExtensions = new Dictionary<string, GameObject>();
        private WallGenerator _wallGenerator;
        private MapGenerator mapGenerator;
        private ItemGeneratorScript itemGenerator;
        private BreakableBlockGenerator breakableBlock;

        private void Start()
        {
            grid = new string[mapSize, mapSize];

            /*mapGenerator = GetComponentInChildren<MapGenerator>();
            _wallGenerator = GetComponentInChildren<WallGenerator>();
            itemGenerator = GetComponentInChildren<ItemGeneratorScript>();
            breakableBlock = GetComponentInChildren<BreakableBlockGenerator>();
            
            mapGenerator.GenerateMap();
            _wallGenerator.GenerateWalls();
            itemGenerator.GenerateItemAtRandom();
            breakableBlock.GenerateBreakableBlocks();*/
            
            FillFireObjects();
            InitArray();
        }

        private void InitArray()
        {
            for (var i = 0; i < mapSize; i++)
            {
                for (var j = 0; j < mapSize; j++)
                {
                    grid[i, j] = "[-]";
                }
            }
        }

        private void FillFireObjects()
        {
            fireEnds.Add("Up", _fireEndUp);
            fireEnds.Add("Down", _fireEndDown);
            fireEnds.Add("Left", _fireEndLeft);
            fireEnds.Add("Right", _fireEndRight);
            fireExtensions.Add("Up", _fireUp);        
            fireExtensions.Add("Down", _fireDown);      
            fireExtensions.Add("Left", _fireLeft);      
            fireExtensions.Add("Right", _fireRight); 
        }

        public void PrintGrid()
        {
            var gridLayout = "";
            
            for (var i = 0; i < mapSize; i++)
            {
                for (var j = 0; j < mapSize; j++)
                {
                    gridLayout += grid[j, i];
                }
                
                gridLayout += '\n';
            }
            
            print(gridLayout);
        }

        public void DestroyTiles(Vector3 pos, int firepower)
        {
            var currentCell = _wallTilemap.WorldToCell(pos);
            
            DestroyTile(currentCell);
            DetectWallsBeforeExplosion(pos, firepower, "Up");
            DetectWallsBeforeExplosion(pos, firepower, "Down");
            DetectWallsBeforeExplosion(pos, firepower, "Left");
            DetectWallsBeforeExplosion(pos, firepower, "Right");
        }

        private void DetectWallsBeforeExplosion(Vector3 pos, int firepower, string direction)
        {
            var currentCell = _wallTilemap.WorldToCell(pos);
            
            for (var i = 1; i <= firepower; i++)
            {
                var position = 0.15f * i;
                var worldCellPosition = new Vector3(0, 0, 0);
                var tilemapCellPosition = new Vector3Int(0, 0, 0);

                switch (direction)
                {
                    case "Up":
                        worldCellPosition = pos + new Vector3(0, position);
                        tilemapCellPosition = currentCell + new Vector3Int(0, i, 0);
                        break;
                    case "Down":
                        worldCellPosition = pos + new Vector3(0, -position);
                        tilemapCellPosition = currentCell + new Vector3Int(0, -i, 0);
                        break;
                    case "Left":
                        worldCellPosition = pos + new Vector3(-position, 0);
                        tilemapCellPosition = currentCell + new Vector3Int(-i, 0, 0);
                        break;
                    case "Right":
                        worldCellPosition = pos + new Vector3(position, 0);
                        tilemapCellPosition = currentCell + new Vector3Int(i, 0, 0);
                        break;
                }

                if (i == firepower)
                {
                    if (!CreateFire(worldCellPosition, fireEnds[direction], tilemapCellPosition))
                    {
                        break;
                    }
                }
                else
                {
                    if (!CreateFire(worldCellPosition, fireExtensions[direction], tilemapCellPosition))
                    { 
                        break;
                    }
                }
                
                if (!DestroyTile(tilemapCellPosition))
                {
                    break;
                }
            }
        }

        private bool DestroyTile(Vector3Int cell)
        {
            var tile = _wallTilemap.GetTile<Tile>(cell);

            if (tile == wallTile)
            {
                return false;
            }
            
            if (tile == destroyableTile)
            {
                _wallTilemap.SetTile(cell, null);
            }

            return true;
        }

        private bool CreateFire(Vector3 worldCell, GameObject fireObject, Vector3Int tilemapCell)
        {
            var tile = _wallTilemap.GetTile<Tile>(tilemapCell);
            
            if (tile == wallTile)
            {
                return false;
            }

            var fireInstance = Instantiate(fireObject, worldCell, Quaternion.identity);
            OnCreateFireCommand(fireInstance);
            return true;
        }

        [Command]
        private void OnCreateFireCommand(GameObject fireInstance)
        {
            NetworkServer.Spawn(fireInstance);
        }
        
    }
}
