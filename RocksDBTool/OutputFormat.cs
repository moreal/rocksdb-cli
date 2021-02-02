namespace RocksDBTool
{
    using System;

    /// <summary>
    /// An enum class to represent what format to treat output variables as.
    /// </summary>
    public enum OutputFormat
    {
        /// <summary>
        /// A format to treat output variables as hexadecimal string.
        /// </summary>
        Hex,

        /// <summary>
        /// A format to treat output variables with <see cref="Convert.FromBase64String"/>
        /// and <see cref="Convert.ToBase64String(byte[])"/>.
        /// </summary>
        Base64,
    }
}
