#nullable enable
using System;
using System.IO;
using System.Threading.Tasks;
using Cocona;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RocksDBTool
{
    internal static class Program
    {
        static async Task Main(string[] args)
        {
            await CoconaApp.Create().ConfigureServices(
                services =>
                {
                    services.AddTransient<IRocksDbService, RocksDbService>();
                    services.AddTransient<IInputOutputErrorContainer, StandardInputOutputErrorContainer>();
                }).ConfigureAppConfiguration(
                (context, builder) =>
                {
                    var path = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                        ".config",
                        "rocksdb-tool.json");
                    builder.AddJsonFile(path, true);
                }).RunAsync<RocksDbCommand>(args);
        }
    }
}
