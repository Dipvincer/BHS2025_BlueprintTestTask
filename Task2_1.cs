namespace Task2;

public class TreeNode<T>(T value, TreeNode<T>? left = null, TreeNode<T>? right = null)
{
    public T Value { get; } = value;
    public TreeNode<T>? Left { get; init; } = left;
    public TreeNode<T>? Right { get; init; } = right;

    public int MaxDepth() => 1 + Math.Max(Left?.MaxDepth() ?? 0, Right?.MaxDepth() ?? 0);
}


public static class Program
{
    private static void Main()
    {
        // [3,9,20,null,null,15,7]
        var root = new TreeNode<int>(3)
        {
            Left = new TreeNode<int>(9),
            Right = new TreeNode<int>(20)
            {
                Left = new TreeNode<int>(15),
                Right = new TreeNode<int>(7)
            }
        };
        
        Console.WriteLine(root.MaxDepth()); // 3
        
        
        // [1,null,2]
        root = new TreeNode<int>(1)
        {
            Right = new TreeNode<int>(2)
        };
        
        Console.WriteLine(root.MaxDepth()); // 2
    }
}