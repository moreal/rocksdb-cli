namespace RocksDBTool
{
    using System;
    using RocksDbSharp;

    /// <summary>
    /// A default implementation of <see cref="IRocksDbService"/> interface.
    /// </summary>
    public sealed class RocksDbService : IRocksDbService
    {
        private readonly IFileConfigurationService<RocksDbServiceConfiguration> _configurationService;

        private readonly DbOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="RocksDbService"/> class.
        /// </summary>
        /// <param name="configurationService">A service to load the configuration having variables
        /// to load <see cref="RocksDb"/>.</param>
        public RocksDbService(IFileConfigurationService<RocksDbServiceConfiguration> configurationService)
        {
            _configurationService = configurationService;
            _options = new DbOptions();
        }

        /// <inheritdoc cref="IRocksDbService.Load"/>
        private string? CurrentRocksDbPath => _configurationService.Load().CurrentRocksDbPath;

        /// <summary>
        /// Open <see cref="RocksDb"/> from path in configuration.
        /// </summary>
        /// <returns>A <see cref="RocksDb"/> instance.</returns>
        public RocksDb Load() => Load(CurrentRocksDbPath);

        private RocksDb Load(string? currentRocksDbPath)
        {
            if (currentRocksDbPath is null)
            {
                throw new ArgumentNullException(nameof(currentRocksDbPath));
            }

            return RocksDb.Open(
                _options,
                currentRocksDbPath!);
        }
    }
}
