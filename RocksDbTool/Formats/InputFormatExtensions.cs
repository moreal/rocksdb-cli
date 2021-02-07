namespace RocksDbTool.Formats
{
    using System;
    using System.Text;

    /// <summary>
    /// A static class containing extension methods related to <see cref="InputFormat"/>.
    /// </summary>
    public static class InputFormatExtensions
    {
        /// <summary>
        /// Decodes <paramref name="value"/> by <paramref name="inputFormat"/>.
        /// </summary>
        /// <param name="inputFormat">An <see cref="InputFormat"/> to decode <paramref name="value"/>.</param>
        /// <param name="value">A value to decode.</param>
        /// <returns>A decoded value from the given <paramref name="value"/> by <paramref name="inputFormat"/>.
        /// </returns>
        /// <seealso cref="InputFormat"/>
        public static byte[] Decode(this InputFormat inputFormat, string value) => inputFormat switch
        {
            InputFormat.String => Encoding.UTF8.GetBytes(value),
            InputFormat.Base64 => Convert.FromBase64String(value),
            InputFormat.Hex => ByteUtil.ParseHex(value),
            _ => throw new ArgumentException(nameof(value)),
        };
    }
}
