namespace PixelGameAssets.Scripts.GKUtils
{
    public static class GkLog
    {
        public static void Debug(string tag, object content)
        {
            UnityEngine.Debug.Log(tag + " :" + content);
        }
    }
}