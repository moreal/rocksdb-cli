namespace RocksDbTool.Formats
{
    using System;

    /// <summary>
    /// A static class containing extension methods related to <see cref="OutputFormat"/>.
    /// </summary>
    public static class OutputFormatExtensions
    {
        /// <summary>
        /// Decodes <paramref name="value"/> by <paramref name="outputFormat"/>.
        /// </summary>
        /// <param name="outputFormat">An <see cref="OutputFormat"/> to encode <paramref name="value"/>.</param>
        /// <param name="value">A value to encode.</param>
        /// <returns>A encoded value from the given <paramref name="value"/> by <paramref name="outputFormat"/>.
        /// </returns>
        /// <seealso cref="OutputFormat"/>
        public static string Encode(this OutputFormat outputFormat, byte[] value) => outputFormat switch
        {
            OutputFormat.Base64 => Convert.ToBase64String(value),
            OutputFormat.Hex => ByteUtil.Hex(value),
            _ => throw new ArgumentException(nameof(value)),
        };
    }
}
