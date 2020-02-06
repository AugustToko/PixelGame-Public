/* Written by Kaz Crowe */
/* PlayerHealth.cs */
using UnityEngine;
using System.Collections;

namespace SimpleHealthBar_SpaceshipExample
{
	public class PlayerHealth : MonoBehaviour
	{
		static PlayerHealth instance;
		public static PlayerHealth Instance { get { return instance; } }
		bool canTakeDamage = true;

		public int maxHealth = 100;
		float currentHealth = 0;
		public float invulnerabilityTime = 0.5f;

		float currentShield = 0;
		public int maxShield = 25;
		float regenShieldTimer = 0.0f;
		public float regenShieldTimerMax = 1.0f;

		public GameObject explosionParticles;

		public SimpleHealthBar healthBar;
		public SimpleHealthBar shieldBar;

	
		void Awake ()
		{
			// If the instance variable is already assigned, then there are multiple player health scripts in the scene. Inform the user.
			if( instance != null )
				Debug.LogError( "There are multiple instances of the Player Health script. Assigning the most recent one to Instance." );
			
			// Assign the instance variable as the Player Health script on this object.
			instance = GetComponent<PlayerHealth>();
		}

		void Start ()
		{
			// Set the current health and shield to max values.
			currentHealth = maxHealth;
			currentShield = maxShield;

			// Update the Simple Health Bar with the updated values of Health and Shield.
			healthBar.UpdateBar( currentHealth, maxHealth );
			shieldBar.UpdateBar( currentShield, maxShield );
		}

		void Update ()
		{
			// If the shield is less than max, and the regen cooldown is not in effect...
			if( currentShield < maxShield && regenShieldTimer <= 0 )
			{
				// Increase the shield.
				currentShield += Time.deltaTime * 5;

				// Update the Simple Health Bar with the new Shield values.
				shieldBar.UpdateBar( currentShield, maxShield );
			}

			// If the shield regen timer is greater than zero, then decrease the timer.
			if( regenShieldTimer > 0 )
				regenShieldTimer -= Time.deltaTime;
		}

		public void HealPlayer ()
		{
			// Increase the current health by 25%.
			currentHealth += ( maxHealth / 4 );

			// If the current health is greater than max, then set it to max.
			if( currentHealth > maxHealth )
				currentHealth = maxHealth;

			// Update the Simple Health Bar with the new Health values.
			healthBar.UpdateBar( currentHealth, maxHealth );
		}

		public void TakeDamage ( int damage )
		{
			// If the player can't take damage, then return.
			if( canTakeDamage == false )
				return;

			// If the shield value is greater than 0...
			if( currentShield > 0 )
			{
				// Reduce the current shield value.
				currentShield -= damage;

				// If the shield is now less than 0...
				if( currentShield < 0 )
				{
					// Reduce the health by how much damage went past the shield.
					currentHealth -= currentShield * -1;

					// Set the shield to zero.
					currentShield = 0;
				}
			}
			// Else there was no shield, so reduce health.
			else
				currentHealth -= damage;

			// If the health is less than zero...
			if( currentHealth <= 0 )
			{
				// Set the current health to zero.
				currentHealth = 0;

				// Run the Death function since the player has died.
				Death();
			}

			// Set canTakeDamage to false to make sure that the player cannot take damage for a brief moment.
			canTakeDamage = false;

			// Run the Invulnerablilty coroutine to delay incoming damage.
			StartCoroutine( "Invulnerablilty" );

			// Shake the camera for a moment to make each hit more dramatic.
			StartCoroutine( "ShakeCamera" );

			// Update the Health and Shield status bars.
			healthBar.UpdateBar( currentHealth, maxHealth );
			shieldBar.UpdateBar( currentShield, maxShield );

			// Reset the shield regen timer.
			regenShieldTimer = regenShieldTimerMax;
		}

		public void Death ()
		{
			// Show the death screen, and disable the player's control.
			GameManager.Instance.ShowDeathScreen();
			GetComponent<PlayerController>().canControl = false;

			// Spawn an explosion particle effect and the player's current position.
			GameObject explo = ( GameObject )Instantiate( explosionParticles, transform.position, Quaternion.identity );

			// Destroy the explosion in 2 seconds.
			Destroy( explo, 2 );

			// Destroy this game object.
			Destroy( gameObject );
		}

		IEnumerator Invulnerablilty ()
		{
			// Wait for the invulnerability time variable.
			yield return new WaitForSeconds( invulnerabilityTime );

			// Then allow the player to take damage again.
			canTakeDamage = true;
		}

		IEnumerator ShakeCamera ()
		{
			// Store the original position of the camera.
			Vector2 origPos = Camera.main.transform.position;
			for( float t = 0.0f; t < 1.0f; t += Time.deltaTime * 2.0f )
			{
				// Create a temporary vector2 with the camera's original position modified by a random distance from the origin.
				Vector2 tempVec = origPos + Random.insideUnitCircle;

				// Apply the temporary vector.
				Camera.main.transform.position = tempVec;

				// Yield until next frame.
				yield return null;
			}

			// Return back to the original position.
			Camera.main.transform.position = origPos;
		}
	}
}