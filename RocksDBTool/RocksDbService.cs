namespace RocksDBTool
{
    using Microsoft.Extensions.Configuration;
    using RocksDbSharp;

    public sealed class RocksDbService : IRocksDbService
    {
        public const string CurrentRocksDbPathKey = "current_rocks_db_path";

        private readonly IConfiguration _configuration;

        private readonly DbOptions _options;

        public RocksDbService(IConfiguration configuration)
        {
            _configuration = configuration;
            _options = new DbOptions();
        }

        private string CurrentRocksDbPath => _configuration[CurrentRocksDbPathKey];

        public RocksDb Load()
        {
            return RocksDb.Open(
                _options,
                CurrentRocksDbPath);
        }
    }
}
