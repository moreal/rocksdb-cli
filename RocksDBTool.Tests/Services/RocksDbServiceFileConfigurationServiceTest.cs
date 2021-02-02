namespace RocksDBTool.Tests.Services
{
    using System;
    using System.IO;
    using RocksDBTool.Services;
    using Xunit;

    public class RocksDbServiceFileConfigurationServiceTest
    {
        private readonly string _path;
        private readonly RocksDbServiceFileConfigurationService _service;
        private readonly RocksDbServiceFileConfigurationService _defaultService;

        public RocksDbServiceFileConfigurationServiceTest()
        {
            _path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
            _service = new RocksDbServiceFileConfigurationService(_path);
            _defaultService = new RocksDbServiceFileConfigurationService();
        }

        [Fact]
        public void Path()
        {
            Assert.Equal(_path, _service.Path);
        }

        [Fact]
        public void DefaultPath()
        {
            Assert.Equal(RocksDbServiceFileConfigurationService.DefaultPath, _defaultService.Path);
        }

        [Fact]
        public void Load()
        {
            var guidString = Guid.NewGuid().ToString();
            var configuration = new RocksDbServiceConfiguration
            {
                CurrentRocksDbPath = guidString,
            };
            var configurationString = @$"{{""current_rocksdb_path"":""{guidString}""}}";

            File.WriteAllText(_path, configurationString);
            Assert.Equal(configuration, _service.Load());
        }

        [Fact]
        public void Store()
        {
            var guidString = Guid.NewGuid().ToString();
            var configuration = new RocksDbServiceConfiguration
            {
                CurrentRocksDbPath = guidString,
            };
            var expected = @$"{{""current_rocksdb_path"":""{guidString}""}}";

            _service.Store(configuration);
            Assert.Equal(expected, File.ReadAllText(_path));
        }
    }
}
