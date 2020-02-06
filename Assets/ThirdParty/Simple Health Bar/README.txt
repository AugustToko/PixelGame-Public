Thank you for purchasing the Simple Health Bar package from the Unity Asset Store!

Support Email: tankandhealerstudio@outlook.com
Contact Form: http://www.tankandhealerstudio.com/contact-us.html
Official Documentation: Window / Tank and Healer Studio / Simple Health Bar FREE

// --- HOW TO CREATE --- //
	In order to create a Simple Health Bar in your scene, first make sure there is Canvas in your scene. After there is a Canvas
in your scene, simply find the prefab that you want. Prefabs are located at Assets / Simple Health Bar / Prefabs, and drag it into
the Canvas in your scene.

// --- HOW TO CUSTOMIZE --- //
	The Simple Health Bar is very easy to use and customize. Please see the Simple Health Bar inspector to see all the options that
you have available to you. All properties on the inspector have tooltips, so if you are confused about an option, just hover over the
property to read about what it does.

	// --- HOW TO REFERENCE --- //
	The Simple Health Bar is very easy to reference into your own scripts. Simply assign a name to the Simple Health Bar at the top
of the Simple Health Bar inspector, and then copy the example code provided. Below is the key function that you will be using to display
your status values to your player.

		// --- KEY FUNCTION --- //
		UpdateBar - Updates the values of the targeted Simple Health Bar so that it can be displayed on screen.

	Since the most common use of these kinds of bars is for health, let's assume that we will be implementing a Simple Health Bar into
our scene. In order to send the correct and current value to the Simple Health Bar, it is important to remember to only send information
once it has finished being modified. For example, for a players health, you will only want to update the Simple Health Bar once the
player has finished taking damage. For example:
		void TakeDamage ( int damage )
		{
			health -= damage;

			// <------- This is where you will want to update the Simple Health Bar. Only AFTER the value has been modified.
		}
	
	
		NOTE: For more information about each function and how to use it, please see the In-Engine Documentation window that was included in
	the download. It can be found at Window / Tank and Healer Studio / Ultimate Health Bar.

	// --- ATTENTION JAVASCRIPT USERS --- //
	If you are using Javascript to program your game, you may be unable to reference the Simple Health Bar in it's current folder
structure. In order for us to upload this package to the Unity Asset Store, we are required to put all the files within a single
sub-folder. This means that we cannot have a Plugins folder. For more information about the Plugins folder and script compilation
order, please look into Unity's documentation. In short, C# scripts are compiled in a different pass than Javascript, which means
that Javascript cannot reference C# scripts without them being placed in a special folder. In order to reference the Simple Health
Bar script from Javascript, simply create a folder in your main Assets folder named "Plugins", and place the SimpleHealthBar.cs
script into the Plugins folder.

	// --- CHANGE LOG --- //
	Version 1.0
		- Initial Release