namespace RocksDbTool.Tests.Commands
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using RocksDbSharp;
    using RocksDbTool.Commands;
    using RocksDbTool.Formats;
    using RocksDbTool.Services;
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

            _rocksDbService = new RocksDbService();
            SetupRocksDb(_temporaryDirectory, new Dictionary<string, string>
            {
                ["string"] = "foo",
            });
            SetupRocksDb(_temporaryDirectory, new Dictionary<byte[], byte[]>
            {
                [new byte[] { 0xde, 0xad }] = new byte[] { 0xbe, 0xef },
            });
            SetupRocksDb(
                _temporaryDirectory,
                new Dictionary<string, string>
                {
                    ["foo"] = "bar",
                },
                "column-family");

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
        [InlineData(InputFormat.Base64, OutputFormat.Base64, "3q0=", 0, "vu8=")]
        [InlineData(InputFormat.Base64, OutputFormat.Hex, "3q0=", 0, "beef")]
        [InlineData(InputFormat.String, OutputFormat.Base64, "string", 0, "Zm9v")]
        [InlineData(InputFormat.String, OutputFormat.Hex, "string", 0, "666f6f")]
        [InlineData(InputFormat.Hex, OutputFormat.Base64, "737472696e67", 0, "Zm9v")]
        [InlineData(InputFormat.Hex, OutputFormat.Hex, "737472696e67", 0, "666f6f")]
        [InlineData(InputFormat.String, OutputFormat.Base64, "unknown", -1, "")]
        [InlineData(InputFormat.Base64, OutputFormat.Base64, "AA==", -1, "")]
        public void Get(InputFormat inputFormat, OutputFormat outputFormat, string key, int expectedReturnCode, string expectedOutput)
        {
            Assert.Equal(expectedReturnCode, _command.Get(key, inputFormat, outputFormat, _temporaryDirectory));  // key: binary
            Assert.Equal(expectedOutput, _stringInputOutputErrorContainer.Out.ToString());
        }

        [Theory]
        [InlineData("foo", "bar", 0)]
        public void SetWithString(string key, string value, int expectedReturnCode)
        {
            Assert.Equal(expectedReturnCode, _command.Set(key, value, InputFormat.String, _temporaryDirectory));
            using var db = _rocksDbService.Load(_temporaryDirectory);
            Assert.Equal(value, db.Get(key));
        }

        [Theory]
        [InlineData("3q0=", "vu8=", 0)]
        public void SetWithBase64(string key, string value, int expectedReturnCode)
        {
            Assert.Equal(expectedReturnCode, _command.Set(key, value, InputFormat.Base64, _temporaryDirectory));
            using var db = _rocksDbService.Load(_temporaryDirectory);
            Assert.Equal(Convert.FromBase64String(value), db.Get(Convert.FromBase64String(key)));
        }

        [Theory]
        [InlineData(InputFormat.Base64, OutputFormat.Hex, "", "737472696e67\t666f6f\n" + "dead\tbeef\n")]
        [InlineData(InputFormat.Base64, OutputFormat.Base64, "", "c3RyaW5n\tZm9v\n" + "3q0=\tvu8=\n")]
        [InlineData(InputFormat.String, OutputFormat.Hex, "", "737472696e67\t666f6f\n" + "dead\tbeef\n")]
        [InlineData(InputFormat.String, OutputFormat.Base64, "", "c3RyaW5n\tZm9v\n" + "3q0=\tvu8=\n")]
        [InlineData(InputFormat.Hex, OutputFormat.Hex, "", "737472696e67\t666f6f\n" + "dead\tbeef\n")]
        [InlineData(InputFormat.Hex, OutputFormat.Base64, "", "c3RyaW5n\tZm9v\n" + "3q0=\tvu8=\n")]
        [InlineData(InputFormat.Base64, OutputFormat.Hex, "3g==", "dead\tbeef\n")]
        [InlineData(InputFormat.Base64, OutputFormat.Base64, "3g==", "3q0=\tvu8=\n")]
        [InlineData(InputFormat.Base64, OutputFormat.Hex, "3q0=", "dead\tbeef\n")]
        [InlineData(InputFormat.Base64, OutputFormat.Base64, "3q0=", "3q0=\tvu8=\n")]
        [InlineData(InputFormat.Base64, OutputFormat.Hex, "dW5rbm93bi1wcmVmaXg=", "")]
        [InlineData(InputFormat.Base64, OutputFormat.Base64, "dW5rbm93bi1wcmVmaXg=", "")]
        [InlineData(InputFormat.String, OutputFormat.Hex, "s", "737472696e67\t666f6f\n")]
        [InlineData(InputFormat.String, OutputFormat.Base64, "s", "c3RyaW5n\tZm9v\n")]
        [InlineData(InputFormat.String, OutputFormat.Hex, "str", "737472696e67\t666f6f\n")]
        [InlineData(InputFormat.String, OutputFormat.Base64, "str", "c3RyaW5n\tZm9v\n")]
        [InlineData(InputFormat.String, OutputFormat.Hex, "unknown-prefix", "")]
        [InlineData(InputFormat.String, OutputFormat.Base64, "unknown-prefix", "")]
        [InlineData(InputFormat.Hex, OutputFormat.Hex, "73", "737472696e67\t666f6f\n")]
        [InlineData(InputFormat.Hex, OutputFormat.Base64, "73", "c3RyaW5n\tZm9v\n")]
        [InlineData(InputFormat.Hex, OutputFormat.Hex, "737472", "737472696e67\t666f6f\n")]
        [InlineData(InputFormat.Hex, OutputFormat.Base64, "737472", "c3RyaW5n\tZm9v\n")]
        [InlineData(InputFormat.Hex, OutputFormat.Hex, "756e6b6e6f776e2d707265666978", "")]
        [InlineData(InputFormat.Hex, OutputFormat.Hex, "756E6B6E6F776E2D707265666978", "")]
        [InlineData(InputFormat.Hex, OutputFormat.Base64, "756e6b6e6f776e2d707265666978", "")]
        [InlineData(InputFormat.Hex, OutputFormat.Base64, "756E6B6E6F776E2D707265666978", "")]
        [InlineData(InputFormat.Hex, OutputFormat.Hex, "de", "dead\tbeef\n")]
        [InlineData(InputFormat.Hex, OutputFormat.Hex, "De", "dead\tbeef\n")]
        [InlineData(InputFormat.Hex, OutputFormat.Hex, "DE", "dead\tbeef\n")]
        [InlineData(InputFormat.Hex, OutputFormat.Base64, "de", "3q0=\tvu8=\n")]
        [InlineData(InputFormat.Hex, OutputFormat.Base64, "De", "3q0=\tvu8=\n")]
        [InlineData(InputFormat.Hex, OutputFormat.Base64, "DE", "3q0=\tvu8=\n")]
        [InlineData(InputFormat.Hex, OutputFormat.Hex, "dead", "dead\tbeef\n")]
        [InlineData(InputFormat.Hex, OutputFormat.Hex, "deAd", "dead\tbeef\n")]
        [InlineData(InputFormat.Hex, OutputFormat.Hex, "DEAD", "dead\tbeef\n")]
        [InlineData(InputFormat.Hex, OutputFormat.Base64, "dead", "3q0=\tvu8=\n")]
        [InlineData(InputFormat.Hex, OutputFormat.Base64, "dEad", "3q0=\tvu8=\n")]
        [InlineData(InputFormat.Hex, OutputFormat.Base64, "DEAD", "3q0=\tvu8=\n")]
        public void List(InputFormat inputFormat, OutputFormat outputFormat, string prefix, string expectedOutput)
        {
            _command.List(inputFormat: inputFormat, outputFormat: outputFormat, prefix: prefix, rocksdbPath: _temporaryDirectory);
            Assert.Equal(expectedOutput, _stringInputOutputErrorContainer.Out.ToString());
            Assert.Equal("Key\tValue\n", _stringInputOutputErrorContainer.Error.ToString());
        }

        [Theory]
        [InlineData(InputFormat.Base64, "bG9uZy1ieXRlcw==")] // "long-bytes"
        [InlineData(InputFormat.Hex, "6c6f6e672d6279746573")] // "long-bytes"
        [InlineData(InputFormat.String, "long-bytes")] // "long-bytes"
        [InlineData(InputFormat.Base64, "c3RyaW5n")] // "string"
        [InlineData(InputFormat.Hex, "737472696e67")] // "string"
        [InlineData(InputFormat.String, "string")] // "string"
        [InlineData(InputFormat.Base64, "3q0=")] // b"\xde\xad"
        [InlineData(InputFormat.Hex, "dead")] // b"\xde\xad"
        [InlineData(InputFormat.Base64, "")] // empty
        [InlineData(InputFormat.Hex, "")] // empty
        [InlineData(InputFormat.String, "")] // empty
        public void Remove(InputFormat inputFormat, string key)
        {
            _command.Remove(
                key: key,
                inputFormat: inputFormat,
                rocksdbPath: _temporaryDirectory);
            using var db = _rocksDbService.Load(_temporaryDirectory);
            Assert.Null(db.Get(inputFormat.Decode(key)));
        }

        private void SetupRocksDb(string path, IEnumerable<KeyValuePair<string, string>> pairs, string columnFamily = "default")
        {
            using var db = OpenRocksDb(path);
            var cf = CreateColumnFamilyIfMissing(db, columnFamily);
            foreach (var pair in pairs)
            {
                db.Put(
                    pair.Key,
                    pair.Value,
                    cf);
            }
        }

        private void SetupRocksDb(string path, IEnumerable<KeyValuePair<byte[], byte[]>> pairs, string columnFamily = "default")
        {
            using var db = OpenRocksDb(path);
            var cf = CreateColumnFamilyIfMissing(db, columnFamily);
            foreach (var pair in pairs)
            {
                db.Put(
                    pair.Key,
                    pair.Value,
                    cf);
            }
        }

        private ColumnFamilyHandle CreateColumnFamilyIfMissing(RocksDb db, string columnFamily)
        {
            try
            {
                return db.GetColumnFamily(columnFamily);
            }
            catch
            {
                var options = new ColumnFamilyOptions();
                return db.CreateColumnFamily(options, columnFamily);
            }
        }

        private RocksDb OpenRocksDb(string path)
        {
            var options = new DbOptions().SetCreateIfMissing();
            var db = RocksDb.Open(options, path);
            db.Dispose();

            return _rocksDbService.Load(path);
        }
    }
}
