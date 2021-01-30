namespace RocksDBTool.Tests
{
    public class MockRocksDbFileConfigurationService : IFileConfigurationService<RocksDbServiceConfiguration>
    {
        private readonly string _path;
        private readonly RocksDbServiceConfiguration _configuration;

        public MockRocksDbFileConfigurationService(string path, RocksDbServiceConfiguration configuration)
        {
            _path = path;
            _configuration = configuration;
        }

        public string Path => _path;

        public RocksDbServiceConfiguration Load()
        {
            return _configuration;
        }

        public void Store(RocksDbServiceConfiguration configuration)
        {
            throw new System.NotSupportedException();
        }
    }
}
