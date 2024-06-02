namespace SharpMp4.Tests
{
    [TestClass]
    public class H264Tests
    {
        [DataRow("67420029E2900800A3602DC040406907891150")]
        [DataRow("67640028ACB402802DC8")]
        [DataRow("6742C01592440F047F5808800000030080000018478B1750")]
        [DataRow("674D0028E900F0044FCB0800001F480004E34020")]
        [DataRow("2742E01EA9181405FF2E00D418041ADB0AD7BDF010")]
        [DataRow("674D40208D8D402802DFF80B7010101400000FA00001D4C3A18003D3F00004C4B4BBCB8D0C001E9F80002625A5DE5C28")]
        [TestMethod]
        public void ParseSPS(string input)
        {
            var sps = H264SpsNalUnit.Parse(Utilities.FromHexString(input));
            string output = Utilities.ToHexString(H264SpsNalUnit.Build(sps));
            Assert.AreEqual(input, output);
        }

        [DataRow("68CE3C80")]
        [DataRow("68EE019E2C")]
        // TODO: Investigate the zero padding
        //[DataRow("68EE019E2C00")]
        [DataRow("68CE32C8")]
        [DataRow("68EA8F20")]
        [DataRow("28DE09C8")]
        [DataRow("68EE3880")]
        [TestMethod]
        public void ParsePPS(string input)
        {
            var pps = H264PpsNalUnit.Parse(Utilities.FromHexString(input));
            string output = Utilities.ToHexString(H264PpsNalUnit.Build(pps));
            Assert.AreEqual(input, output);
        }
    }
}