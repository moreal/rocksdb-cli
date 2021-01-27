namespace RocksDBTool
{
    using System;
    using System.IO;

    /// <summary>
    /// A implementation of <see cref="IInputOutputErrorContainer"/> interface with standard IO
    /// (i.e. stdin, stdout and stderr.).
    /// </summary>
    public sealed class StandardInputOutputErrorContainer : IInputOutputErrorContainer
    {
        /// <inheritdoc cref="IInputOutputErrorContainer.Error"/>
        /// <seealso cref="Console.In"/>
        public TextReader In => Console.In;

        /// <inheritdoc cref="IInputOutputErrorContainer.Error"/>
        /// <seealso cref="Console.Out"/>
        public TextWriter Out => Console.Out;

        /// <inheritdoc cref="IInputOutputErrorContainer.Error"/>
        /// <seealso cref="Console.Error"/>
        public TextWriter Error => Console.Error;
    }
}
