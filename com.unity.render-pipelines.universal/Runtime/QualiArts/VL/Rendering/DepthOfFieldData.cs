namespace VL.Rendering
{
    public struct DepthOfFieldData
    {
        public float backgroundRange;
        public float foregroundRange;
        public bool infinityBackground => backgroundRange < 0f;
        public bool infinityForeground => foregroundRange < 0f;
        public float range => 0f + (infinityBackground ? 0f : backgroundRange) + (infinityForeground ? 0f : foregroundRange);
        public DepthOfFieldData(float targetRange, float focalLength, float fNumber)
        {
            double distance = targetRange * 1000.0;
            double focalSquared = (double)focalLength * focalLength;
            double aperture = distance * (fNumber * 0.03);
            backgroundRange = (float)(distance * aperture / (focalSquared - aperture) / 1000.0);
            foregroundRange = (float)(distance * aperture / (focalSquared + aperture) / 1000.0);
        }
    }
}
