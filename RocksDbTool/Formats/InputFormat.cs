namespace RocksDbTool.Formats
{
    using System;

    /// <summary>
    /// An enum class to represent what format to treat input variables as.
    /// </summary>
    public enum InputFormat
    {
        /// <summary>
        /// A format to treat input variables as <see cref="string"/>.
        /// </summary>
        String,

        /// <summary>
        /// A format to treat input variables as hexadecimal string.
        /// </summary>
        Hex,

        /// <summary>
        /// A format to treat input and output variables with <see cref="Convert.FromBase64String"/>
        /// and <see cref="Convert.ToBase64String(byte[])"/>.
        /// </summary>
        Base64,
    }
}
