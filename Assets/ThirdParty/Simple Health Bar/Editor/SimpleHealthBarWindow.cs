/* Written by Kaz Crowe */
/* SimpleHealthBarWindow.cs */
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SimpleHealthBarWindow : EditorWindow
{
	static string version = "1.0.3f";// ALWAYS UDPATE
	static int importantChanges = 0;// UPDATE ON IMPORTANT CHANGES
	static string menuTitle = "Main Menu";

	// LAYOUT STYLES //
	int sectionSpace = 20;
	int itemHeaderSpace = 10;
	int paragraphSpace = 5;
	GUIStyle sectionHeaderStyle = new GUIStyle();
	GUIStyle itemHeaderStyle = new GUIStyle();
	GUIStyle paragraphStyle = new GUIStyle();

	GUILayoutOption[] buttonSize = new GUILayoutOption[] { GUILayout.Width( 200 ), GUILayout.Height( 35 ) }; 
	GUILayoutOption[] docSize = new GUILayoutOption[] { GUILayout.Width( 300 ), GUILayout.Height( 330 ) };
	GUISkin style;
	Texture2D scriptReference;
	Texture2D ujPromo, shbProPromo;

	class PageInformation
	{
		public string pageName = "";
		public Vector2 scrollPosition = Vector2.zero;
		public delegate void TargetMethod();
		public TargetMethod targetMethod;
	}
	static PageInformation mainMenu = new PageInformation() { pageName = "Main Menu" };
	static PageInformation howTo = new PageInformation() { pageName = "How To" };
	static PageInformation overview = new PageInformation() { pageName = "Overview" };
	static PageInformation documentation = new PageInformation() { pageName = "Documentation" };
	static PageInformation otherProducts = new PageInformation() { pageName = "Other Products" };
	static PageInformation feedback = new PageInformation() { pageName = "Feedback" };
	static PageInformation changeLog = new PageInformation() { pageName = "Change Log" };
	static PageInformation versionChanges = new PageInformation() { pageName = "Version Changes" };
	static PageInformation thankYou = new PageInformation() { pageName = "Thank You" };
	static PageInformation settings = new PageInformation() { pageName = "Window Settings" };
	static List<PageInformation> pageHistory = new List<PageInformation>();
	static PageInformation currentPage = new PageInformation();

	enum FontSize
	{
		Small,
		Medium,
		Large
	}
	FontSize fontSize = FontSize.Small;
	bool configuredFontSize = false;

	static Texture2D introThumbnail;
	static WWW introThumbnailPage;

	
	class DocumentationInfo
	{
		public string functionName = "";
		public string[] parameter;
		public string returnType = "";
		public string description = "";
		public string codeExample = "";
	}
	#region Simple Health Bar Documentation
	DocumentationInfo p_UpdateBar = new DocumentationInfo()
	{
		functionName = "UpdateBar()",
		description = "Updates the Simple Health Bar with the current and max values.",
		codeExample = "healthBar.UpdateBar( health, maxHealth );"
	};
	DocumentationInfo p_UpdateColor = new DocumentationInfo()
	{
		functionName = "UpdateColor()",
		parameter = new string[ 2 ]
		{
			"Color targetColor - The color to apply to the image.",
			"Gradient targetGradient - The gradient to apply to the image."
		},
		description = "Updates the image component with the Color parameter. This function also has an override to apply a new Gradient to the image as well.",
		codeExample = "healthBar.UpdateColor( Color.white );"
	};
	DocumentationInfo p_UpdateTextColor = new DocumentationInfo()
	{
		functionName = "UpdateTextColor()",
		parameter = new string[ 1 ]
		{
			"Color targetColor - The color to apply to the text component."
		},
		description = "Updates the color of the text component.",
		codeExample = "healthBar.UpdateTextColor( Color.red );"
	};
	#endregion
	
	[MenuItem( "Window/Tank and Healer Studio/Simple Health Bar FREE", false, 0 )]
	static void InitializeWindow ()
	{
		EditorWindow window = GetWindow<SimpleHealthBarWindow>( true, "Tank and Healer Studio Asset Window", true );
		window.maxSize = new Vector2( 500, 500 );
		window.minSize = new Vector2( 500, 500 );
		window.Show();
	}

	public static void OpenDocumentation ()
	{
		InitializeWindow();

		if( !pageHistory.Contains( documentation ) )
			NavigateForward( documentation );
	}

	void OnEnable ()
	{
		style = ( GUISkin )Resources.Load( "SimpleHealthBarEditorSkin" );

		scriptReference = ( Texture2D )Resources.Load( "SHBF_ScriptRef" );
		ujPromo = ( Texture2D ) Resources.Load( "UJ_Promo" );
		shbProPromo = ( Texture2D ) Resources.Load( "SHB_Pro_Promo" );

		if( !pageHistory.Contains( mainMenu ) )
			pageHistory.Insert( 0, mainMenu );

		mainMenu.targetMethod = MainMenu;
		howTo.targetMethod = HowTo;
		overview.targetMethod = OverviewPage;
		documentation.targetMethod = DocumentationPage;
		otherProducts.targetMethod = OtherProducts;
		feedback.targetMethod = Feedback;
		changeLog.targetMethod = ChangeLog;
		versionChanges.targetMethod = VersionChanges;
		thankYou.targetMethod = ThankYou;
		settings.targetMethod = WindowSettings;

		if( pageHistory.Count == 1 )
			currentPage = mainMenu;
	}
	
	void OnGUI ()
	{
		if( style == null )
		{
			GUILayout.BeginVertical( "Box" );
			GUILayout.FlexibleSpace();
			ErrorScreen();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndVertical();
			return;
		}

		GUI.skin = style;

		paragraphStyle = GUI.skin.GetStyle( "ParagraphStyle" );
		itemHeaderStyle = GUI.skin.GetStyle( "ItemHeader" );
		sectionHeaderStyle = GUI.skin.GetStyle( "SectionHeader" );

		if( !configuredFontSize )
		{
			configuredFontSize = true;
			if( paragraphStyle.fontSize == 14 )
				fontSize = FontSize.Large;
			else if( paragraphStyle.fontSize == 12 )
				fontSize = FontSize.Medium;
			else
				fontSize = FontSize.Small;
		}
		
		GUILayout.BeginVertical( "Box" );
		
		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.LabelField( "Simple Health Bar FREE", GUI.skin.GetStyle( "WindowTitle" ) );

		if( GUILayout.Button( "", GUI.skin.GetStyle( "SettingsButton" ) ) && currentPage != settings && !pageHistory.Contains( settings ) )
			NavigateForward( settings );

		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 3 );
		
		if( GUILayout.Button( "Version " + version, GUI.skin.GetStyle( "VersionNumber" ) ) && currentPage != changeLog && !pageHistory.Contains( changeLog ) )
			NavigateForward( changeLog );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.Space( 12 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.Space( 5 );
		if( pageHistory.Count > 1 && !pageHistory.Contains( thankYou ) )
		{
			if( GUILayout.Button( "", GUI.skin.GetStyle( "BackButton" ), GUILayout.Width( 80 ), GUILayout.Height( 40 ) ) )
				NavigateBack();
			rect = GUILayoutUtility.GetLastRect();
			EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		}
		else
			GUILayout.Space( 80 );

		GUILayout.Space( 15 );
		EditorGUILayout.LabelField( menuTitle, GUI.skin.GetStyle( "MenuTitle" ) );
		GUILayout.FlexibleSpace();
		GUILayout.Space( 80 );
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		
		if( currentPage.targetMethod != null )
			currentPage.targetMethod();

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		GUILayout.Space( 25 );
		
		EditorGUILayout.EndVertical();

		Repaint();
	}

	void ErrorScreen ()
	{
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUIStyle errorStyle = new GUIStyle( GUI.skin.label );
		errorStyle.fixedHeight = 55;
		errorStyle.fixedWidth = 175;
		errorStyle.fontSize = 48;
		errorStyle.normal.textColor = new Color( 1.0f, 0.0f, 0.0f, 1.0f );
		EditorGUILayout.LabelField( "ERROR", errorStyle );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 50 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.Space( 50 );
		EditorGUILayout.LabelField( "Could not find the needed GUISkin located in the Editor / Resources folder. Please ensure that the correct GUISkin, SimpleHealthBarEditorSkin, is in the right folder( Simple Health Bar / Editor / Resources ) before trying to access the Simple Health Bar Window.", EditorStyles.wordWrappedLabel );
		GUILayout.Space( 50 );
		EditorGUILayout.EndHorizontal();
	}

	static void NavigateBack ()
	{
		pageHistory.RemoveAt( pageHistory.Count - 1 );
		menuTitle = pageHistory[ pageHistory.Count - 1 ].pageName;
		currentPage = pageHistory[ pageHistory.Count - 1 ];
	}

	static void NavigateForward ( PageInformation menu )
	{
		pageHistory.Add( menu );
		menuTitle = menu.pageName;
		currentPage = menu;
	}
	
	void MainMenu ()
	{
		mainMenu.scrollPosition = EditorGUILayout.BeginScrollView( mainMenu.scrollPosition, false, false, docSize );

		GUILayout.Space( 25 );
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "How To", buttonSize ) )
			NavigateForward( howTo );

		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Overview", buttonSize ) )
			NavigateForward( overview );
		
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Documentation", buttonSize ) )
			NavigateForward( documentation );
		
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Other Products", buttonSize ) )
			NavigateForward( otherProducts );
		
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Feedback", buttonSize ) )
			NavigateForward( feedback );
		
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();

		EditorGUILayout.EndScrollView();
	}
	
	void HowTo ()
	{
		StartPage( howTo );

		EditorGUILayout.LabelField( "Watch Introduction Video", sectionHeaderStyle );
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
			Application.OpenURL( "https://www.youtube.com/watch?v=0evXvWE--BQ&feature=youtu.be" );
		
		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "How To Create", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "In order to create a Simple Health Bar in your scene, first make sure there is Canvas in your scene. After there is a Canvas in your scene, simply find the prefab that you want. Prefabs are located at Assets / Simple Health Bar / Prefabs, and drag it into the Canvas in your scene.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "How To Reference", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "The Simple Health Bar is very easy to reference into your own scripts. Simply assign a name to the Simple Health Bar at the top of the Simple Health Bar inspector, and then copy the example code provided. Below is the key function that you will be using to display your status values to your player. For this example, we will be using the variable name \"healthBar\" for our Simple Health bar.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Key Function:", itemHeaderStyle );
		EditorGUILayout.TextArea( "healthBar.UpdateBar( currentValue, maxValue );", GUI.skin.GetStyle( "TextArea" ) );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label( scriptReference );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		
		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "Simple Example", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "Since the most common use of these kinds of bars is for health, let's assume that we will be implementing a Simple Health Bar into our scene. In order to send the correct and current value to the Simple Health Bar, it is important to remember to only send information once it has finished being modified. For example, for a players health, you will only want to update the Simple Health Bar once the player has finished taking damage. For example:", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Coding Example:", itemHeaderStyle );
		EditorGUILayout.TextArea( "void TakeDamage ( int damage )\n{\n" + Indent + "health -= damage;\n\n" + Indent + "// Now is where you will want to update the Simple Health Bar. Only AFTER the value has been modified.\n" + Indent + "healthBar.UpdateBar( health, maxHealth );\n}", GUI.skin.GetStyle( "TextArea" ) );
		
		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "<b>NOTE:</b> For more information about each function and how to use it, please see the Documentation section of this window.", paragraphStyle );

		EndPage();
	}
	
	void OverviewPage ()
	{
		StartPage( overview );

		EditorGUILayout.LabelField( "Simple Health Bar Overview", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Bar Name", itemHeaderStyle );
		EditorGUILayout.LabelField( "The unique name to be used in reference to this bar. This string will be used to call functions on this particular Simple Health Bar.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Bar Image", itemHeaderStyle );
		EditorGUILayout.LabelField( "The image component to be used for this bar. When assigned, this image will be set to Image Type: Filled if has not been changed before. Remember to refer to the actual Image component to change the Method and Origin of the Image.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Color Mode", itemHeaderStyle );
		EditorGUILayout.LabelField( "The mode in which to display the color to the barImage component. The option <i>Single</i> will use a single color to apply to the Image component, and the option <i>Gradient</i> will allow you to set a custom gradient for the fill of the Image.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Display Text", itemHeaderStyle );
		EditorGUILayout.LabelField( "The Display Text option allows you to determine how this bar will display text to the user, if at all. Once enabled, the sub-options will allow you to set the Text component to display the text to, the color of the text, and any additional text that you want added to the values.", paragraphStyle );
		
		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( "Test Value", itemHeaderStyle );
		EditorGUILayout.LabelField( "A simple slider to help see how the Simple Health Bar will look when it is being used.", paragraphStyle );

		EndPage();
	}
	
	void DocumentationPage ()
	{
		StartPage( documentation );

		/* //// --------------------------- < PUBLIC FUNCTIONS > --------------------------- \\\\ */
		EditorGUILayout.LabelField( "Public Functions", sectionHeaderStyle );

		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( Indent + "All of the following public functions are only available from a reference to the Simple Health Bar. Each example provided relies on having a Simple Health Bar variable named 'healthBar' stored inside your script. When using any of the example code provided, make sure that you have a public Simple Health Bar variable like the one below:", paragraphStyle );

		EditorGUILayout.TextArea( "public SimpleHealthBar healthBar;", GUI.skin.textArea );

		EditorGUILayout.LabelField( "Please click on the function name to learn more.", paragraphStyle );

		GUILayout.Space( paragraphSpace );

		ShowDocumentation( p_UpdateBar );
		ShowDocumentation( p_UpdateColor );
		ShowDocumentation( p_UpdateTextColor );

		GUILayout.Space( itemHeaderSpace );
		
		EndPage();
	}
	
	void OtherProducts ()
	{
		StartPage( otherProducts );

		/* ------------ < SIMPLE HEALTH BAR > ------------ */
		if( shbProPromo != null )
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Space( 15 );
			GUILayout.Label( shbProPromo, GUILayout.Width( 250 ), GUILayout.Height( 125 ) );
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			GUILayout.Space( paragraphSpace );
		}

		EditorGUILayout.LabelField( "Simple Health Bar PRO", sectionHeaderStyle );

		EditorGUILayout.LabelField( Indent + "The Simple Health Bar PRO is an incredibly easy solution to visually display health and other important bars to the user. It features fast and easy implementation with copy-and-paste script reference, as well as simple and useful customization options. With the Simple Health Bar PRO, you'll have access to features such as color gradient, <i>Smooth Fill</i> and a <i>Dramatic</i> double fill effect, adding to a professional and polished look for your game's UI.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "More Info", buttonSize ) )
			Application.OpenURL( "https://www.tankandhealerstudio.com/simple-health-bar-pro.html" );
		
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		/* -------------- < END SIMPLE HEALTH BAR > --------------- */

		GUILayout.Space( 25 );

		/* -------------- < ULTIMATE JOYSTICK > -------------- */
		if( ujPromo != null )
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Space( 15 );
			GUILayout.Label( ujPromo, GUILayout.Width( 250 ), GUILayout.Height( 125 ) );
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();

			GUILayout.Space( paragraphSpace );
		}

		EditorGUILayout.LabelField( "Ultimate Joystick", sectionHeaderStyle );

		EditorGUILayout.LabelField( Indent + "The Ultimate Joystick is a simple, yet powerful input tool for the development of your mobile games. The Ultimate Joystick was created with the goal of giving developers an incredibly versatile joystick solution. Designed with this in mind, it is extremely fast and easy to implement into either new or existing scripts. You don't need to be a programmer to work with the Ultimate Joystick, and it is very easy to integrate into any type of character controller. Additionally, the Ultimate Joystick features a complete in-engine Documentation Window to help you understand exactly what you have at your disposal. In its entirety, Ultimate Joystick is an elegant and easy to utilize mobile joystick solution.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "More Info", buttonSize ) )
			Application.OpenURL( "http://www.tankandhealerstudio.com/ultimate-joystick.html" );
		
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		/* ------------ < END ULTIMATE JOYSTICK > ------------ */
		
		EndPage();
	}
	
	void Feedback ()
	{
		StartPage( feedback );

		EditorGUILayout.LabelField( "Having Problems?", sectionHeaderStyle );

		EditorGUILayout.LabelField( Indent + "If you experience any issues with the Simple Health Bar, please contact us right away! We will lend any assistance that we can to resolve any issues that you have.\n\n<b>Support Email:</b>.", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com", itemHeaderStyle, GUILayout.Height( 15 ) );
		GUILayout.Space( 25 );


		EditorGUILayout.LabelField( "Good Experiences?", sectionHeaderStyle );

		EditorGUILayout.LabelField( Indent + "If you have appreciated how easy the Simple Health Bar is to get into your project, leave us a comment and rating on the Unity Asset Store. We are very grateful for all positive feedback that we get.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Rate Us", buttonSize ) )
			Application.OpenURL( "https://www.assetstore.unity3d.com/#!/content/95420" );
		
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 25 );

		EditorGUILayout.LabelField( "Show Us What You've Done!", sectionHeaderStyle );

		EditorGUILayout.LabelField( Indent + "If you have used any of the assets created by Tank & Healer Studio in your project, we would love to see what you have done. Contact us with any information on your game and we will be happy to support you in any way that we can!\n\n<b>Contact Us:</b>", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com" , itemHeaderStyle, GUILayout.Height( 15 ) );
		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField( "Happy Game Making,\n	-Tank & Healer Studio", GUILayout.Height( 30 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 25 );

		EndPage();
	}

	void ChangeLog ()
	{
		StartPage( changeLog );

		EditorGUILayout.LabelField( "Version 1.0.3f", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Updated Documentation Window to contain a link to a brief introduction video to help get you started quickly.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 1.0.2f", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Added complete In-Engine Documentation window.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed the ability to reference the Simple Health Bar from static functions. All this functionality is inside the Simple Health Bar PRO package.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Removed the Fill Constraint option from the Simple Health Bar FREE package.", paragraphStyle );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 1.0.1f", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Fixed some prefabs not having assigned variables.", paragraphStyle );
		EditorGUILayout.LabelField( "  • Fixed example scene bars that were not correct.", paragraphStyle );
		
		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.LabelField( "Version 1.0", itemHeaderStyle );
		EditorGUILayout.LabelField( "  • Initial Release.", paragraphStyle );

		EndPage();
	}
	
	void ThankYou ()
	{
		StartPage( thankYou );

		EditorGUILayout.LabelField( Indent + "The two of us at Tank & Healer Studio would like to thank you for purchasing the Simple Health Bar asset package from the Unity Asset Store. If you have any questions about the Simple Health Bar that are not covered in this Documentation Window, please don't hesitate to contact us at: ", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com" , itemHeaderStyle, GUILayout.Height( 15 ) );
		GUILayout.Space( itemHeaderSpace );
		
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label( introThumbnail );

		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
			Application.OpenURL( "https://www.youtube.com/watch?v=0evXvWE--BQ&feature=youtu.be" );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.LabelField( Indent + "We hope that the Simple Health Bar will be a great help to you in the development of your game. Please click the video above to see a brief introduction to the Simple Health Bar FREE package. Then, after pressing the continue button below, you will be presented with helpful information on this asset to assist you in implementing Simple Health Bar into your project.", paragraphStyle );

		GUILayout.Space( sectionSpace );

		EditorGUILayout.LabelField( "ATTENTION", sectionHeaderStyle );
		EditorGUILayout.LabelField( "If you previously had the Simple Health Bar, and are now experiencing errors, please visit the link below to learn more about how to fix these errors.", paragraphStyle );

		EditorGUILayout.LabelField( "Learn More", itemHeaderStyle );
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );
		if( Event.current.type == EventType.MouseDown && rect.Contains( Event.current.mousePosition ) )
			Application.OpenURL( "https://www.tankandhealerstudio.com/simple-health-bar-error.html" );

		GUILayout.Space( itemHeaderSpace );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		EditorGUILayout.LabelField( "Happy Game Making,\n	-Tank & Healer Studio", paragraphStyle, GUILayout.Height( 30 ) );
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 15 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Continue", buttonSize ) )
			NavigateBack();
		
		rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		EndPage();
	}

	void VersionChanges ()
	{
		StartPage( versionChanges );

		EditorGUILayout.LabelField( Indent + "This page is used for major changes to the asset that needs to be brought to the attention of the user.", paragraphStyle );

		GUILayout.Space( paragraphSpace );
		EditorGUILayout.SelectableLabel( "tankandhealerstudio@outlook.com", itemHeaderStyle, GUILayout.Height( 15 ) );
		GUILayout.Space( sectionSpace );

		GUILayout.Space( 15 );

		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if( GUILayout.Button( "Got it!", buttonSize ) )
			NavigateBack();
		
		var rect = GUILayoutUtility.GetLastRect();
		EditorGUIUtility.AddCursorRect( rect, MouseCursor.Link );

		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
		
		EndPage();
	}

	void WindowSettings ()
	{
		StartPage( settings );

		EditorGUI.BeginChangeCheck();
		fontSize = ( FontSize )EditorGUILayout.EnumPopup( "Font Size", fontSize );
		if( EditorGUI.EndChangeCheck() )
		{
			switch( fontSize )
			{
				case FontSize.Small:
				default:
				{
					GUI.skin.textArea.fontSize = 11;
					paragraphStyle.fontSize = 11;
					itemHeaderStyle.fontSize = 11;
					sectionHeaderStyle.fontSize = 14;
				}
				break;
				case FontSize.Medium:
				{
					GUI.skin.textArea.fontSize = 12;
					paragraphStyle.fontSize = 12;
					itemHeaderStyle.fontSize = 12;
					sectionHeaderStyle.fontSize = 18;
				}
				break;
				case FontSize.Large:
				{
					GUI.skin.textArea.fontSize = 14;
					paragraphStyle.fontSize = 14;
					itemHeaderStyle.fontSize = 14;
					sectionHeaderStyle.fontSize = 20;
				}
				break;
			}
		}

		GUILayout.Space( 20 );
		
		EditorGUILayout.LabelField( "Example Text", sectionHeaderStyle );
		GUILayout.Space( paragraphSpace );
		EditorGUILayout.LabelField( "Example Text", itemHeaderStyle );
		GUILayout.Space( paragraphSpace );
		EditorGUILayout.LabelField( "This is an example paragraph to see the size of the text after modification.", paragraphStyle );

		EndPage();
	}

	void StartPage ( PageInformation pageInfo )
	{
		pageInfo.scrollPosition = EditorGUILayout.BeginScrollView( pageInfo.scrollPosition, false, false, docSize );
		GUILayout.Space( 15 );
	}

	void EndPage ()
	{
		EditorGUILayout.EndScrollView();
	}

	void ShowDocumentation ( DocumentationInfo info )
	{
		GUILayout.Space( paragraphSpace );

		EditorGUILayout.LabelField( info.functionName, itemHeaderStyle );
		if( info.parameter != null )
		{
			for( int i = 0; i < info.parameter.Length; i++ )
				EditorGUILayout.LabelField( Indent + "<i>Parameter:</i> " + info.parameter[ i ], paragraphStyle );
		}
		if( info.returnType != string.Empty )
			EditorGUILayout.LabelField( Indent + "<i>Return type:</i> " + info.returnType, paragraphStyle );

		EditorGUILayout.LabelField( Indent + "<i>Description:</i> " + info.description, paragraphStyle );

		if( info.codeExample != string.Empty )
			EditorGUILayout.TextArea( info.codeExample, GUI.skin.textArea );

		GUILayout.Space( paragraphSpace );
	}

	string Indent
	{
		get
		{
			return "    ";
		}
	}

	[InitializeOnLoad]
	class SimpleHealthBarInitialLoad
	{
		static SimpleHealthBarInitialLoad ()
		{
			// If this is the first time that the user has downloaded this package...
			if( !EditorPrefs.HasKey( "SimpleHealthBarFreeVersion" ) )
			{
				// Navigate to the Thank You page.
				NavigateForward( thankYou );

				// Set the version to current so they won't see these version changes.
				EditorPrefs.SetInt( "SimpleHealthBarFreeVersion", importantChanges );

				introThumbnailPage = new WWW( "https://www.tankandhealerstudio.com/uploads/7/7/4/9/77490188/shb-free-intro-thumb-small_orig.png" );

				EditorApplication.update += WaitForIntroThumbnail;
			}
			else if( EditorPrefs.GetInt( "SimpleHealthBarFreeVersion" ) < importantChanges )
			{
				// Navigate to the Version Changes page.
				NavigateForward( versionChanges );

				// Set the version to current so they won't see this page again.
				EditorPrefs.SetInt( "SimpleHealthBarFreeVersion", importantChanges );

				EditorApplication.update += WaitForCompile;
			}
		}

		static void WaitForCompile ()
		{
			if( EditorApplication.isCompiling )
				return;

			EditorApplication.update -= WaitForCompile;

			InitializeWindow();
		}

		static void WaitForIntroThumbnail ()
		{
			if( !introThumbnailPage.isDone || EditorApplication.isCompiling )
				return;

			EditorApplication.update -= WaitForIntroThumbnail;

			InitializeWindow();

			introThumbnail = introThumbnailPage.texture;
		}
	}
}