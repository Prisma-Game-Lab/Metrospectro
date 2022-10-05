using System;
using UnityEngine;

public class MazeSpawner : MonoBehaviour
{

    [SerializeField] private int mazeWidth;
    [SerializeField] private int mazeHeight;
    [SerializeField] private string mazeSeed;
    
    [SerializeField] private GameObject wallPrefab;


    private System.Random _mazeRg;

    private Maze _maze;

    public bool IsCellBlocked(int x, int z) {
        if(x >= mazeWidth || x < 0 || z >= mazeHeight || z < 0) {
            return false;
        }
        return !_maze.Grid[x, z];
    }
    
    private void Start() {
        _mazeRg = new System.Random(mazeSeed.GetHashCode());

        if(mazeWidth % 2 == 0)
            mazeWidth++;

        if(mazeHeight % 2 == 0) {
            mazeHeight++;
        }

        _maze = new Maze(mazeWidth, mazeHeight, _mazeRg);
        _maze.Generate();
        DrawMaze();
    }

    private void DrawMaze() {
        for(int x = 0; x < mazeWidth; x++) {
            for(int z = 0; z < mazeHeight; z++) {
                Vector3 position = new Vector3(x, 0, z);

                if(!_maze.Grid[x,z])
                {
                    CreateWall(position, wallPrefab, transform);
                }
            }
        }
    }


    private void CreateWall(Vector3 position, GameObject prefab, Transform parent) {
        var wall = Instantiate(prefab, position, Quaternion.identity, parent);
    }
}
