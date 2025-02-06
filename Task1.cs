namespace Task1;

public abstract class Vertex(string title, Type type)
{
    public Type Type { get; } = type;
    public string Title { get; } = title;
}

public class OrderVertex(string title, IExecutableBlock? executableBlockOwner) : Vertex(title, typeof(OrderVertex))
{
    public IExecutableBlock? ExecutableBlockOwner { get; set; } = executableBlockOwner;
}

public class InputVertex(string title, Type type, OutputVertex? source = null) : Vertex(title, type)
{
    public OutputVertex? Source { get; set; } = source;
    
    public bool SourceReady => Source?.Evaluated == true && (Source.Type == Type || Type == typeof(object));
}

public class NecessaryInputVertex(string title, Type type, OutputVertex? source = null) : InputVertex(title, type, source);

public class OutputVertex(string title, Type type, object? value = null, bool evaluated = false) : Vertex(title, type)
{
    public bool Evaluated { get; private set; } = evaluated || value != null;
    public object? Value { get; private set; } = value;

    public void SetValue(object? value)
    {
        Evaluated = true;
        Value = value;
    }
}

public interface IBlock
{
    public string Title { get; }
};

public interface IExecutableBlock : IBlock
{
    public void Execute();
}

public interface IInitialBlock : IBlock
{
    public OrderVertex Next { get; }
    
    public void SetNextBlock(IExecutableBlock target) => Next.ExecutableBlockOwner = target;
}

public interface ITerminalBlock : IBlock
{
    public OrderVertex Previous { get; }
    
    public void SetPreviusBlock(IExecutableBlock source) => Previous.ExecutableBlockOwner = source;
}

public interface IOrderedExecutableBlock : ITerminalBlock, IInitialBlock, IExecutableBlock;

public interface IInputBlock : IBlock
{
    public InputVertex[] Inputs { get; }
}

public interface IOutputBlock : IBlock
{
    public OutputVertex[] Outputs { get; }
}

public interface ISimpleBlock : IInputBlock, IOutputBlock;

public abstract class SimpleOrderedExecutableBlock : ISimpleBlock, IOrderedExecutableBlock
{
    protected SimpleOrderedExecutableBlock(IInitialBlock? previous, ITerminalBlock? next)
    {
        if (previous == null)
        {
            Previous = new OrderVertex(nameof(Previous), null);
        }
        else
        {
            previous.SetNextBlock(this);
            Previous = new OrderVertex(nameof(Previous), previous as IExecutableBlock);
        }
        
        if (next == null)
        {
            Next = new OrderVertex(nameof(Next), null);
        }
        else
        {
            next.SetPreviusBlock(this);
            Next = new OrderVertex(nameof(Next), next as IExecutableBlock);
        }
    }

    public abstract string Title { get; set; }
    
    public abstract InputVertex[] Inputs { get; }
    public abstract OutputVertex[] Outputs { get; }

    public OrderVertex Previous { get; }
    public OrderVertex Next { get; }

    private bool CheckInputs()
    {
        foreach (var input in Inputs)
        {
            if (input is NecessaryInputVertex { SourceReady: false })
            {
                return false;
            }
        }

        return true;
    }
    
    private static void SetInput(InputVertex input, OutputVertex source) => input.Source = source;
    public void SetInput(int inputIndex, IOutputBlock source, int outputIndex)
    {
        SetInput(Inputs[inputIndex], source.Outputs[outputIndex]);
    }
    public void SetInput(int inputIndex, OutputVertex source)
    {
        SetInput(Inputs[inputIndex], source);
    }
    
    protected abstract void Run();
    
    public void Execute()
    {
        if (!CheckInputs()) throw new InvalidOperationException("Not all inputs are ready");
        
        Run();
        
        Next.ExecutableBlockOwner?.Execute();
    }
}

public class Constant<T>(T value) : IOutputBlock
{
    public string Title => nameof(Constant<T>);

    public OutputVertex[] Outputs { get; } = [new("Out", typeof(T), value)];
}

public class Entrypoint : IInitialBlock
{
    public string Title => nameof(Entrypoint);
    public OrderVertex Next { get; } = new(nameof(Next), null);
    
    public void Execute() => Next.ExecutableBlockOwner?.Execute();
}

public class Adder(IInitialBlock previous, ITerminalBlock? next = null) : SimpleOrderedExecutableBlock(previous, next)
{
    public override string Title { get; set; } = nameof(Adder);
    
    public override InputVertex[] Inputs { get; } = [
        new NecessaryInputVertex("Term 1", typeof(int)), 
        new NecessaryInputVertex("Term 2", typeof(int))];
    
    public override OutputVertex[] Outputs { get; } = [
        new("Sum", typeof(int))];

    protected override void Run()
    {
        var term1 = (int)Inputs[0].Source!.Value!;
        var term2 = (int)Inputs[1].Source!.Value!;
        
        Outputs[0].SetValue(term1 + term2);
    }
}

public class Printer(IInitialBlock previous, ITerminalBlock? next = null) : SimpleOrderedExecutableBlock(previous, next)
{
    public override string Title { get; set; } = nameof(Printer);
    
    public override InputVertex[] Inputs { get; } = [
        new NecessaryInputVertex("Value", typeof(object))];
    
    public override OutputVertex[] Outputs { get; } = [
        new("Passthrough", typeof(object))
    ];
    
    protected override void Run()
    {
        var value = Inputs[0].Source!.Value;
        
        Console.WriteLine(value);
    }
}

public static class Program
{
    private static void Main()
    {
        var entrypoint = new Entrypoint();
        
        var const1 = new Constant<int>(1);
        var const2 = new Constant<int>(2);
        
        var adder = new Adder(entrypoint);
        adder.SetInput(0, const1, 0);
        adder.SetInput(1, const2.Outputs[0]);
        
        var printer = new Printer(adder);
        printer.SetInput(0, adder, 0);
        
        entrypoint.Execute();
    }
}