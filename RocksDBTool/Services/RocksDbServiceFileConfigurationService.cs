namespace RocksDbTool.Services
{
    using System;
    using System.IO;
    using System.Text.Json;

    /// <inheritdoc cref="IFileConfigurationService{T}"/>
    /// <seealso cref="RocksDbService"/>
    /// <seealso cref="RocksDbServiceConfiguration"/>
    public class RocksDbServiceFileConfigurationService : IFileConfigurationService<RocksDbServiceConfiguration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RocksDbServiceFileConfigurationService"/> class.
        /// </summary>
        /// <param name="path">The path of rocksdb.</param>
        public RocksDbServiceFileConfigurationService(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RocksDbServiceFileConfigurationService"/> class.
        /// </summary>
        public RocksDbServiceFileConfigurationService()
            : this(DefaultPath)
        {
        }

        /// <summary>
        /// Gets the default path of rocksdb tool configuration file.
        /// </summary>
        /// <see cref="RocksDbServiceFileConfigurationService()"/>
        public static string DefaultPath => System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".config",
            "rocksdb-tool.json");

        /// <inheritdoc cref="IFileConfigurationService{T}.Path"/>
        public string Path { get; }

        /// <inheritdoc cref="IFileConfigurationService{T}.Load"/>
        public RocksDbServiceConfiguration Load()
        {
            return JsonSerializer.Deserialize<RocksDbServiceConfiguration>(File.ReadAllText(Path));
        }

        /// <inheritdoc cref="IFileConfigurationService{T}.Store"/>
        public void Store(RocksDbServiceConfiguration configuration)
        {
            File.WriteAllText(Path, JsonSerializer.Serialize(configuration));
        }
    }
}
