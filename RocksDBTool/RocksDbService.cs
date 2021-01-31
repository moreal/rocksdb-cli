namespace RocksDBTool
{
    using System;
    using RocksDbSharp;

    /// <summary>
    /// A default implementation of <see cref="IRocksDbService"/> interface.
    /// </summary>
    public sealed class RocksDbService : IRocksDbService
    {
        private readonly DbOptions _options;

        /// <summary>
        /// Initializes a new instance of the <see cref="RocksDbService"/> class.
        /// </summary>
        public RocksDbService()
        {
            _options = new DbOptions();
        }

        /// <summary>
        /// Open <see cref="RocksDb"/> from path in configuration.
        /// </summary>
        /// <param name="path">The path of <see cref="RocksDb"/> to load.</param>
        /// <returns>A <see cref="RocksDb"/> instance.</returns>
        public RocksDb Load(string path)
        {
            return RocksDb.Open(_options, path);
        }
    }
}
