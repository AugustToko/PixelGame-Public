using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace PixelGameAssets.Scripts.Misc
{
    [ExecuteInEditMode]
    public class ZOrder : MonoBehaviour
    {
        [SerializeField] private float m_floorHeight;
        private float m_spriteLowerBound;
        private float m_spriteHalfWidth;
        private readonly float m_tan30 = Mathf.Tan(Mathf.PI / 5);

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private void Start()
        {
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            m_spriteLowerBound = spriteRenderer.bounds.size.y * 0.5f;
            m_spriteHalfWidth = spriteRenderer.bounds.size.x * 0.5f;
        }

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private void LateUpdate()
        {
//            if (!Application.isPlaying)
//            {
                transform.position = new Vector3(
                    transform.position.x, transform.position.y,
                    (transform.position.y - m_spriteLowerBound + m_floorHeight) * m_tan30);
//            }
        }

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private void OnDrawGizmos()
        {
            Vector3 floor = new Vector3(
                transform.position.x, transform.position.y - m_spriteLowerBound + m_floorHeight, transform.position.z
            );
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(floor + Vector3.left * m_spriteHalfWidth, floor + Vector3.right * m_spriteHalfWidth);
        }
    }
}