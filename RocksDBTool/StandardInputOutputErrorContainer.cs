namespace RocksDBTool
{
    using System;
    using System.IO;

    public sealed class StandardInputOutputErrorContainer : IInputOutputErrorContainer
    {
        public TextReader In => Console.In;

        public TextWriter Out => Console.Out;

        public TextWriter Error => Console.Error;
    }
}
