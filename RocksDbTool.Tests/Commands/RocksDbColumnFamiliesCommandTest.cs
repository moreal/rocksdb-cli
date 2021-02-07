namespace RocksDbTool.Tests.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using RocksDbSharp;
    using RocksDbTool.Commands;
    using RocksDbTool.Services;
    using Xunit;

    public class RocksDbColumnFamiliesCommandTest
    {
        private readonly IRocksDbService _rocksDbService;

        private readonly RocksDbColumnFamiliesCommand _command;

        private readonly StringInputOutputErrorContainer _stringInputOutputErrorContainer;

        private readonly string _temporaryDirectory;

        private readonly List<Guid> _columnFamilies;

        public RocksDbColumnFamiliesCommandTest()
        {
            _temporaryDirectory = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString());

            _columnFamilies = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid())
                .ToList();
            using (var db = OpenRocksDb(_temporaryDirectory))
            {
                var columnFamilyOptions = new ColumnFamilyOptions();
                foreach (var columnFamily in _columnFamilies)
                {
                    db.CreateColumnFamily(
                        columnFamilyOptions,
                        columnFamily.ToString());
                }
            }

            _rocksDbService = new RocksDbService();
            _stringInputOutputErrorContainer = new StringInputOutputErrorContainer(
                new StringReader(string.Empty),
                new StringWriter(),
                new StringWriter());
            _command = new RocksDbColumnFamiliesCommand(
                _stringInputOutputErrorContainer,
                _rocksDbService);

            _stringInputOutputErrorContainer.SetNewLines();
        }

        [Fact]
        public void Create()
        {
            var guid = Guid.NewGuid();
            _command.Create(guid.ToString(), rocksdbPath: _temporaryDirectory);
            using var db = _rocksDbService.Load(_temporaryDirectory);
            Assert.NotNull(db.GetColumnFamily(guid.ToString()));
        }

        [Fact]
        public void Remove()
        {
            foreach (var columnFamily in _columnFamilies)
            {
                using (var db = _rocksDbService.Load(_temporaryDirectory))
                {
                    Assert.NotNull(db.GetColumnFamily(columnFamily.ToString()));
                }

                _command.Remove(columnFamily.ToString(), rocksdbPath: _temporaryDirectory);
                using (var db = _rocksDbService.Load(_temporaryDirectory))
                {
                    Assert.Throws<KeyNotFoundException>(() => db.GetColumnFamily(columnFamily.ToString()));
                }
            }
        }

        [Fact]
        public void List()
        {
            _command.List(_temporaryDirectory);
            Assert.Equal(
                "default\n" + string.Join("\n", _columnFamilies) + "\n",
                _stringInputOutputErrorContainer.Out.ToString());
        }

        private RocksDb OpenRocksDb(string path)
        {
            var options = new DbOptions().SetCreateIfMissing();
            return RocksDb.Open(options, path, new ColumnFamilies());
        }
    }
}
