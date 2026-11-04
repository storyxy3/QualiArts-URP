namespace UnityEngine.Rendering.Universal
{
    public static class ShaderTimeManager
    {
        private static float _customTime = -1.0f;
        public static float CurrentTime => _customTime < 0.0f ? Time.time : _customTime;
        public static void SetCustomTime(float time) => _customTime = time;
        public static void ResetCustomTime() => _customTime = -1.0f;
    }
}
