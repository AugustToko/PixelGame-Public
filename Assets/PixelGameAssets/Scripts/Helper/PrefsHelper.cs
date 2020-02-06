using System;

namespace PixelGameAssets.Scripts.Helper
{
    public static class PrefsHelper
    {
  
        /// <summary>
        /// 持久化存储标签
        /// </summary>
        public class Tag
        {
            public const string IsFirstEnterGame = "IsFirstEnterGame";
        }
        
        public class GameSettings
        {
            public const string MusicVolume = "MusicVolume";
            public const string VoiceVolume = "VoiceVolume";
            public const string FxVolume = "FxVolume";

            public const string RenderQuality = "RenderQuality";
            public const string LightQuality = "LightQuality";
        }
    }
}