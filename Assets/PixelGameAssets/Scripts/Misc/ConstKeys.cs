using UnityEngine;

namespace PixelGameAssets.Scripts.Misc
{
    public static class ConstKeys
    {
        /// <summary>
        /// for <see cref="PlayerPrefs"/>
        /// 是否进入过教程, string {"true", "false"}
        /// </summary>
        public const string HasEnterTutorial = "HasEnterTutorial";

        /// <summary>
        /// for <see cref="PlayerPrefs"/>
        /// 是否看过 OP 1, string {"true", "false"}
        /// </summary>
        public const string HasEnterVideoOp1 = "HasEnterVideoOp1";

        /// <summary>
        /// for <see cref="PlayerPrefs"/>
        /// 最高分
        /// </HasEnterTutorial
        public const string HIGHSCORE = "HIGHSCORE";
    }
}