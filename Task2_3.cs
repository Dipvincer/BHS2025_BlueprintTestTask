namespace Task2;

public class TreeNode<T> where T : IComparable<T>
{
    public TreeNode(T[] values)
    {
        if (values.Length == 0) throw new ArgumentException("Cannot create a tree from an empty array.");

        Value = values[0];
        for (var i = 1; i < values.Length; i++) Insert(values[i]);
    }
    
    public TreeNode(T value, TreeNode<T>? left = null, TreeNode<T>? right = null)
    {
        Value = value;
        Left = left;
        Right = right;
    }

    public T Value { get; }
    public TreeNode<T>? Left { get; private set; }
    public TreeNode<T>? Right { get; private set; }

    public void Insert(T newValue)
    {
        TreeNode<T>? current = this;
        TreeNode<T>? parent = null;

        while (current != null)
        {
            parent = current;
            current = newValue.CompareTo(current.Value) < 0 ? current.Left : current.Right;
        }

        if (parent == null) throw new InvalidOperationException("Cannot insert into a null tree.");

        if (newValue.CompareTo(parent.Value) < 0)
        {
            parent.Left = new TreeNode<T>(newValue);
        }
        else
        {
            parent.Right = new TreeNode<T>(newValue);
        }
    }
}

public class BSTIterator<T>(TreeNode<T>? root) where T : IComparable<T>
{
    private readonly Stack<TreeNode<T>> _stack = new();
    private TreeNode<T>? _current = root;

    public BSTIterator() : this(null) { }

    public T Next()
    {
        while (_current != null)
        {
            _stack.Push(_current);
            _current = _current.Left;
        }

        var node = _stack.Pop();
        _current = node.Right;
        return node.Value;
    }

    public bool HasNext() => _stack.Count > 0 || _current != null;
}

public static class Program
{
    private static void Main()
    {
        var root = new TreeNode<int>([7, 3, 15, 9, 20]);

        var iterator = new BSTIterator<int>(root);
        Console.WriteLine(iterator.Next()); // 3
        Console.WriteLine(iterator.Next()); // 7
        Console.WriteLine(iterator.HasNext()); // True
        Console.WriteLine(iterator.Next()); // 9
        Console.WriteLine(iterator.HasNext()); // True
        Console.WriteLine(iterator.Next()); // 15
        Console.WriteLine(iterator.HasNext()); // True
        Console.WriteLine(iterator.Next()); // 20
        Console.WriteLine(iterator.HasNext()); // False
    }
}