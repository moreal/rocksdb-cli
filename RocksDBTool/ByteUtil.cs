namespace RocksDBTool
{
    using System;

    /// <summary>
    /// A static class having static methods related to hexadecimal conversion.
    /// </summary>
    public static class ByteUtil
    {
        /// <summary>
        /// Converts <paramref name="hex"/> into byte array.
        /// </summary>
        /// <param name="hex">A hexadecimal string to convert.</param>
        /// <returns>A byte array parsed from <paramref name="hex"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="hex"/> is odd length string.</exception>
        public static byte[] ParseHex(string hex)
        {
            if (hex.Length % 2 != 0)
            {
                throw new ArgumentException(nameof(hex));
            }

            byte[] buf = new byte[hex.Length / 2];

            for (int i = 0; i < hex.Length / 2; ++i)
            {
                buf[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }

            return buf;
        }

        /// <summary>
        /// Converts <paramref name="bytes"/> into hexadecimal string.
        /// </summary>
        /// <param name="bytes">The byte array to convert.</param>
        /// <returns>A hexadecimal string representation of <paramref name="bytes"/>.</returns>
        public static string Hex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace(
                "-",
                string.Empty).ToLowerInvariant();
        }
    }
}