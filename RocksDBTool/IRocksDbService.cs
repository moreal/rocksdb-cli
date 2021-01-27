namespace RocksDBTool
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
        /// <returns>A <see cref="RocksDb"/> instance.</returns>
        RocksDb Load();
    }
}
