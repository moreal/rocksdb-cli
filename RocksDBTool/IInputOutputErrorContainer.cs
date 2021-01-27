namespace RocksDBTool
{
    using System.IO;

    /// <summary>
    /// A container class to treat input, output and error as a set.
    /// </summary>
    public interface IInputOutputErrorContainer
    {
        /// <summary>
        /// Gets a reader to read from a stream (e.g. stdin).
        /// </summary>
        TextReader In { get; }

        /// <summary>
        /// Gets a writer to write into a stream for output (e.g. stdout).
        /// </summary>
        TextWriter Out { get; }

        /// <summary>
        /// Gets a writer to write into a stream for error (e.g. stderr).
        /// </summary>
        TextWriter Error { get; }
    }
}
