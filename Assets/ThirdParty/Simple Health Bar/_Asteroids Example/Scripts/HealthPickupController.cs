/* Written by Kaz Crowe */
/* HealthPickupController.cs */
using UnityEngine;
using System.Collections;

namespace SimpleHealthBar_SpaceshipExample
{
	public class HealthPickupController : MonoBehaviour
	{
		// Reference Variables //
		Rigidbody myRigidbody;
		public ParticleSystem particles;
		public SpriteRenderer mySprite;

		// Controller Booleans //
		bool canDestroy = false;
		bool canPickup = true;
		

		public void Setup ( Vector3 force )
		{
			// Assign the rigidbody component attached to this game object.
			myRigidbody = GetComponent<Rigidbody>();

			// Add the force and torque to the rigidbody.
			myRigidbody.AddForce( force );
			
			StartCoroutine( DelayInitialDestruction( 1.0f ) );
		}

		IEnumerator DelayInitialDestruction ( float delayTime )
		{
			// Wait for the designated time.
			yield return new WaitForSeconds( delayTime );

			// Allow this asteroid to be destoryed.
			canDestroy = true;
		}
	
		void Update ()
		{
			// If the asteroid is out of the screen...
			if( Mathf.Abs( transform.position.x ) > Camera.main.orthographicSize * Camera.main.aspect * 1.3f || Mathf.Abs( transform.position.z ) > Camera.main.orthographicSize * 1.3f )
			{
				// If this asteroid can be destoryed, then commence destruction!
				if( canDestroy == true )
					Destroy( gameObject );
			}
		}

		void OnTriggerEnter ( Collider theCollider )
		{
			// If the collision was from the player...
			if( theCollider.gameObject.name == "Player" && canPickup == true )
			{
				canPickup = false;
				mySprite.enabled = false;
				PlayerHealth.Instance.HealPlayer();
				myRigidbody.isKinematic = true;
				particles.Stop();
				Destroy( gameObject, 3.0f );
			}
		}
	}
}