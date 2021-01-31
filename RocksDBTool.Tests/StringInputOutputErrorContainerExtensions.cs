namespace RocksDBTool.Tests
{
    public static class StringInputOutputErrorContainerExtensions
    {
        public static void SetNewLines(this StringInputOutputErrorContainer container, string newLine = "\n")
        {
            container.Out.NewLine = newLine;
            container.Error.NewLine = newLine;
        }
    }
}
