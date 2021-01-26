namespace RocksDBTool
{
    using System.IO;

    public interface IInputOutputErrorContainer
    {
        TextReader In { get; }

        TextWriter Out { get; }

        TextWriter Error { get; }
    }
}
