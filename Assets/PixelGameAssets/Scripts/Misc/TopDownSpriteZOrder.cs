using PixelGameAssets.Scripts.Actor;
using PixelGameAssets.Scripts.WeaponSystem.WeaponType;
using UnityEngine;

namespace PixelGameAssets.Scripts.Misc
{
	/// <summary>
	/// 使用 Y 轴距离来控制层的距离
	/// </summary>
	public class TopDownSpriteZOrder : MonoBehaviour {

		private SpriteRenderer _spriteRenderer;

		private void Awake () {
			_spriteRenderer = GetComponent<SpriteRenderer> ();
		}

		private void LateUpdate () {
			if (_spriteRenderer != null)
			{
				_spriteRenderer.sortingOrder = Mathf.RoundToInt(transform.position.y) * -1;
//				if (Weapon == null)
//				{
//					Weapon = GetComponent<CommonPlayer>().currWeapon;
//				}
//
//				if (Weapon != null)
//				{
//					Weapon.GetComponentInChildren<SpriteRenderer>().sortingOrder = _spriteRenderer.sortingOrder + 1;
//				}
//				
			}
		}
	}
}
