namespace SharpMp4.Tests
{
    [TestClass]
    public class H265Tests
    {
        // TODO: investigate the additional bits at the end
        //[DataRow("40010C01FFFF01600000030080000003000003007BAC0901")]
        [DataRow(  "40010C01FFFF01600000030080000003000003007BAC09")]
        [TestMethod]
        public void ParseVPS(string input)
        {
            var vps = H265VpsNalUnit.Parse(Utilities.FromHexString(input));
            string output = Utilities.ToHexString(H265VpsNalUnit.Build(vps));
            Assert.AreEqual(input, output);
        }

        //[DataRow("42010101600000030080000003000003007BA003C08010E7F8DAEED4DDDC97580B70101010400000FA000009C41C877B944001E9F80002273D0003D3F000044E7A200002")]
        [DataRow(  "42010101600000030080000003000003007BA003C08010E7F8DAEED4DDDC97580B70101010400000FA000009C41C877B944001E9F80002273D0003D3F000044E7A20")]
        [TestMethod]
        public void ParseSPS(string input)
        {
            var sps = H265SpsNalUnit.Parse(Utilities.FromHexString(input));
            string output = Utilities.ToHexString(H265SpsNalUnit.Build(sps));
            Assert.AreEqual(input, output);
        }

        //[DataRow("4401C172B09C140A62400002")]
        [DataRow(  "4401C172B09C140A6240")]
        [TestMethod]
        public void ParsePPS(string input)
        {
            var pps = H265PpsNalUnit.Parse(Utilities.FromHexString(input));
            string output = Utilities.ToHexString(H265PpsNalUnit.Build(pps));
            Assert.AreEqual(input, output);
        }
    }
}
