namespace RocksDBTool
{
    using RocksDbSharp;

    public interface IRocksDbService
    {
        RocksDb Load();
    }
}
