namespace RocksDBTool
{
    using System;

    /// <summary>
    /// An enum class to represent what format to treat input and output variables as.
    /// </summary>
    public enum InputOutputFormat
    {
        /// <summary>
        /// A format to treat input and output variables as <see cref="string"/>.
        /// </summary>
        String,

        /// <summary>
        /// A format to treat input and output variables with <see cref="Convert.FromBase64String"/>
        /// and <see cref="Convert.ToBase64String(byte[])"/>.
        /// </summary>
        Base64,
    }
}
