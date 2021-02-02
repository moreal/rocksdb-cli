namespace RocksDBTool
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Cocona;

    /// <summary>
    /// A class to handle <see cref="RocksDbSharp.RocksDb"/> in command line.
    /// </summary>
    public sealed class RocksDbCommand
    {
        private readonly IInputOutputErrorContainer _inputOutputErrorContainer;
        private readonly IRocksDbService _rocksDbService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RocksDbCommand"/> class.
        /// </summary>
        /// <param name="inputOutputErrorContainer">A container having writers to write output and error.</param>
        /// <param name="rocksDbService">A service to load <see cref="RocksDbSharp.RocksDb"/>.</param>
        public RocksDbCommand(IInputOutputErrorContainer inputOutputErrorContainer, IRocksDbService rocksDbService)
        {
            _inputOutputErrorContainer = inputOutputErrorContainer;
            _rocksDbService = rocksDbService;
        }

        /// <summary>
        /// Gets the value corresponded to <paramref name="key"/> from rocksdb.
        /// </summary>
        /// <param name="key">A key string following <paramref name="inputFormat"/>,
        /// to get from <see cref="RocksDbSharp.RocksDb"/> as a key.</param>
        /// <param name="inputFormat">An format of <paramref name="key"/>.
        /// If <paramref name="inputFormat"/> is <see cref="InputFormat.Base64"/>, <paramref name="key"/>
        /// should be base64 encoded value. If <paramref name="inputFormat"/> is
        /// <see cref="InputFormat.String"/>, it treats <paramref name="key"/> as <see cref="Encoding.UTF8"/> encoded
        /// string.</param>
        /// <param name="outputFormat">A format of output.</param>
        /// <param name="rocksdbPath">The path of <see cref="RocksDbSharp.RocksDb"/> to load.</param>
        /// <returns>If it successes, returns 0. If it fails, returns -1.</returns>
        public int Get([Argument] string key, [Option] InputFormat inputFormat = InputFormat.String, [Option] OutputFormat outputFormat = OutputFormat.Base64, [Option] string? rocksdbPath = null)
        {
            rocksdbPath ??= Directory.GetCurrentDirectory();
            try
            {
                using var db = _rocksDbService.Load(rocksdbPath);
                byte[] keyBytes = ConvertWithInputFormat(inputFormat, key);

                if (db.Get(keyBytes) is { } valueBytes)
                {
                    string value = ConvertWithOutputFormat(outputFormat, valueBytes);
                    _inputOutputErrorContainer.Out.Write(value);
                    return 0;
                }
            }
            catch (Exception e)
            {
                _inputOutputErrorContainer.Error.WriteLine(e.Message);
            }

            return -1;
        }

        /// <summary>
        /// Sets the value corresponded to <paramref name="key"/> from rocksdb.
        /// </summary>
        /// <param name="key">A key string following <paramref name="inputFormat"/>,
        /// to set into <see cref="RocksDbSharp.RocksDb"/> as a key.</param>
        /// <param name="value">A value following <paramref name="inputFormat"/>,
        /// to set into <see cref="RocksDbSharp.RocksDb"/> as a value.</param>
        /// <param name="inputFormat">An format of <paramref name="key"/> and <paramref name="value"/>.
        /// If <paramref name="inputFormat"/> is <see cref="InputFormat.Base64"/>, <paramref name="key"/>
        /// and <paramref name="value"/> should be base64 encoded value. If <paramref name="inputFormat"/> is
        /// <see cref="InputFormat.String"/>, it treats <paramref name="key"/> and <paramref name="value"/> as string.</param>
        /// <param name="rocksdbPath">The path of <see cref="RocksDbSharp.RocksDb"/> to load.</param>
        /// <returns>If it successes, returns 0. If it fails, returns -1.</returns>
        public int Set([Argument] string key, [Argument] string value, [Option] InputFormat inputFormat = InputFormat.String, [Option] string? rocksdbPath = null)
        {
            rocksdbPath ??= Directory.GetCurrentDirectory();
            try
            {
                using var db = _rocksDbService.Load(rocksdbPath);
                byte[] keyBytes = ConvertWithInputFormat(inputFormat, key);
                byte[] valueBytes = ConvertWithInputFormat(inputFormat, value);
                db.Put(keyBytes, valueBytes);
                return 0;
            }
            catch (Exception e)
            {
                _inputOutputErrorContainer.Error.WriteLine(e.Message);
            }

            return -1;
        }

        /// <summary>
        /// Prints keys and values from <see cref="RocksDbSharp.RocksDb"/> located at <paramref name="rocksdbPath"/>.
        /// </summary>
        /// <param name="prefix">The prefix of keys to filter.</param>
        /// <param name="inputFormat">An format of <paramref name="prefix"/>.
        /// For instance, if <paramref name="inputFormat"/> is <see cref="InputFormat.Base64"/>, <paramref name="prefix"/>
        /// should be base64 encoded value. If <paramref name="inputFormat"/> is <see cref="InputFormat.String"/>,
        /// it treats <paramref name="prefix"/> as <see cref="Encoding.UTF8"/> encoded string.</param>
        /// <param name="outputFormat">A format of output (i.e. keys and values).</param>
        /// <param name="rocksdbPath">The path of <see cref="RocksDbSharp.RocksDb"/> to load.</param>
        public void List(
            [Option] string prefix = "",
            [Option] InputFormat inputFormat = InputFormat.String,
            [Option] OutputFormat outputFormat = OutputFormat.Hex,
            [Option] string? rocksdbPath = null)
        {
            rocksdbPath ??= Directory.GetCurrentDirectory();
            try
            {
                using var db = _rocksDbService.Load(rocksdbPath);
                byte[] prefixBytes = ConvertWithInputFormat(inputFormat, prefix);

                _inputOutputErrorContainer.Error.WriteLine("Key\tValue");
                using var iterator = db.NewIterator();
                for (iterator.Seek(prefixBytes);
                    iterator.Valid() && iterator.Key().Take(prefixBytes.Length).SequenceEqual(prefixBytes);
                    iterator.Next())
                {
                    _inputOutputErrorContainer.Out.Write(ConvertWithOutputFormat(outputFormat, iterator.Key()));
                    _inputOutputErrorContainer.Out.Write('\t');
                    _inputOutputErrorContainer.Out.WriteLine(ConvertWithOutputFormat(outputFormat, iterator.Value()));
                }
            }
            catch (Exception e)
            {
                _inputOutputErrorContainer.Error.WriteLine(e.Message);
            }
        }

        private byte[] ConvertWithInputFormat(InputFormat inputFormat, string value) => inputFormat switch
        {
            InputFormat.String => Encoding.UTF8.GetBytes(value),
            InputFormat.Base64 => Convert.FromBase64String(value),
            InputFormat.Hex => ByteUtil.ParseHex(value),
            _ => throw new ArgumentException(nameof(value)),
        };

        private string ConvertWithOutputFormat(OutputFormat outputFormat, byte[] value) => outputFormat switch
        {
            OutputFormat.Base64 => Convert.ToBase64String(value),
            OutputFormat.Hex => ByteUtil.Hex(value),
            _ => throw new ArgumentException(nameof(value)),
        };
    }
}
