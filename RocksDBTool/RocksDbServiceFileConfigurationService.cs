namespace RocksDBTool
{
    using System;

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
            : this(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile))
        {
        }

        /// <inheritdoc cref="IFileConfigurationService{T}.Path"/>
        public string Path { get; }

        /// <inheritdoc cref="IFileConfigurationService{T}.Load"/>
        public RocksDbServiceConfiguration Load()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc cref="IFileConfigurationService{T}.Store"/>
        public void Store(RocksDbServiceConfiguration configuration)
        {
            throw new System.NotImplementedException();
        }
    }
}
