using System;
using Microsoft.Extensions.Configuration;
using RocksDbSharp;

namespace RocksDBTool
{
    public class RocksDbService : IRocksDbService
    {
        private readonly IConfiguration _configuration;

        private readonly DbOptions _options;

        public const string CurrentRocksDbPathKey = "current_rocks_db_path";

        public string CurrentRocksDbPath => _configuration[CurrentRocksDbPathKey];

        public RocksDbService(IConfiguration configuration)
        {
            _configuration = configuration;
            _options = new DbOptions();
        }

        public RocksDb Load()
        {
            if (CurrentRocksDbPath is null)
            {
                throw new ArgumentNullException(nameof(CurrentRocksDbPath));
            }

            return RocksDb.Open(
                _options,
                CurrentRocksDbPath);   
        }
    }
}
