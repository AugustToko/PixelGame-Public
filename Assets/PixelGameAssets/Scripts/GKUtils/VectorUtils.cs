using UnityEngine;

namespace PixelGameAssets.Scripts.GKUtils
{
    public static class VectorUtils
    {
        /// <summary>
        /// 获取两点之间的一个点,在方法中进行了向量的减法，以及乘法运算
        /// </summary>
        /// <param name="start">起始点</param>
        /// <param name="end">结束点</param>
        /// <param name="distance">距离</param>
        /// <returns>求到的点</returns>
        public static Vector3 GetBetweenPoint(Vector3 start, Vector3 end, float distance)
        {
            var normal = (end - start).normalized;
            return normal * distance + start;
        }
    }
}