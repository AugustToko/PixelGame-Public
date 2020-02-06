using PixelGameAssets.Scripts.Damage;
using UnityEngine;

namespace PixelGameAssets.Scripts.SkillSystem.Obj
{
    /// <summary>
    /// 技能释放出的实体
    /// </summary>
    public class SkillEntity : MonoBehaviour
    {
        // 弹丸的拥有者
        [HideInInspector] public Health owner;
        
        // 层遮罩
        [Header("Layers")]
        // 固态层 (墙壁)
        public LayerMask solid_layer;
        
        // 实体层 (Actor等)
        public LayerMask entities_layer;

    }
}