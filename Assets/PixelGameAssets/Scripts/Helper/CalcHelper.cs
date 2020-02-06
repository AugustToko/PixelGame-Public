using UnityEngine;

namespace PixelGameAssets.Scripts.Helper
{
    public static class CalcHelper
    {
        public static float Approach(float start, float end, float shift)
        {
            if (start < end)
                return Mathf.Min(start + shift, end);
            return Mathf.Max(start - shift, end);
        }

        public static Vector2 RadianToVector2(float radian)
        {
            return new Vector2(Mathf.Cos(radian), Mathf.Sin(radian));
        }

        public static Vector2 DegreeToVector2(float degree)
        {
            return RadianToVector2(degree * Mathf.Deg2Rad);
        }

        public static float Vector2ToDegree(Vector2 angleVector)
        {
            //return (float)Mathf.Atan2(angleVector.x, -angleVector.y);

            var value = Mathf.Atan2(angleVector.x, angleVector.y) / Mathf.PI * 180f;
            if (value < 0) value += 360f;

            return value;
        }
    }
}