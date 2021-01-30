namespace RocksDBTool.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Extensions.Configuration;
    using RocksDbSharp;
    using Xunit;

    public sealed class RocksDbCommandTest
    {
        private readonly IRocksDbService _rocksDbService;

        private readonly RocksDbCommand _command;

        private readonly StringInputOutputErrorContainer _stringInputOutputErrorContainer;

        public RocksDbCommandTest()
        {
            var temporaryDirectory = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString());
            var configuration = new RocksDbServiceConfiguration
            {
                CurrentRocksDbPath = temporaryDirectory,
            };
            var configurationService = new MockRocksDbFileConfigurationService(
                string.Empty,
                configuration);
            SetupRocksDb(temporaryDirectory, new Dictionary<string, string>
            {
                ["string"] = "foo",
            });
            SetupRocksDb(temporaryDirectory, new Dictionary<byte[], byte[]>
            {
                [new byte[] { 0xde, 0xad }] = new byte[] { 0xbe, 0xef },
            });

            _rocksDbService = new RocksDbService(configurationService);
            _stringInputOutputErrorContainer = new StringInputOutputErrorContainer(
                new StringReader(string.Empty),
                new StringWriter(),
                new StringWriter());
            _command = new RocksDbCommand(
                _stringInputOutputErrorContainer,
                _rocksDbService);
        }

        [Theory]
        [InlineData(InputOutputFormat.Base64, "3q0=", 0, "vu8=")]
        [InlineData(InputOutputFormat.String, "string", 0, "foo")]
        [InlineData(InputOutputFormat.String, "unknown", -1, "")]
        [InlineData(InputOutputFormat.Base64, "AA==", -1, "")]
        public void Get(InputOutputFormat inputOutputFormat, string key, int expectedReturnCode, string expectedOutput)
        {
            Assert.Equal(expectedReturnCode, _command.Get(key, inputOutputFormat));  // key: binary
            Assert.Equal(expectedOutput, _stringInputOutputErrorContainer.Out.ToString());
        }

        [Theory]
        [InlineData("foo", "bar", 0)]
        public void SetWithString(string key, string value, int expectedReturnCode)
        {
            Assert.Equal(expectedReturnCode, _command.Set(key, value, InputOutputFormat.String));
            using var db = _rocksDbService.Load();
            Assert.Equal(value, db.Get(key));
        }

        [Theory]
        [InlineData("3q0=", "vu8=", 0)]
        public void SetWithBase64(string key, string value, int expectedReturnCode)
        {
            Assert.Equal(expectedReturnCode, _command.Set(key, value, InputOutputFormat.Base64));
            using var db = _rocksDbService.Load();
            Assert.Equal(Convert.FromBase64String(value), db.Get(Convert.FromBase64String(key)));
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
