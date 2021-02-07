namespace RocksDbTool.Services
{
    using RocksDbSharp;

    /// <summary>
    /// An interface to load <see cref="RocksDb"/>.
    /// </summary>
    public interface IRocksDbService
    {
        /// <summary>
        /// Loads <see cref="RocksDb"/> instance.
        /// </summary>
        /// <param name="path">The path of <see cref="RocksDb"/>.</param>
        /// <returns>A <see cref="RocksDb"/> instance.</returns>
        RocksDb Load(string path);
    }
}
