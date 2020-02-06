using System.Collections;
using UnityEngine;

namespace PixelGameAssets.Scripts.Misc
{
	/// <summary>
	/// 精灵闪光
	/// </summary>
	public class SpriteFlasher : MonoBehaviour {

		public float flashTime = 0.1f;

		public float flashamount = 1f;

		public SpriteRenderer spriteRenderer;

		IEnumerator flashRoutine;
		
		private static readonly int FlashAmount = Shader.PropertyToID("_FlashAmount");

		public void FlashMe () {
			if (flashRoutine != null) {
				StopCoroutine (flashRoutine);
			}

			flashRoutine = FlashRoutine ();
			StartCoroutine (flashRoutine);
		}

		IEnumerator FlashRoutine () {
			spriteRenderer.material.SetFloat (FlashAmount, flashamount);

			yield return new WaitForSeconds (flashTime);

			spriteRenderer.material.SetFloat (FlashAmount, 0f);
		}
	}
}
