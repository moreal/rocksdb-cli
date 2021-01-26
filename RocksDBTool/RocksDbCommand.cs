using System;
using System.IO;
using Cocona;
using RocksDbSharp;

namespace RocksDBTool
{
    public class RocksDbCommand
    {
        private readonly IInputOutputErrorContainer _inputOutputErrorContainer;
        private readonly IRocksDbService _rocksDbService;

        public RocksDbCommand(IInputOutputErrorContainer inputOutputErrorContainer, IRocksDbService rocksDbService)
        {
            _inputOutputErrorContainer = inputOutputErrorContainer;
            _rocksDbService = rocksDbService;
        }

        public int Get([Argument] InputOutputFormat inputOutputFormat, [Argument] string key)
        {
            try
            {
                using var db = _rocksDbService.Load();
                switch (inputOutputFormat)
                {
                    case InputOutputFormat.Base64:
                        if (db.Get(Convert.FromBase64String(key)) is {} bytesValue)
                        {
                            _inputOutputErrorContainer.Out.Write(Convert.ToBase64String(bytesValue));   
                            return 0;
                        }
                        break;

                    case InputOutputFormat.String:
                        if (db.Get(key) is {} stringValue)
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

        public int Set([Argument] InputOutputFormat inputOutputFormat, [Argument] string key, [Argument] string value)
        {
            try
            {
                using var db = _rocksDbService.Load();
                switch (inputOutputFormat)
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
