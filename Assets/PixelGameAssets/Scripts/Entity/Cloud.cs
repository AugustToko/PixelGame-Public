using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PixelGameAssets.Scripts.Entity
{
    /// <summary>
    /// Cloud 云
    /// </summary>
    public class Cloud : MonoBehaviour
    {
        private UnityEngine.Camera MainCamera { get; set; }

        public float speed = 2;

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private void Awake()
        {
            MainCamera = UnityEngine.Camera.main;
            if (MainCamera != null)
            {
                var pos = MainCamera.ViewportToWorldPoint(new Vector3(Random.Range(-1, 0), Random.Range(0, 1f)));
                transform.position = new Vector3(pos.x, pos.y, 0);
            }

            transform.localScale = new Vector3(4, 4, 0);
            GetComponentInChildren<SpriteRenderer>().sortingOrder = -100;
        }

        [SuppressMessage("ReSharper", "Unity.InefficientPropertyAccess")]
        private void Update()
        {
            transform.position = new Vector3(transform.position.x + Time.deltaTime * 5f * speed, transform.position.y, 0f);

            if (MainCamera.WorldToViewportPoint(transform.position).x > 1.2f)
            {
                var pos = MainCamera.ViewportToWorldPoint(new Vector3(-1, Random.Range(0, 1f)));
                transform.position = new Vector3(pos.x, pos.y, 0);
            }
        }
    }
}