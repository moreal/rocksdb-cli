using System;
using System.IO;
using Cocona;
using RocksDbSharp;

namespace RocksDBTool
{
    public class RocksDbCommand
    {
        private readonly IRocksDbService _rocksDbService;
        private readonly TextWriter _output;

        public RocksDbCommand(TextWriter output, IRocksDbService rocksDbService)
        {
            _output = output;
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
                            _output.Write(Convert.ToBase64String(bytesValue));   
                            return 0;
                        }
                        break;

                    case InputOutputFormat.String:
                        if (db.Get(key) is {} stringValue)
                        {
                            _output.Write(stringValue);   
                            return 0;
                        }
                        break;
                }
            }
            catch (RocksDbException e)
            {
                // FIXME: should be written into stderr instead of stdout.
                _output.WriteLine($"{nameof(RocksDbException)} occurred.");
                _output.WriteLine(e.Message);
            }
            catch (ArgumentNullException e)
            {
                // FIXME: should be written into stderr instead of stdout.
                _output.WriteLine("The configuration seems not configured yet. you ");
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
            catch (RocksDbException e)
            {
                // FIXME: should be written into stderr instead of stdout.
                _output.WriteLine($"{nameof(RocksDbException)} occurred.");
                _output.WriteLine(e.Message);
            }
            catch (ArgumentNullException e)
            {
                // FIXME: should be written into stderr instead of stdout.
                _output.WriteLine("The configuration seems not configured yet. you ");
            }

            return -1;
        }
    }
}
