using System.IO;

namespace RocksDBTool.Tests
{
    public sealed class StringInputOutputErrorContainer : IInputOutputErrorContainer
    {
        public StringInputOutputErrorContainer(StringReader @in, StringWriter error, StringWriter @out)
        {
            In = @in;
            Out = @out;
            Error = error;
        }

        TextReader IInputOutputErrorContainer.In => In; 

        TextWriter IInputOutputErrorContainer.Out => Out;

        TextWriter IInputOutputErrorContainer.Error => Error;

        public StringReader In { get; }

        public StringWriter Out { get; }

        public StringWriter Error { get; }
    }
}
