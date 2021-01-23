using RocksDbSharp;

namespace RocksDBTool
{
    public interface IRocksDbService
    {
        RocksDb Load();
    }
}
