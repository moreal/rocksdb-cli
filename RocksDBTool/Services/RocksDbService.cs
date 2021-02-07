namespace RocksDbTool.Services
{
    using RocksDbSharp;

    /// <summary>
    /// A default implementation of <see cref="IRocksDbService"/> interface.
    /// </summary>
    public sealed class RocksDbService : IRocksDbService
    {
        private readonly DbOptions _options;

        private readonly ColumnFamilyOptions _columnFamilyOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="RocksDbService"/> class.
        /// </summary>
        public RocksDbService()
        {
            _options = new DbOptions();
            _columnFamilyOptions = new ColumnFamilyOptions();
        }

        /// <summary>
        /// Open <see cref="RocksDb"/> from path in configuration.
        /// </summary>
        /// <param name="path">The path of <see cref="RocksDb"/> to load.</param>
        /// <returns>A <see cref="RocksDb"/> instance.</returns>
        public RocksDb Load(string path)
        {
            var columnFamilies = new ColumnFamilies();

            foreach (var columnFamily in RocksDb.ListColumnFamilies(
                _options,
                path))
            {
                if (columnFamily is null)
                {
                    continue;
                }

                columnFamilies.Add(columnFamily, new ColumnFamilyOptions());
            }

            return RocksDb.Open(_options, path, columnFamilies!);
        }
    }
}
