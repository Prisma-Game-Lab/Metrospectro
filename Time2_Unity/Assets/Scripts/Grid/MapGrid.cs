using System;
using UnityEngine;

public class MapGrid : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;

    private CellType[,] _grid;
    
    private void Awake()
    {
        _grid = new CellType[gridWidth, gridHeight];
        UpdateGrid();
    }

    private void UpdateGrid()
    {
        for(int i = 0; i<gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                Collider[] results = new Collider[5];
                var amount = Physics.OverlapSphereNonAlloc(new Vector3(i+.5f, .5f, j+.5f), .4f, results);
                if (amount != 0)
                {
                    _grid[i, j] = CellType.Blocked;
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
                        case CellType.Interactable:
                            DrawSquare(i,j,Color.green);
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

public enum CellType { Empty, Blocked, Interactable }
