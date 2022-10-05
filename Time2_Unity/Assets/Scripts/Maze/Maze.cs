public class Maze {

    private readonly int _width;
    private readonly int _height;

    private bool[,] _grid;

    private readonly System.Random _rg;

    private int _startX;
    private int _startY;

    public bool[,] Grid => _grid;

    public Maze(int width, int height, System.Random rg) {
        this._width = width;
        this._height = height;

        this._rg = rg;
    }

    public void Generate() {
        _grid = new bool[_width, _height];

        _startX = 1;
        _startY = 1;

        _grid[_startX, _startY] = true;

        MazeDigger(_startX, _startY);
    }

    void MazeDigger(int x, int y) {
        int[] directions = new int[] { 1, 2, 3, 4 };

        Tools.Shuffle(directions, _rg);

        for(int i = 0; i < directions.Length; i++) {
            if(directions[i] == 1) {
                if(y - 2 <= 0)
                    continue;

                if(_grid[x, y - 2] == false) {
                    _grid[x, y - 2] = true;
                    _grid[x, y - 1] = true;

                    MazeDigger(x, y - 2);
                }
            }

            if(directions[i] == 2) {
                if(x - 2 <= 0)
                    continue;

                if(_grid[x - 2, y] == false) {
                    _grid[x - 2, y] = true;
                    _grid[x - 1, y] = true;

                    MazeDigger(x - 2, y);
                }
            }

            if(directions[i] == 3) {
                if(x + 2 >= _width - 1)
                    continue;

                if(_grid[x + 2, y] == false) {
                    _grid[x + 2, y] = true;
                    _grid[x + 1, y] = true;

                    MazeDigger(x + 2, y);
                }
            }

            if(directions[i] == 4) {
                if(y + 2 >= _height - 1)
                    continue;

                if(_grid[x, y + 2] == false) {
                    _grid[x, y + 2] = true;
                    _grid[x, y + 1] = true;

                    MazeDigger(x, y + 2);
                }
            }
        }
    }
    
}