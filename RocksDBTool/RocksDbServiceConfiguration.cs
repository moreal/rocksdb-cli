namespace RocksDbTool
{
    using System;
    using System.Text.Json.Serialization;

    /// <summary>
    /// A class to represent configuration of <see cref="Services.RocksDbService"/>.
    /// </summary>
    public sealed class RocksDbServiceConfiguration : IEquatable<RocksDbServiceConfiguration>
    {
        /// <summary>
        /// Gets or sets current <see cref="RocksDbSharp.RocksDb"/>  path.
        /// </summary>
        [JsonPropertyName("current_rocksdb_path")]
        public string? CurrentRocksDbPath { get; set; }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(CurrentRocksDbPath);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return (obj is RocksDbServiceConfiguration configuration) && Equals(configuration);
        }

        /// <inheritdoc />
        public bool Equals(RocksDbServiceConfiguration? other)
        {
            return !(other is null) && (this == other || this.GetHashCode() == other.GetHashCode());
        }
    }
}
