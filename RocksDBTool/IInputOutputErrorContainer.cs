using System.IO;

namespace RocksDBTool
{
    public interface IInputOutputErrorContainer
    {
        TextReader In { get; }

        TextWriter Out { get; }

        TextWriter Error { get; }
    }
}
