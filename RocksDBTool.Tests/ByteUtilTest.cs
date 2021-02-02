namespace RocksDBTool.Tests
{
    using System;
    using Xunit;

    public class ByteUtilTest
    {
        [Fact]
        public void Hex()
        {
            Assert.Equal(string.Empty, ByteUtil.Hex(new byte[] { }));
            Assert.Equal("beef", ByteUtil.Hex(new byte[] { 0xbe, 0xef }));
            Assert.Equal("00", ByteUtil.Hex(new byte[] { 0x00 }));
        }

        [Fact]
        public void ParseHex()
        {
            Assert.Equal(new byte[] { }, ByteUtil.ParseHex(string.Empty));
            Assert.Equal(new byte[] { 0xbe, 0xef }, ByteUtil.ParseHex("bEEF"));
            Assert.Equal(new byte[] { 0xbe, 0xef }, ByteUtil.ParseHex("beef"));
            Assert.Equal(new byte[] { 0xbe, 0xef }, ByteUtil.ParseHex("bEeF"));
            Assert.Equal(new byte[] { 0xbe, 0xef }, ByteUtil.ParseHex("BeeF"));
            Assert.Equal("beef", ByteUtil.Hex(new byte[] { 0xbe, 0xef }));
            Assert.Equal("00", ByteUtil.Hex(new byte[] { 0x00 }));
        }

        [Fact]
        public void ParseHexThrowsWhenOddStringPassed()
        {
            Assert.Throws<ArgumentException>(() => ByteUtil.ParseHex("1"));
            Assert.Throws<ArgumentException>(() => ByteUtil.ParseHex("123"));
        }
    }
}