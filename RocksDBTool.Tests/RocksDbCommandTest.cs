#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;
using RocksDbSharp;
using Xunit;

namespace RocksDBTool.Tests
{
    public class RocksDbCommandTest
    {
        private readonly IRocksDbService _rocksDbService;

        private readonly RocksDbCommand _command;

        private readonly StringWriter _stringWriter;

        public RocksDbCommandTest()
        {
            
            var temporaryDirectory = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString());
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        [RocksDbService.CurrentRocksDbPathKey] = temporaryDirectory,
                    }).Build();
            SetupRocksDb(temporaryDirectory, new Dictionary<string, string>
            {
                ["string"] = "foo",
            });
            SetupRocksDb(temporaryDirectory, new Dictionary<byte[], byte[]>
            {
                [Encoding.ASCII.GetBytes("binary")] = new byte[] { 0xbe, 0xef },
            });

            _rocksDbService = new RocksDbService(configuration);
            _stringWriter = new StringWriter();
            _command = new RocksDbCommand(_stringWriter, _rocksDbService);
        }
        
        [Theory]
        [InlineData("binary", 0, "\xbe\xef")]
        [InlineData("string", 0, "foo")]
        [InlineData("unknown", -1, "")]
        [InlineData("\x00", -1, "")]
        public void Get(string key, int expectedReturnCode, string expectedOutput)
        {
            Assert.Equal(expectedReturnCode,_command.Get(key));  // key: binary
            Assert.Equal(expectedOutput, _stringWriter.ToString());
        }
        
        [Theory]
        [InlineData("foo", "bar", 0)]
        [InlineData("\xde\xad", "\xbe\xef", 0)]
        public void Set(string key, string value, int expectedReturnCode)
        {
            Assert.Equal(expectedReturnCode, _command.Set(key, value));
            using var db = _rocksDbService.Load();
            Assert.Equal(value, db.Get(key));
        }

        private void SetupRocksDb(string path, IEnumerable<KeyValuePair<string, string>> pairs)
        {
            using var db = OpenRocksDb(path);
            foreach (var pair in pairs)
            {
                db.Put(
                    pair.Key,
                    pair.Value);
            }
        }

        private void SetupRocksDb(string path, IEnumerable<KeyValuePair<byte[], byte[]>> pairs)
        {
            using var db = OpenRocksDb(path);
            foreach (var pair in pairs)
            {
                db.Put(
                    pair.Key,
                    pair.Value);
            }
        }

        private RocksDb OpenRocksDb(string path)
        {
            var options = new DbOptions().SetCreateIfMissing();
            return RocksDb.Open(options, path);
        }
    }
}
