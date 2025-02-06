namespace Task2;

public static class Solution
{
    public static int CountIslands(char[][]? grid)
    {
        if (grid == null || grid.Length == 0) return 0;
        
        int count = 0;
        var directions = new[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
        
        for (int i = 0; i < grid.Length; i++)
        {
            for (int j = 0; j < grid[0].Length; j++)
            {
                if (grid[i][j] != '1') continue;
                
                count++;
                TraverseIsland(grid, (i, j), directions);
            }
        }
        
        return count;
    }

    private static void TraverseIsland(char[][] grid, (int r, int c) start, (int dr, int dc)[] directions)
    {
        var queue = new Queue<(int r, int c)>();
        queue.Enqueue(start);
        grid[start.r][start.c] = '0';

        while (queue.TryDequeue(out var current))
        {
            foreach (var (dr, dc) in directions)
            {
                int nr = current.r + dr;
                int nc = current.c + dc;

                if (!IsValidCell(grid, nr, nc)) continue;
                
                grid[nr][nc] = '0';
                queue.Enqueue((nr, nc));
            }
        }
    }

    private static bool IsValidCell(char[][] grid, int r, int c) =>
        r >= 0 && r < grid.Length &&
        c >= 0 && c < grid[0].Length &&
        grid[r][c] == '1';
}


public static class Program
{
    private static void Main()
    {
        char[][] grid =
        [
            ['1', '1', '1', '1', '0'],
            ['1', '1', '0', '1', '0'],
            ['1', '1', '0', '0', '0'],
            ['0', '0', '0', '0', '0']
        ];
        Console.WriteLine(Solution.CountIslands(grid)); // 1

        grid =
        [
            ['1', '1', '0', '0', '0'],
            ['1', '1', '0', '0', '0'],
            ['0', '0', '1', '0', '0'],
            ['0', '0', '0', '1', '1']
        ];
        Console.WriteLine(Solution.CountIslands(grid)); // 3
    }
}