namespace RocksDBTool
{
    using System;
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
        /// <param name="key">A key string following <paramref name="format"/>.
        /// If <paramref name="format"/> is <see cref="InputOutputFormat.Base64"/>, <paramref name="key"/>
        /// should be base64 encoded value. If <paramref name="format"/> is
        /// <see cref="InputOutputFormat.String"/>, it treats <paramref name="key"/> as string.</param>
        /// <param name="format">An format of <paramref name="key"/>.
        /// If <paramref name="format"/> is <see cref="InputOutputFormat.Base64"/>, <paramref name="key"/>
        /// should be base64 encoded value. If <paramref name="format"/> is
        /// <see cref="InputOutputFormat.String"/>, it treats <paramref name="key"/> as string.</param>
        /// <returns>If it successes, returns 0. If it fails, returns -1.</returns>
        public int Get([Argument] string key, [Option] InputOutputFormat format = InputOutputFormat.String)
        {
            try
            {
                using var db = _rocksDbService.Load();
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
        /// to set into <see cref="RocksDbSharp.RocksDb"/> as key.</param>
        /// <param name="value">A value following <paramref name="format"/>,
        /// to set into <see cref="RocksDbSharp.RocksDb"/> as value.</param>
        /// <param name="format">An format of <paramref name="key"/> and <paramref name="value"/>.
        /// If <paramref name="format"/> is <see cref="InputOutputFormat.Base64"/>, <paramref name="key"/>
        /// and <paramref name="value"/> should be base64 encoded value. If <paramref name="format"/> is
        /// <see cref="InputOutputFormat.String"/>, it treats <paramref name="key"/> and <paramref name="value"/> as string.</param>
        /// <returns>If it successes, returns 0. If it fails, returns -1.</returns>
        public int Set([Argument] string key, [Argument] string value, [Argument] InputOutputFormat format = InputOutputFormat.String)
        {
            try
            {
                using var db = _rocksDbService.Load();
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
    }
}
