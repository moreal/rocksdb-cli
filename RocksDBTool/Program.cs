namespace RocksDbTool
{
    using System.Threading.Tasks;
    using Cocona;
    using Microsoft.Extensions.DependencyInjection;
    using RocksDbTool.Services;

    /// <summary>
    /// The main class to run as entrypoint.
    /// </summary>
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            await CoconaApp.Create().ConfigureServices(
                services =>
                {
                    services.AddTransient<IFileConfigurationService<RocksDbServiceConfiguration>, RocksDbServiceFileConfigurationService>();
                    services.AddTransient<IRocksDbService, RocksDbService>();
                    services.AddTransient<IInputOutputErrorContainer, StandardInputOutputErrorContainer>();
                }).RunAsync<RocksDbCommand>(args);
        }
    }
}
