namespace RocksDbTool.Services
{
    /// <summary>
    /// A class which load and store configuration on a file.
    /// </summary>
    /// <typeparam name="T">A configuration type.</typeparam>
    public interface IFileConfigurationService<T>
        where T : class, new()
    {
        /// <summary>
        /// Gets the path of the configuration file.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Loads the json configuration.
        /// </summary>
        /// <returns>An instance of <typeparamref name="T"/>.</returns>
        T Load();

        /// <summary>
        /// Stores the <paramref name="configuration"/> into file.
        /// </summary>
        /// <param name="configuration">The configuration to store.</param>
        void Store(T configuration);
    }
}
