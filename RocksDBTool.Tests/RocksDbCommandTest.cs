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

        private readonly string _temporaryDirectory;

        public RocksDbCommandTest()
        {
            _temporaryDirectory = Path.Combine(
                Path.GetTempPath(),
                Guid.NewGuid().ToString());
            SetupRocksDb(_temporaryDirectory, new Dictionary<string, string>
            {
                ["string"] = "foo",
            });
            SetupRocksDb(_temporaryDirectory, new Dictionary<byte[], byte[]>
            {
                [new byte[] { 0xde, 0xad }] = new byte[] { 0xbe, 0xef },
            });

            _rocksDbService = new RocksDbService();
            _stringInputOutputErrorContainer = new StringInputOutputErrorContainer(
                new StringReader(string.Empty),
                new StringWriter(),
                new StringWriter());
            _command = new RocksDbCommand(
                _stringInputOutputErrorContainer,
                _rocksDbService);

            _stringInputOutputErrorContainer.SetNewLines();
        }

        [Theory]
        [InlineData(InputOutputFormat.Base64, "3q0=", 0, "vu8=")]
        [InlineData(InputOutputFormat.String, "string", 0, "foo")]
        [InlineData(InputOutputFormat.String, "unknown", -1, "")]
        [InlineData(InputOutputFormat.Base64, "AA==", -1, "")]
        public void Get(InputOutputFormat inputOutputFormat, string key, int expectedReturnCode, string expectedOutput)
        {
            Assert.Equal(expectedReturnCode, _command.Get(key, inputOutputFormat, _temporaryDirectory));  // key: binary
            Assert.Equal(expectedOutput, _stringInputOutputErrorContainer.Out.ToString());
        }

        [Theory]
        [InlineData("foo", "bar", 0)]
        public void SetWithString(string key, string value, int expectedReturnCode)
        {
            Assert.Equal(expectedReturnCode, _command.Set(key, value, InputOutputFormat.String, _temporaryDirectory));
            using var db = _rocksDbService.Load(_temporaryDirectory);
            Assert.Equal(value, db.Get(key));
        }

        [Theory]
        [InlineData("3q0=", "vu8=", 0)]
        public void SetWithBase64(string key, string value, int expectedReturnCode)
        {
            Assert.Equal(expectedReturnCode, _command.Set(key, value, InputOutputFormat.Base64, _temporaryDirectory));
            using var db = _rocksDbService.Load(_temporaryDirectory);
            Assert.Equal(Convert.FromBase64String(value), db.Get(Convert.FromBase64String(key)));
        }

        [Theory]
        [InlineData(InputOutputFormat.Base64, "c3RyaW5n\tZm9v\n" + "3q0=\tvu8=\n")]
        [InlineData(InputOutputFormat.String, "string\tfoo\n" + "\xde\xad\t\xbe\xef\n")]
        public void List(InputOutputFormat format, string expectedOutput)
        {
            _command.List(format: format, rocksdbPath: _temporaryDirectory);
            Assert.Equal(expectedOutput, _stringInputOutputErrorContainer.Out.ToString());
            Assert.Equal("Key\tValue\n", _stringInputOutputErrorContainer.Error.ToString());
        }

        [Theory]
        [InlineData(InputOutputFormat.Base64, "3g==", "3q0=\tvu8=\n")]
        [InlineData(InputOutputFormat.Base64, "3q0=", "3q0=\tvu8=\n")]
        [InlineData(InputOutputFormat.Base64, "dW5rbm93bi1wcmVmaXg=", "")]
        [InlineData(InputOutputFormat.String, "s", "string\tfoo\n")]
        [InlineData(InputOutputFormat.String, "str", "string\tfoo\n")]
        [InlineData(InputOutputFormat.String, "unknown-prefix", "")]
        public void ListWithPrefix(InputOutputFormat format, string prefix, string expectedOutput)
        {
            _command.List(prefix: prefix, format: format, rocksdbPath: _temporaryDirectory);
            Assert.Equal(expectedOutput, _stringInputOutputErrorContainer.Out.ToString());
            Assert.Equal("Key\tValue\n", _stringInputOutputErrorContainer.Error.ToString());
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
