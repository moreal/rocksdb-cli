using System;
using System.IO;
using Cocona;

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

        public int Get([Argument] string key)
        {
            throw new NotImplementedException();
        }

        public int Set([Argument] string key, [Argument] string value)
        {
            throw new NotImplementedException();
        }
    }
}
