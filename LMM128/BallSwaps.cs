using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Slider
{

    public static int N;
    public static int C;

    public static bool NextPermutation(int[] array)
    {
        // Find non-increasing suffix
        int i = array.Length - 1;
        while (i > 0 && array[i - 1] >= array[i]) i--;
        if (i <= 0) return false;

        // Find successor to pivot
        int j = array.Length - 1;
        while (array[j] <= array[i - 1]) j--;
        int temp = array[i - 1];
        array[i - 1] = array[j];
        array[j] = temp;

        // Reverse suffix
        j = array.Length - 1;
        while (i < j)
        {
            temp = array[i];
            array[i] = array[j];
            array[j] = temp;
            i++;
            j--;
        }
        return true;
    }

    public static void copyGrid(int[,] grid, int[,] tmpGrid, int[] newColors)
    {
        if(newColors == null)
        {
            newColors = new int[C];
            for (int c = 0; c < C; c++) newColors[c] = c;
        }
        for (int i = 0; i < N; i++) for (int k = 0; k < N; k++) tmpGrid[i, k] = newColors[grid[i, k]];
    }

    public static bool inGrid(int r, int c)
    {
        return (r >= 0 && r < N && c >= 0 && c < N);
    }

    public static bool areNeighbours(int r1, int c1, int r2, int c2)
    {
        return (Math.Abs(r1 - r2) + Math.Abs(c1 - c2) == 1);
    }

    public static int countComponents(int[,] grid)
    {
        int[,] tmpGrid = new int[N, N];
        for (int i = 0; i < N; i++) for (int k = 0; k < N; k++) tmpGrid[i,k] = grid[i,k];

        int count = 0;

        for (int r = 0; r < N; r++)
            for (int c = 0; c < N; c++)
                if (tmpGrid[r,c] != -1)
                {
                    dfs(r, c, tmpGrid[r,c], tmpGrid);
                    count++;
                }

        return count;
    }

    public static void dfs(int r, int c, int color, int[,] grid)
    {
        if (!inGrid(r, c)) return;
        if (grid[r,c] != color) return;

        grid[r,c] = -1;    //set to used
        dfs(r + 1, c, color, grid);
        dfs(r - 1, c, color, grid);
        dfs(r, c + 1, color, grid);
        dfs(r, c - 1, color, grid);
    }

    public static int sortByRow(int[,] grid, List<string> moves)
    {
        int movesCounter = 0;
        for (int row = 0; row < N; row++)
        {
            for (int color = 0; color < N-1; color++)
            {
                for (int col = 0; col < N - 1; col++)
                {
                    if (grid[row, col] > grid[row, col + 1])
                    {
                        int tmp = grid[row, col];
                        grid[row, col] = grid[row, col + 1];
                        grid[row, col + 1] = tmp;
                        moves.Add(row.ToString() + " " + col.ToString() + " " + row.ToString() + " " + (col + 1).ToString());
                        movesCounter++;
                    }
                }
            }
        }
        return movesCounter;
    }

    public static int sortByCol(int[,] grid, List<string> moves)
    {
        int movesCounter = 0;
        for (int col = 0; col < N; col++)
        {
            for (int color = 0; color < N-1; color++)
            {
                for (int row = 0; row < N - 1; row++)
                {
                    if (grid[row, col] > grid[row + 1, col])
                    {
                        int tmp = grid[row, col];
                        grid[row, col] = grid[row + 1, col];
                        grid[row + 1, col] = tmp;
                        moves.Add(row.ToString() + " " + col.ToString() + " " + (row + 1).ToString() + " " + (col).ToString());
                        movesCounter++;
                    }
                }
            }
        }
        return movesCounter;
    }

    public static List<string> getSolutionByRow(int[,] grid)
    {
        var colorCountDict = new Dictionary<int, int>();
        for (int c = 0; c < C; c++) colorCountDict.Add(c, 0);
        for (int r = 0; r < N; r++) for (int c = 0; c < N; c++) colorCountDict[grid[r, c]]++;

        var color = 0;
        var colorCount = colorCountDict[color];

        //Create a solution from scratch
        var newGrid = new int[N, N];
        for(int row=0; row<N; row++)
        {
            if (row % 2 == 0)
            {
                for(int col=0; col<N; col++)
                {
                    newGrid[row, col] = color;
                    colorCount--;
                    if(colorCount == 0)
                    {
                        color++;
                        if (color < C) colorCount = colorCountDict[color];
                    }
                }
            }
            else
            {
                for (int col = N-1; col >= 0; col--)
                {
                    newGrid[row, col] = color;
                    colorCount--;
                    if (colorCount == 0)
                    {
                        color++;
                        if(color < C) colorCount = colorCountDict[color];
                    }
                }
            }

        }

        int currentColor = newGrid[0, 0];
        int previousColor = newGrid[0, 0];

        var moves = new List<string>();
        //Find the moves to match the proposed solution
        var tmpGrid = new int[N, N];
        copyGrid(grid, tmpGrid, null);
        bool end = false;
        for (int row = 0; !end && row < N; row++)
        {
            for (int col = 0; !end && col < N; col++)
            {

                previousColor = currentColor;
                var colorToSearch = newGrid[row, col];
                currentColor = colorToSearch;

                if (currentColor != previousColor)
                {
                    if (countComponents(tmpGrid) == C)
                    {
                        end = true;
                        continue;
                    }
                }


                if (tmpGrid[row,col] != colorToSearch)
                {
                    //Find the closest cell with the selected color
                    var bestDist = int.MaxValue;
                    var bestRow = 0;
                    var bestCol = 0;
                    for (int r = row; r < N; r++)
                    {
                        int c = 0;
                        if(r <= row) c = col + 1;
                        while(c<N)
                        {
                            if(tmpGrid[r, c] == colorToSearch)
                            {
                                var dist = Math.Abs(row - r) + Math.Abs(col - c); 
                                if(dist < bestDist) 
                                {
                                    bestDist = dist;
                                    bestRow = r;
                                    bestCol = c;
                                }
                            }
                            c++;
                        }
                    }

                    //Move the selected cell to the proposed position
                    if (bestCol < col)
                    {
                        for (int c = bestCol; c < col; c++)
                        {
                            int tmp = tmpGrid[bestRow, c];
                            tmpGrid[bestRow, c] = tmpGrid[bestRow, c + 1];
                            tmpGrid[bestRow, c + 1] = tmp;
                            moves.Add(bestRow.ToString() + " " + c.ToString() + " " + (bestRow).ToString() + " " + (c + 1).ToString());
                        }
                    }
                    else
                    {
                        for (int c = bestCol; c > col; c--)
                        {
                            int tmp = tmpGrid[bestRow, c];
                            tmpGrid[bestRow, c] = tmpGrid[bestRow, c - 1];
                            tmpGrid[bestRow, c - 1] = tmp;
                            moves.Add(bestRow.ToString() + " " + c.ToString() + " " + (bestRow).ToString() + " " + (c - 1).ToString());
                        }
                    }

                    for (int r = bestRow; r > row; r--)
                    {
                        int tmp = tmpGrid[r, col];
                        tmpGrid[r, col] = tmpGrid[r - 1, col];
                        tmpGrid[r - 1, col] = tmp;
                        moves.Add(r.ToString() + " " + col.ToString() + " " + (r - 1).ToString() + " " + (col).ToString());
                    }

                }
                
            }
        }

        return moves;
    }

    public static List<string> getSolutionByCol(int[,] grid)
    {
        var colorCountDict = new Dictionary<int, int>();
        for (int c = 0; c < C; c++) colorCountDict.Add(c, 0);
        for (int r = 0; r < N; r++) for (int c = 0; c < N; c++) colorCountDict[grid[r, c]]++;

        var color = 0;
        var colorCount = colorCountDict[color];

        //Create a solution from scratch
        var newGrid = new int[N, N];
        for (int col = 0; col < N; col++)
        {
            if (col % 2 == 0)
            {
                for (int row = 0; row < N; row++)
                {
                    newGrid[row, col] = color;
                    colorCount--;
                    if (colorCount == 0)
                    {
                        color++;
                        if (color < C) colorCount = colorCountDict[color];
                    }
                }
            }
            else
            {
                for (int row = N - 1; row >= 0; row--)
                {
                    newGrid[row, col] = color;
                    colorCount--;
                    if (colorCount == 0)
                    {
                        color++;
                        if (color < C) colorCount = colorCountDict[color];
                    }
                }
            }

        }


        int currentColor = newGrid[0, 0];
        int previousColor = newGrid[0, 0];

        var moves = new List<string>();
        //Find the moves to match the proposed solution
        var tmpGrid = new int[N, N];
        copyGrid(grid, tmpGrid, null);
        bool end = false;
        for (int col = 0; !end && col < N; col++)
        {
            for (int row = 0; !end && row < N; row++)
            {
                previousColor = currentColor;
                var colorToSearch = newGrid[row, col];
                currentColor = colorToSearch;

                if (currentColor != previousColor)
                {
                    if (countComponents(tmpGrid) == C)
                    {
                        end = true;
                        continue;
                    }
                }

                if (tmpGrid[row, col] != colorToSearch)
                {
                    //Find the closest cell with the selected color
                    var bestDist = int.MaxValue;
                    var bestRow = 0;
                    var bestCol = 0;
                    for (int c = col; c < N; c++)
                    {
                        int r = 0;
                        if (c <= col) r = row + 1;
                        while (r < N)
                        {
                            if (tmpGrid[r, c] == colorToSearch)
                            {
                                var dist = Math.Abs(row - r) + Math.Abs(col - c);
                                if (dist < bestDist)
                                {
                                    bestDist = dist;
                                    bestRow = r;
                                    bestCol = c;
                                }
                            }
                            r++;
                        }
                    }

                    //Move the selected cell to the proposed position
                    if (bestRow < row)
                    {
                        for (int r = bestRow; r < row; r++)
                        {
                            int tmp = tmpGrid[r, bestCol];
                            tmpGrid[r, bestCol] = tmpGrid[r + 1, bestCol];
                            tmpGrid[r + 1, bestCol] = tmp;
                            moves.Add(r.ToString() + " " + bestCol.ToString() + " " + (r + 1).ToString() + " " + (bestCol).ToString());
                        }
                    }
                    else
                    {
                        for (int r = bestRow; r > row; r--)
                        {
                            int tmp = tmpGrid[r, bestCol];
                            tmpGrid[r, bestCol] = tmpGrid[r - 1, bestCol];
                            tmpGrid[r - 1, bestCol] = tmp;
                            moves.Add(r.ToString() + " " + bestCol.ToString() + " " + (r - 1).ToString() + " " + (bestCol).ToString());
                        }
                    }

                    for (int c = bestCol; c > col; c--)
                    {
                        int tmp = tmpGrid[row, c];
                        tmpGrid[row, c] = tmpGrid[row, c - 1];
                        tmpGrid[row, c - 1] = tmp;
                        moves.Add(row.ToString() + " " + c.ToString() + " " + (row).ToString() + " " + (c - 1).ToString());
                    }

                }

            }
        }

        return moves;
    }

    public static List<string> tryPermutationsByRow(int[,] grid, DateTime startTime, int msToStop)
    {
        int[,] tmpGrid = new int[N, N];

        int bestScore = int.MaxValue;
        var bestMoves = new List<string>();

        int[] permutations = new int[C];
        for (int i = 0; i < C; i++) permutations[i] = i;

        while (NextPermutation(permutations))
        {
            copyGrid(grid, tmpGrid, permutations);
            List<string> moves = new List<string>();

            sortByCol(tmpGrid, moves);
            if (countComponents(grid) > C)
            {
                var movesToAppend = getSolutionByRow(tmpGrid);
                moves.AddRange(movesToAppend);
            }

            if(moves.Count < bestScore)
            {
                bestScore = moves.Count;
                bestMoves = moves;
            }

            var endTime = DateTime.Now;
            Double elapsedMillisecs = ((TimeSpan)(endTime - startTime)).TotalMilliseconds;
            if (elapsedMillisecs > msToStop) break;
        }

        return bestMoves;
    }

    public static List<string> tryPermutationsByCol(int[,] grid, DateTime startTime, int msToStop)
    {
        int[,] tmpGrid = new int[N, N];

        int bestScore = int.MaxValue;
        var bestMoves = new List<string>();

        int[] permutations = new int[C];
        for (int i = 0; i < C; i++) permutations[i] = i;

        while (NextPermutation(permutations))
        {
            copyGrid(grid, tmpGrid, permutations);
            List<string> moves = new List<string>();

            sortByRow(tmpGrid, moves);
            if (countComponents(grid) > C)
            {
                var movesToAppend = getSolutionByCol(tmpGrid);
                moves.AddRange(movesToAppend);
            }

            if (moves.Count < bestScore)
            {
                bestScore = moves.Count;
                bestMoves = moves;
            }

            var endTime = DateTime.Now;
            Double elapsedMillisecs = ((TimeSpan)(endTime - startTime)).TotalMilliseconds;
            if (elapsedMillisecs > msToStop) break;
        }

        return bestMoves;
    }

    public class Node
    {
        public int row;
        public int col;
        public Node prev = null;
        public Node(int row, int col)
        {
            this.row = row;
            this.col = col;
        }

        public Node(int row, int col, Node prev)
        {
            this.row = row;
            this.col = col;
            this.prev = prev;
        }
    }

    public static List<Node> ShortestPath(int startRow, int startCol, int color, int[,] grid, bool[,] fixedCell)
    {
        var startTime = DateTime.Now;
        
        bool goalExists = false;
        for(int r=0; !goalExists && r < N; r++)for(int c=0; !goalExists && c <N; c++)if (!fixedCell[r, c] && grid[r, c] == color) goalExists = true;
        if (!goalExists)return new List<Node>();

        var visited = new bool[N, N];
        var queue = new Queue<Node>();
        queue.Enqueue(new Node(startRow, startCol));

        while(queue.Count > 0)
        {
            var node = queue.Dequeue();
            visited[node.row, node.col] = true;

            if (node.prev != null && grid[node.row, node.col] == color)
            {
                var path = new List<Node>();
                path.Add(node);
                while (node.prev != null)
                {
                    path.Add(node.prev);
                    node = node.prev;
                }
                //path.Reverse();
                return path;
            }

            var dr = new int[] { 1, -1, 0,  0 };
            var dc = new int[] { 0,  0, 1, -1 };
            for(int i=0; i<4; i++)
            {
                var newRow = node.row + dr[i];
                var newCol = node.col + dc[i];
                if (inGrid(newRow,newCol) && !visited[newRow, newCol] && !fixedCell[newRow, newCol])
                    queue.Enqueue(new Node(newRow, newCol, node));
            }

            var endTime = DateTime.Now;
            Double elapsedMillisecs = ((TimeSpan)(endTime - startTime)).TotalMilliseconds;
            if (elapsedMillisecs > 1000) return null;
        }

        return new List<Node>();

    }

    public static List<string> spiralSolution(int[,] grid, int startingColor)
    {
        var moves = new List<string>();

        var fixedCell = new bool[N, N];

        //start at center cell
        int row = N / 2;
        int col = N / 2;

        //set the starting color
        if (startingColor != grid[row, col])
        {
            var path = ShortestPath(row, col, startingColor, grid, fixedCell);
            if (path == null) return null;
            for (int i = 0; i < path.Count - 1; i++)
            {
                moves.Add(path[i].row.ToString() + " " + path[i].col.ToString() + " " + path[i + 1].row.ToString() + " " + path[i + 1].col.ToString());
                int tmp = grid[path[i].row, path[i].col];
                grid[path[i].row, path[i].col] = grid[path[i + 1].row, path[i + 1].col];
                grid[path[i + 1].row, path[i + 1].col] = tmp;
            }
        }

        //walk in spiral
        int d = 0;
        var dr = new int[] { -1, 0, 1, 0 };
        var dc = new int[] { 0, -1, 0, 1 };

        int step = 1;
        int totalCells = 0;
        bool end = false;
        while (!end)
        {
            for (int _ = 0; !end && _ < 2; _++)
            {
                for (int s = 0; !end && s < step; s++)
                {
                    fixedCell[row, col] = true;
                    int prevRow = row;
                    int prevCol = col;
                    row = row + dr[d];
                    col = col + dc[d];

                    if (grid[row, col] != grid[prevRow, prevCol])
                    {
                        var path = ShortestPath(row, col, grid[prevRow, prevCol], grid, fixedCell);
                        if (path == null) return null;
                        for (int i = 0; i < path.Count - 1; i++)
                        {
                            moves.Add(path[i].row.ToString() + " " + path[i].col.ToString() + " " + path[i + 1].row.ToString() + " " + path[i + 1].col.ToString());
                            int tmp = grid[path[i].row, path[i].col];
                            grid[path[i].row, path[i].col] = grid[path[i + 1].row, path[i + 1].col];
                            grid[path[i + 1].row, path[i + 1].col] = tmp;
                        }
                    }

                    totalCells++;
                    if (totalCells >= N * N - 1) end = true;
                }
                d = (d + 1) % 4;
            }
            step++;

        }

        return moves;
    }

    public static List<string> tryPermutationsOfSpiral(int[,] grid, DateTime startTime, int msToStop)
    {
        int[,] tmpGrid = new int[N, N];

        int bestScore = int.MaxValue;
        var bestMoves = new List<string>();

        int[] permutations = new int[C];
        for (int i = 0; i < C; i++) permutations[i] = i;

        for(int c=0; c<C; c++)
        {
            copyGrid(grid, tmpGrid, permutations);
            List<string> moves = spiralSolution(tmpGrid, c);
            if (moves == null) return null;

            if (moves.Count < bestScore)
            {
                bestScore = moves.Count;
                bestMoves = moves;
            }

            var endTime = DateTime.Now;
            Double elapsedMillisecs = ((TimeSpan)(endTime - startTime)).TotalMilliseconds;
            if (elapsedMillisecs > msToStop) break;
        }
        return bestMoves;
    }

    public static void BalanceColorsForEachColumn(int[,] grid, List<string> moves)
    {
        //Count color per column
        var colorsTotal = new int[C];
        var colorMean = new int[C];
        var colorsPerColumn = new int[N, C];
        for (int col = 0; col < N; col++)
        {
            for (int row = 0; row < N; row++)
            {
                colorsTotal[grid[row, col]]++;
                colorsPerColumn[col, grid[row, col]]++;
            }
        }
        for (int c = 0; c < C; c++) colorMean[c] = (int)Math.Ceiling(colorsTotal[c] / (double)N);

        //If a columnA has more than mean colors swap with adjacent columnB iff columnA.colorCount > columnB.colorCount
        for (int _ = 0; _ < 10; _++)
        {
            for (int col = 0; col < N - 1; col++)
            {
                for (int row = 0; row < N; row++)
                {
                    int color1 = grid[row, col];
                    int color2 = grid[row, col + 1];
                    if (color1 != color2)
                    {
                        if (colorsPerColumn[col, color1] > colorMean[color1] && colorsPerColumn[col + 1, color1] < colorMean[color1] &&
                            colorsPerColumn[col + 1, color2] > colorMean[color2] && colorsPerColumn[col, color2] < colorMean[color2])
                        {

                            int tmp = grid[row, col];
                            grid[row, col] = grid[row, col + 1];
                            grid[row, col + 1] = tmp;
                            moves.Add(row.ToString() + " " + col.ToString() + " " + (row).ToString() + " " + (col + 1).ToString());

                            colorsPerColumn[col, color1]--;
                            colorsPerColumn[col + 1, color1]++;

                            colorsPerColumn[col, color2]++;
                            colorsPerColumn[col + 1, color2]--;
                        }
                    }
                }
            }
        }

    }

    public static void Main(string[] args)
    {

        var startTime = DateTime.Now;

        var bestScore = int.MaxValue;
        var bestMoves = new List<string>();

        N = int.Parse(Console.ReadLine());
        C = int.Parse(Console.ReadLine());
        var grid = new int[N, N];
        for (int r = 0; r < N; r++) for (int c = 0; c < N; c++) grid[r, c] = int.Parse(Console.ReadLine());

        ////Example seed 1
        //N = 4;
        //C = 3;
        //var grid = new int[,]{
        //    { 1,0,0,0 },
        //    { 0,1,1,1 },
        //    { 2,2,1,0 },
        //    { 0,1,1,2 }
        //};

        var tmpMoves = new List<string>();

        tmpMoves = tryPermutationsByRow(grid, startTime, 2000);
        if (tmpMoves != null && tmpMoves.Count < bestScore)
        {
            bestScore = tmpMoves.Count;
            bestMoves = tmpMoves;
        }

        tmpMoves = tryPermutationsByCol(grid, startTime, 4000);
        if (tmpMoves != null && tmpMoves.Count < bestScore)
        {
            bestScore = tmpMoves.Count;
            bestMoves = tmpMoves;
        }

        tmpMoves = tryPermutationsOfSpiral(grid, startTime, 7000);
        if (tmpMoves != null && tmpMoves.Count < bestScore)
        {
            bestScore = tmpMoves.Count;
            bestMoves = tmpMoves;
        }

        //print solution
        Console.WriteLine(bestMoves.Count);
        foreach(string move in bestMoves) Console.WriteLine(move);
        Console.Out.Flush();
    }



}

