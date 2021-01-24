using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;
using RocksDbSharp;
using Xunit;

namespace RocksDBTool.Tests
{
    public class RocksDbServiceTest : IDisposable
    {
        private readonly IRocksDbService _rocksDbService;

        private readonly string _temporaryDirectory;

        private readonly IConfiguration _configuration;

        public RocksDbServiceTest()
        {
            _temporaryDirectory = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString());
            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    [RocksDbService.CurrentRocksDbPathKey] = _temporaryDirectory,
                });
            _configuration = builder.Build();
            _rocksDbService = new RocksDbService(_configuration);
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

            using (var db = _rocksDbService.Load())
            {
                Assert.Equal("bar", db.Get("foo"));
                Assert.Equal(guid.ToString(), db.Get("guid"));   
            }
        }

        [Fact]
        public void LoadThrowsExceptionIfMissing()
        {
            Assert.Throws<RocksDbException>(() => _rocksDbService.Load());

            _configuration[RocksDbService.CurrentRocksDbPathKey] = null;
            Assert.Throws<ArgumentNullException>(() => _rocksDbService.Load());
        }

        public void Dispose()
        {
            Directory.Delete(_temporaryDirectory, true);
        }
    }
}
