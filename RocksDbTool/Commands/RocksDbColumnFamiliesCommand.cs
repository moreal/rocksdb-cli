namespace RocksDbTool.Commands
{
    using System.IO;
    using Cocona;
    using RocksDbSharp;
    using RocksDbTool.Services;

    /// <summary>
    /// A class to handle column families of <see cref="RocksDbSharp.RocksDb"/> in command line interface.
    /// </summary>
    public sealed class RocksDbColumnFamiliesCommand
    {
        private readonly IInputOutputErrorContainer _inputOutputErrorContainer;
        private readonly IRocksDbService _rocksDbService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RocksDbColumnFamiliesCommand"/> class.
        /// </summary>
        /// <param name="inputOutputErrorContainer">A container having writers to write output and error.</param>
        /// <param name="rocksDbService">A service to load <see cref="RocksDbSharp.RocksDb"/>.</param>
        public RocksDbColumnFamiliesCommand(IInputOutputErrorContainer inputOutputErrorContainer, IRocksDbService rocksDbService)
        {
            _inputOutputErrorContainer = inputOutputErrorContainer;
            _rocksDbService = rocksDbService;
        }

        /// <summary>
        /// Creates a new column family named as the given <paramref name="name"/>,
        /// on <see cref="RocksDbSharp.RocksDb"/> located at <paramref name="rocksdbPath"/>.
        /// </summary>
        /// <param name="name">The name of the column family to create.</param>
        /// <param name="rocksdbPath">The path of <see cref="RocksDbSharp.RocksDb"/> to load.</param>
        public void Create([Argument] string name, [Option] string? rocksdbPath = null)
        {
            rocksdbPath ??= Directory.GetCurrentDirectory();
            using var db = _rocksDbService.Load(rocksdbPath);
            var options = new ColumnFamilyOptions();
            db.CreateColumnFamily(
                options,
                name);
        }

        /// <summary>
        /// Removes the column family named as the given <paramref name="name"/> from <see cref="RocksDbSharp.RocksDb"/> located at <paramref name="rocksdbPath"/>.
        /// </summary>
        /// <param name="name">The name of the column family to remove.</param>
        /// <param name="rocksdbPath">The path of <see cref="RocksDbSharp.RocksDb"/> to load.</param>
        public void Remove([Argument] string name, [Option] string? rocksdbPath = null)
        {
            rocksdbPath ??= Directory.GetCurrentDirectory();
            using var db = _rocksDbService.Load(rocksdbPath);
            db.DropColumnFamily(name);
        }

        /// <summary>
        /// Lists the column families from <see cref="RocksDbSharp.RocksDb"/> located at <paramref name="rocksdbPath"/>.
        /// </summary>
        /// <param name="rocksdbPath">The path of <see cref="RocksDbSharp.RocksDb"/> to load.</param>
        public void List([Option] string? rocksdbPath = null)
        {
            rocksdbPath ??= Directory.GetCurrentDirectory();
            var options = new DbOptions();
            var columnFamilies = RocksDb.ListColumnFamilies(
                options,
                rocksdbPath);
            foreach (var columnFamily in columnFamilies)
            {
                _inputOutputErrorContainer.Out.WriteLine(columnFamily);
            }
        }
    }
}
