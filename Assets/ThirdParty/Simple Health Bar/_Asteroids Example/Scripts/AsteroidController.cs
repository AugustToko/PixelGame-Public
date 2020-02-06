/* Written by Kaz Crowe */
/* AsteroidController.cs */
using UnityEngine;
using System.Collections;

namespace SimpleHealthBar_SpaceshipExample
{
	public class AsteroidController : MonoBehaviour
	{
		// Reference Variables //
		Rigidbody myRigidbody;

		// Controller Booleans //
		bool canDestroy = false;
		bool isDestroyed = false;
		public bool isDebris = false;

		public float health;
		public float maxHealth;
		

		public void Setup ( Vector3 force, Vector3 torque )
		{
			// Assign the rigidbody component attached to this game object.
			myRigidbody = GetComponent<Rigidbody>();

			// Add the force and torque to the rigidbody.
			myRigidbody.AddForce( force );
			myRigidbody.AddTorque( torque );

			// Delay the time that this asteroid can be destroyed for being out of the screen.
			StartCoroutine( DelayInitialDestruction( isDebris == true  ? 0.25f : 1.0f ) );
		}

		IEnumerator DelayInitialDestruction ( float delayTime )
		{
			// Wait for the designated time.
			yield return new WaitForSeconds( delayTime );

			// Allow this asteroid to be destroyed.
			canDestroy = true;
		}
	
		void Update ()
		{
			// If the asteroid is out of the screen...
			if( Mathf.Abs( transform.position.x ) > Camera.main.orthographicSize * Camera.main.aspect * 1.3f || Mathf.Abs( transform.position.z ) > Camera.main.orthographicSize * 1.3f )
			{
				// If this asteroid can be destroyed, then commence destruction!
				if( canDestroy == true )
					Destroy( gameObject );
			}
		}

		void OnCollisionEnter ( Collision theCollision )
		{
			// If the collision was from a bullet...
			if( theCollision.gameObject.name == "Bullet" )
			{
				// Destroy the bullet.
				Destroy( theCollision.gameObject );

				TakeDamage( 20 );

				// Modify the score.
				if( health <= 0 )
					GameManager.Instance.ModifyScore( isDebris == true ? "Debris" : "Asteroid" );
				else
					GameManager.Instance.ModifyScore();
			}
			// Else if the collision was from the player...
			else if( theCollision.gameObject.name == "Player" )
			{
				theCollision.gameObject.GetComponent<PlayerHealth>().TakeDamage( 20 );

				TakeDamage( 50 );
			}
			// Else the collision is another asteroid/debris...
			else
			{
				AsteroidController astr = theCollision.gameObject.GetComponent<AsteroidController>();
				if( astr != null )
				{
					if( canDestroy && astr.isDebris == false )
						TakeDamage( 50 );
					else if( canDestroy && astr.isDebris == true )
						TakeDamage( 10 );
				}
				else if( canDestroy == true )
					TakeDamage( 50 );
			}

			// Spawn an explosion particle.
			GameManager.Instance.SpawnExplosion( theCollision.transform.position );
		}

		void Explode ()
		{
			// If this asteroid has already been destroyed, then return.
			if( isDestroyed == true )
				return;

			// Let the script know that this asteroid has already been destroyed.
			isDestroyed = true;

			// Spawn some debris from this asteroids position.
			GameManager.Instance.SpawnDebris( transform.position );

			// Destroy this asteroid.
			Destroy( gameObject );
		}

		void TakeDamage ( int damage )
		{
			// Reduce health by the damage dealt.
			health -= damage;
			
			// If health is less than or IS 0...
			if( health <= 0 )
			{
				// Make health absolutely 0.
				health = 0;

				// Run the death function.
				Death();
			}
		}

		void Death ()
		{
			// If this is not debris then run the explode function to create debris.
			if( isDebris == false )
				Explode();
			// Else just destroy the debris.
			else
				Destroy( gameObject );
		}
	}
}