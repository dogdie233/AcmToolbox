#pragma warning disable CS9113 // 参数未读。

using System.Runtime.CompilerServices;

namespace AcmToolbox;

public readonly struct IO() : IDisposable
{
    public readonly ConsoleReader reader = new();
    public readonly ConsoleWriter writer = new();
    
    public void Dispose() => writer.Flush();
}

public class ConsoleWriter
{
    public readonly TextWriter writer = Console.Out;
    
    public void Write([InterpolatedStringHandlerArgument("")] Handler handler){}

    public void Flush() => writer.Flush();

    public static void Flush(ConsoleWriter writer) => writer.Flush();
    
    public static string Endl() => Environment.NewLine;
    
    [InterpolatedStringHandler]
    public readonly ref struct Handler(int literalLength, int formattedCount, ConsoleWriter writer)
    {
        public void AppendLiteral(string s) => writer.writer.Write(s);
        
        public void AppendFormatted<T>(T value) => writer.writer.Write(value);
        
        public void AppendFormatted(Action value) => value();
        
        public void AppendFormatted(Action<ConsoleWriter> value) => value(writer);
        
        public void AppendFormatted<T>(Func<T> value) => writer.writer.Write(value());
        
        public void AppendFormatted<T>(Func<ConsoleWriter, T> value) => writer.writer.Write(value(writer));
    }
}

public class ConsoleReader
{
    public readonly TextReader reader = Console.In;
    private string? _lineCache;
    private int _index;
    
    public string LineCache => _lineCache ??= RenewCacheLine();
    
    public void Read([InterpolatedStringHandlerArgument("")] Handler handler) {}

    private void Advance()
    {
        while (true)
        {
            if (_index >= LineCache.Length)
            {
                RenewCacheLine();
                _index = 0;
                continue;
            }
            if (char.IsWhiteSpace(LineCache[_index]))
            {
                _index++;
                continue;
            }

            break;
        }
    }
    
    private ReadOnlySpan<char> NextToken()
    {
        Advance();
        
        var start = _index;
        while (_index < LineCache.Length && !char.IsWhiteSpace(LineCache[_index]))
            _index++;
        
        return LineCache.AsSpan(start, _index - start);
    }
    
    public string RenewCacheLine() => _lineCache = reader.ReadLine() ?? throw new EndOfStreamException();
    
    [InterpolatedStringHandler]
    public readonly ref struct Handler(int literalLength, int formattedCount, ConsoleReader reader)
    {
        public void AppendLiteral(string s) {}  // Unimplemented
        
        public void AppendFormatted<T>(in T value) where T : ISpanParsable<T>
        {
            var token = reader.NextToken();
            ref var valueRef = ref Unsafe.AsRef(in value);
            if (!T.TryParse(token, null, out valueRef!))
                throw new FormatException($"Failed to parse '{token.ToString()}' to {typeof(T).Name}");
        }
    }
}