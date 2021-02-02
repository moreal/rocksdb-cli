namespace RocksDbTool.Tests.Services
{
    using System;
    using System.IO;
    using RocksDbSharp;
    using RocksDbTool.Services;
    using Xunit;

    public sealed class RocksDbServiceTest : IDisposable
    {
        private readonly IRocksDbService _rocksDbService;

        private readonly string _temporaryDirectory;

        public RocksDbServiceTest()
        {
            _temporaryDirectory = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString());
            _rocksDbService = new RocksDbService();
        }

        [Fact]
        public void Load()
        {
            var options = new DbOptions().SetCreateIfMissing().SetErrorIfExists();
            var guid = Guid.NewGuid();

            using (var db = RocksDb.Open(
                options,
                _temporaryDirectory))
            {
                db.Put("foo", "bar");
                db.Put("guid", guid.ToString());
            }

            using (var db = _rocksDbService.Load(_temporaryDirectory))
            {
                Assert.Equal("bar", db.Get("foo"));
                Assert.Equal(guid.ToString(), db.Get("guid"));
            }
        }

        [Fact]
        public void LoadThrowsExceptionIfMissing()
        {
            Assert.Throws<RocksDbException>(() => _rocksDbService.Load(_temporaryDirectory));
        }

        public void Dispose()
        {
            Directory.Delete(_temporaryDirectory, true);
        }
    }
}
