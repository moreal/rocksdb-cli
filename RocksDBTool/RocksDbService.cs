namespace RocksDBTool
{
    using Microsoft.Extensions.Configuration;
    using RocksDbSharp;

    /// <summary>
    /// A default implementation of <see cref="IRocksDbService"/> interface.
    /// </summary>
    public sealed class RocksDbService : IRocksDbService
    {
        /// <summary>
        /// The key of <see cref="CurrentRocksDbPath"/> in configuration.
        /// </summary>
        public const string CurrentRocksDbPathKey = "current_rocks_db_path";

        private readonly IConfiguration _configuration;

        private readonly DbOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="RocksDbService"/> class.
        /// </summary>
        /// <param name="configuration">The configuration having variables to load <see cref="RocksDb"/>.</param>
        public RocksDbService(IConfiguration configuration)
        {
            _configuration = configuration;
            _options = new DbOptions();
        }

        /// <inheritdoc cref="IRocksDbService.Load"/>
        public string CurrentRocksDbPath => _configuration[CurrentRocksDbPathKey];

        /// <summary>
        /// Open <see cref="RocksDb"/> from path in configuration.
        /// </summary>
        /// <returns>A <see cref="RocksDb"/> instance.</returns>
        public RocksDb Load()
        {
            return RocksDb.Open(
                _options,
                CurrentRocksDbPath);
        }
    }
}
