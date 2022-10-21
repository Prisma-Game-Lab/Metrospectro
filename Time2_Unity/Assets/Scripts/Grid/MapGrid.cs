using System;
using Unity.Mathematics;
using UnityEngine;

public class MapGrid : Singleton<MapGrid>
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;

    private CellType[,] _grid;
    
    private void Awake()
    {
        _grid = new CellType[gridWidth, gridHeight];
        UpdateGrid();
    }

    public void UpdateGrid()
    {
        for(int i = 0; i<gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                var results = new RaycastHit[5];
                var amount = Physics.RaycastNonAlloc(new Vector3(i+.5f, -0.5f, j+.5f),Vector3.up, results, math.INFINITY);
                if (amount != 0)
                {
                    _grid[i, j] = CellType.Blocked;
                }
                else
                {
                    _grid[i, j] = CellType.Empty;

                }
            }
        }
    }

    public CellType this[int index1, int index2]
    {
        get => _grid[index1, index2];
        set => _grid[index1, index2] = value;
    }

    public bool IsCellBlocked(int x, int y) {
        if(x >= gridWidth || x < 0 || y >= gridHeight || y < 0) {
            return true;
        }
        return _grid[x, y] == CellType.Blocked;
    }

    private void OnDrawGizmos()
    {
        for(int i = 0; i<gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                if (_grid == null)
                {
                    DrawSquare(i,j,Color.white);
                }
                else
                {
                    switch (_grid[i, j])
                    {
                        case CellType.Blocked:
                            DrawSquare(i,j,Color.red);
                            break;
                        case CellType.Empty:
                            DrawSquare(i,j,Color.white);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }

    private void DrawSquare(int x,int z, Color color)
    {
        for (float i = 0.2f; i < 1; i += 0.6f)
        {
            Debug.DrawLine(new Vector3(x+0.2f,0, z+i), new Vector3(x+0.8f,0, z+i), color);
        }
        for (float i = 0.2f; i < 1; i += 0.6f)
        {
            Debug.DrawLine(new Vector3(x+i,0, z+0.2f), new Vector3(x+i,0, z+0.8f), color);
        }
    }
}

public enum CellType { Empty, Blocked }
