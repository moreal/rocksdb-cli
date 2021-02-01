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
        /// <param name="key">A key string following <paramref name="format"/>,
        /// to get from <see cref="RocksDbSharp.RocksDb"/> as a key.</param>
        /// <param name="format">An format of <paramref name="key"/>.
        /// If <paramref name="format"/> is <see cref="InputOutputFormat.Base64"/>, <paramref name="key"/>
        /// should be base64 encoded value. If <paramref name="format"/> is
        /// <see cref="InputOutputFormat.String"/>, it treats <paramref name="key"/> as string.</param>
        /// <param name="rocksdbPath">The path of <see cref="RocksDbSharp.RocksDb"/> to load.</param>
        /// <returns>If it successes, returns 0. If it fails, returns -1.</returns>
        public int Get([Argument] string key, [Option] InputOutputFormat format = InputOutputFormat.String, [Option] string? rocksdbPath = null)
        {
            rocksdbPath ??= Directory.GetCurrentDirectory();
            try
            {
                using var db = _rocksDbService.Load(rocksdbPath);
                switch (format)
                {
                    case InputOutputFormat.Base64:
                        if (db.Get(Convert.FromBase64String(key)) is { } bytesValue)
                        {
                            _inputOutputErrorContainer.Out.Write(Convert.ToBase64String(bytesValue));
                            return 0;
                        }

                        break;

                    case InputOutputFormat.String:
                        if (db.Get(key) is { } stringValue)
                        {
                            _inputOutputErrorContainer.Out.Write(stringValue);
                            return 0;
                        }

                        break;
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
        /// <param name="key">A key string following <paramref name="format"/>,
        /// to set into <see cref="RocksDbSharp.RocksDb"/> as a key.</param>
        /// <param name="value">A value following <paramref name="format"/>,
        /// to set into <see cref="RocksDbSharp.RocksDb"/> as a value.</param>
        /// <param name="format">An format of <paramref name="key"/> and <paramref name="value"/>.
        /// If <paramref name="format"/> is <see cref="InputOutputFormat.Base64"/>, <paramref name="key"/>
        /// and <paramref name="value"/> should be base64 encoded value. If <paramref name="format"/> is
        /// <see cref="InputOutputFormat.String"/>, it treats <paramref name="key"/> and <paramref name="value"/> as string.</param>
        /// <param name="rocksdbPath">The path of <see cref="RocksDbSharp.RocksDb"/> to load.</param>
        /// <returns>If it successes, returns 0. If it fails, returns -1.</returns>
        public int Set([Argument] string key, [Argument] string value, [Option] InputOutputFormat format = InputOutputFormat.String, [Option] string? rocksdbPath = null)
        {
            rocksdbPath ??= Directory.GetCurrentDirectory();
            try
            {
                using var db = _rocksDbService.Load(rocksdbPath);
                switch (format)
                {
                    case InputOutputFormat.Base64:
                        db.Put(Convert.FromBase64String(key), Convert.FromBase64String(value));
                        break;
                    case InputOutputFormat.String:
                        db.Put(key, value);
                        break;
                }

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
        /// <param name="format">An format of <paramref name="prefix"/>, keys and values.
        /// For instance, if <paramref name="format"/> is <see cref="InputOutputFormat.Base64"/>, <paramref name="prefix"/>
        /// should be base64 encoded value. If <paramref name="format"/> is <see cref="InputOutputFormat.String"/>,
        /// it treats <paramref name="prefix"/> as string.</param>
        /// <param name="rocksdbPath">The path of <see cref="RocksDbSharp.RocksDb"/> to load.</param>
        public void List(
            [Option] string prefix = "",
            [Option] InputOutputFormat format = InputOutputFormat.String,
            [Option] string? rocksdbPath = null)
        {
            rocksdbPath ??= Directory.GetCurrentDirectory();
            try
            {
                using var db = _rocksDbService.Load(rocksdbPath);
                byte[] prefixBytes = format switch
                {
                    InputOutputFormat.Base64 => Convert.FromBase64String(prefix),
                    InputOutputFormat.String => Encoding.UTF8.GetBytes(prefix),
                    _ => throw new ArgumentException(nameof(format)),
                };

                _inputOutputErrorContainer.Error.WriteLine("Key\tValue");
                using var iterator = db.NewIterator();
                for (iterator.Seek(prefixBytes);
                    iterator.Valid() && iterator.Key().Take(prefixBytes.Length).SequenceEqual(prefixBytes);
                    iterator.Next())
                {
                    switch (format)
                    {
                        case InputOutputFormat.Base64:
                            _inputOutputErrorContainer.Out.Write(Convert.ToBase64String(iterator.Key()));
                            _inputOutputErrorContainer.Out.Write('\t');
                            _inputOutputErrorContainer.Out.WriteLine(Convert.ToBase64String(iterator.Value()));
                            break;
                        case InputOutputFormat.String:
                            _inputOutputErrorContainer.Out.Write(iterator.StringKey());
                            _inputOutputErrorContainer.Out.Write('\t');
                            _inputOutputErrorContainer.Out.WriteLine(iterator.StringValue());
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                _inputOutputErrorContainer.Error.WriteLine(e.Message);
            }
        }
    }
}
