using System;
using System.IO;

namespace RocksDBTool
{
    public sealed class StandardInputOutputErrorContainer : IInputOutputErrorContainer
    {
        public TextReader In => Console.In;

        public TextWriter Out => Console.Out;

        public TextWriter Error => Console.Error;
    }
}
