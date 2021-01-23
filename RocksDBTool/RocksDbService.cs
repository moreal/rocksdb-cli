#nullable enable
using Microsoft.Extensions.Configuration;
using RocksDbSharp;

namespace RocksDBTool
{
    public class RocksDbService : IRocksDbService
    {
        private readonly DbOptions _options;

        private readonly string _currentRocksDbPath;

        public const string CurrentRocksDbPathKey = "current_rocks_db_path";

        public RocksDbService(IConfiguration configuration)
        {
            _currentRocksDbPath = configuration[CurrentRocksDbPathKey];
            _options = new DbOptions();
        }

        public RocksDb Load()
        {
            return RocksDb.Open(
                _options,
                _currentRocksDbPath);   
        }
    }
}
