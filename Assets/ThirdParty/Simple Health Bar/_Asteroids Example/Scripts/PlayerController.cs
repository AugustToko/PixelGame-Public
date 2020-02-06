/* Written by Kaz Crowe */
/* PlayerController.cs */
using UnityEngine;
using System.Collections;

namespace SimpleHealthBar_SpaceshipExample
{
	public class PlayerController : MonoBehaviour
	{
		static PlayerController instance;
		public static PlayerController Instance { get { return instance; } }

		// Speeds //
		[Header( "Speeds" )]
		public float rotationSpeed = 45.0f;
		public float accelerationSpeed = 2.0f;
		public float maxSpeed = 3.0f;
		public float shootingCooldown = 0.2f;

		// Prefabs //
		[Header( "Prefabs" )]
		public GameObject bulletPrefab;

		// Player Components //
		Rigidbody myRigidbody;
		[Header( "Components" )]
		public Transform gunTrans;
		public Transform bulletSpawnPos;

		// Timers //
		float shootingTimer = 0;

		// Controls //
		public bool canControl = true;

		// Input Positions //
		float rotation;
		float acceleration;

		[Header( "Overheat" )]
		public RectTransform overheatVisual;
		float overheatTimer = 0.0f;
		public float overheatTimerMax = 5.0f;
		public float cooldownSpeed = 2.0f;
		bool canShoot = true;
		
		public SimpleHealthBar gunHeatBar;


		void OnDisable ()
		{
			if( Application.isPlaying == true && overheatVisual != null )
				overheatVisual.gameObject.SetActive( false );
		}
		
		void Awake ()
		{
			// If the instance variable is already assigned, then there are multiple of this component in the scene. Inform the user.
			if( instance != null )
				Debug.LogError( "There are multiple instances of the Player Controller script. Assigning the most recent one to Instance." );
			
			// Assign the instance variable as the Player Controller script on this object.
			instance = GetComponent<PlayerController>();
		}

		void Start ()
		{
			// Store the player's rigidbody.
			myRigidbody = GetComponent<Rigidbody>();

			overheatVisual.sizeDelta = new Vector2( Screen.width / 12, Screen.width / 12 );
		}

		void Update ()
		{
			// Run the CheckExitScreen function to see if the player has left the screen.
			CheckExitScreen();

			// If the user cannot control the player, then return.
			if( canControl == false )
				return;

			// Store the input positions
			rotation = Input.GetAxis( "Horizontal" );
			acceleration = Input.GetAxis( "Vertical" );

			if( canShoot == true )
			{
				// If the shooting button is being used...
				if( Input.GetMouseButton( 0 ) )
				{
					// If the gun is not overheated, then increase the timer.
					if( overheatTimer < overheatTimerMax )
						overheatTimer += Time.deltaTime;
					// Else the gun has overheated, so start the overheat coroutine.
					else
					{
						canShoot = false;
						StartCoroutine( "DelayOverheat" );
					}

					if( shootingTimer <= 0 )
					{
						// Then reset the timer and shoot a bullet.
						shootingTimer = shootingCooldown;
						CreateBullets();
					}
				}
				else
				{
					if( overheatTimer > 0 )
						overheatTimer -= Time.deltaTime * cooldownSpeed;
				}

				gunHeatBar.UpdateBar( overheatTimer, overheatTimerMax );
			}

			// If the shoot timer is above zero, reduce it.
			if( shootingTimer > 0 )
				shootingTimer -= Time.deltaTime;
			
			Aiming();
		}

		void FixedUpdate ()
		{
			// If the user cannot control the player...
			if( canControl == false )
				return;

			// Figure out the rotation that the player should be facing and apply it.
			Vector3 lookRot = new Vector3( rotation, 0, acceleration );
			transform.LookAt( transform.position + lookRot );

			float distVec = Vector2.Distance( Vector2.zero, new Vector2( rotation, acceleration ) );

			// Apply the input force to the player.
			myRigidbody.AddForce( transform.forward * distVec * 1000.0f * accelerationSpeed * Time.deltaTime );

			// If the player's force is greater than the max speed, then normalize it.
			if( myRigidbody.velocity.magnitude > maxSpeed )
				myRigidbody.velocity = myRigidbody.velocity.normalized * maxSpeed;
		}

		void Aiming ()
		{
			// Position the image on the mouse.
			overheatVisual.position = Input.mousePosition - ( Vector3 )( overheatVisual.sizeDelta / 2 );

			float mousePositionX = Input.mousePosition.x;
			float mousePositionY = Input.mousePosition.y;

			mousePositionX /= Screen.width;
			mousePositionY /= Screen.height;

			mousePositionX -= 0.5f;
			mousePositionY -= 0.5f;

			Vector3 lookAtPosition = new Vector3( mousePositionX * ( Camera.main.orthographicSize * Camera.main.aspect ) * 2.0f, 0, mousePositionY * Camera.main.orthographicSize * 2.0f );

			gunTrans.LookAt( lookAtPosition );
		}

		void CheckExitScreen ()
		{
			// If the main camera is not assigned, then return.
			if( Camera.main == null )
				return;
			
			// If the absolute value of the player's X position is greater than the ortho size of the camera multiplied by the camera's aspect ratio, then reset the player on the other side.
			if( Mathf.Abs( myRigidbody.position.x ) > Camera.main.orthographicSize * Camera.main.aspect )
				myRigidbody.position = new Vector3( -Mathf.Sign( myRigidbody.position.x ) * Camera.main.orthographicSize * Camera.main.aspect, 0, myRigidbody.position.z );
			
			// If the absolute value of the player's Z position is greater than the ortho size, then reset the Z position to the other side.
			if( Mathf.Abs( myRigidbody.position.z ) > Camera.main.orthographicSize )
				myRigidbody.position = new Vector3( myRigidbody.position.x, myRigidbody.position.y, -Mathf.Sign( myRigidbody.position.z ) * Camera.main.orthographicSize );
		}

		void CreateBullets ()
		{
			// Create a new bulletPrefab game object at the barrel's position and rotation.
			GameObject bullet = Instantiate( bulletPrefab, bulletSpawnPos.position, bulletSpawnPos.rotation ) as GameObject;

			// Rename the bullet for reference within the asteroid script.
			bullet.name = bulletPrefab.name;
			
			// Apply a speed to the bullet's velocity.
			bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 200.0f;

			// Destroy the bullet after 3 seconds.
			Destroy( bullet, 3.0f );
		}

		IEnumerator DelayOverheat ()
		{
			gunHeatBar.UpdateBar( 1.0f, 1.0f );

			yield return new WaitForSeconds( 1.0f );

			for( float t = 0.0f; t < 1.0f; t += Time.deltaTime * 0.25f )
			{
				overheatTimer = Mathf.Lerp( 1.0f, 0.0f, t );
				gunHeatBar.UpdateBar( overheatTimer, 1.0f );
				yield return null;
			}

			overheatTimer = 0;
			gunHeatBar.UpdateBar( overheatTimer, overheatTimerMax );

			canShoot = true;
		}
	}
}