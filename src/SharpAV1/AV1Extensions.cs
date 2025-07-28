namespace SharpAV1
{
    public static class AV1Extensions
    {
        public static (uint Width, uint Height) CalculateDimensions(this AV1Context context)
        {
            return ((uint)context._MaxFrameWidthMinus1 + 1, (uint)context._MaxFrameHeightMinus1 + 1);
        }
    }
}
