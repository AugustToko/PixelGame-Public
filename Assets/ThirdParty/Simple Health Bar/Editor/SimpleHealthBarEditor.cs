/* Written by Kaz Crowe */
/* SimpleHealthBarEditor.cs */
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.AnimatedValues;

[CanEditMultipleObjects]
[CustomEditor( typeof( SimpleHealthBar ) )]
public class SimpleHealthBarEditor : Editor
{
	SimpleHealthBar targ;

	// ----->>> IMAGE //
	AnimBool ImageAssigned, ImageUnassigned;
	AnimBool ImageFilledWarning;
	SerializedProperty barImage;

	// ----->>> COLOR //
	AnimBool ImageColorWarning;
	SerializedProperty colorMode, barColor, barGradient;

	// ----->>> TEXT //
	AnimBool DisplayTextOption;
	Color textColor;
	SerializedProperty barText;
	SerializedProperty displayText, additionalText;

	// ---- < SCRIPT REFERENCE > ---- //
	AnimBool ExampleCode;
	enum FunctionList
	{
		UpdateBar,
		UpdateColor,
		UpdateTextColor
	}
	FunctionList functionList = FunctionList.UpdateBar;
	string exampleCode;

	// ----->>> TEST VALUE //
	float testValue = 100.0f;

	string barName = "healthBar";


	void OnEnable ()
	{
		// Store the references to all variables.
		StoreReferences();

		// Register the UndoRedoCallback function to be called when an undo/redo is performed.
		Undo.undoRedoPerformed += UndoRedoCallback;

		if( targ != null && targ.barImage != null )
			testValue = targ.barImage.fillAmount * 100.0f;
	}

	void OnDisable ()
	{
		// Remove the UndoRedoCallback from the Undo event.
		Undo.undoRedoPerformed -= UndoRedoCallback;
	}

	// Function called for Undo/Redo operations.
	void UndoRedoCallback ()
	{
		// Re-reference all variables on undo/redo.
		StoreReferences();
	}

	public override void OnInspectorGUI ()
	{
		serializedObject.Update();

		EditorGUILayout.Space();
		
		EditorGUILayout.BeginVertical( "Box" );

		// ----- < BAR NAME > ----- //
		if( barName == string.Empty && Event.current.type == EventType.Repaint )
		{
			GUIStyle style = new GUIStyle( GUI.skin.textField );
			style.normal.textColor = new Color( 0.5f, 0.5f, 0.5f, 0.75f );
			EditorGUILayout.TextField( new GUIContent( "Bar Name", "The unique name to be used in reference to this bar." ), "Bar Name", style );
		}
		else
		{
			EditorGUI.BeginChangeCheck();
			barName = EditorGUILayout.TextField( new GUIContent( "Bar Name", "The unique name to be used in reference to this bar." ), barName );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				ExampleCode.target = barName != string.Empty;
				GenerateExampleCode();
			}
		}
		// ----- < END BAR NAME > ----- //

		// ----- < NAME ERRORS > ----- //
		if( EditorGUILayout.BeginFadeGroup( ExampleCode.faded ) )
		{
			GUILayout.Space( 1 );

			EditorGUILayout.LabelField( "Public Variable", EditorStyles.boldLabel );
			EditorGUILayout.LabelField( "Copy this variable declaration into your custom scripts.", EditorStyles.wordWrappedLabel );
			EditorGUILayout.TextField( "public SimpleHealthBar " + barName + ";" );

			EditorGUILayout.LabelField( "Example Code Generator", EditorStyles.boldLabel );
			EditorGUILayout.LabelField( "Please choose the function that you want to use. Afterward, copy and paste the provided code into your scripts where you want to display a status to the user.", EditorStyles.wordWrappedLabel );
			EditorGUI.BeginChangeCheck();
			functionList = ( FunctionList )EditorGUILayout.EnumPopup( "Function", functionList );
			if( EditorGUI.EndChangeCheck() )
				GenerateExampleCode();

			EditorGUILayout.TextField( exampleCode );

			GUILayout.Space( 1 );
		}
		EditorGUILayout.EndFadeGroup();
		// ----- < END NAME ERRORS > ----- //

		EditorGUILayout.EndVertical();

		// ----- < BAR IMAGE > ----- //
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField( barImage, new GUIContent( "Bar Image", "The image component to be used for this bar." ) );
		if( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();
			if( targ.barImage != null && targ.barImage.type != Image.Type.Filled )
			{
				targ.barImage.type = Image.Type.Filled;
				targ.barImage.fillMethod = Image.FillMethod.Horizontal;
				EditorUtility.SetDirty( targ.barImage );
			}
			if( targ.barImage != null )
			{
				barColor.colorValue = targ.barImage.color;
				serializedObject.ApplyModifiedProperties();
			}
			targ.UpdateBar( testValue, 100.0f );

			ImageFilledWarning.target = GetBarImageWarning();
			ImageAssigned.target = GetImageAssigned();
			ImageUnassigned.target = GetImageUnassigned();
		}

		if( EditorGUILayout.BeginFadeGroup( ImageUnassigned.faded ) )
		{
			EditorGUILayout.BeginVertical( "Box" );
			EditorGUILayout.HelpBox( "Image is unassigned.", MessageType.Warning );
			if( GUILayout.Button( "Find", EditorStyles.miniButton ) )
			{
				barImage.objectReferenceValue = targ.GetComponent<Image>();
				serializedObject.ApplyModifiedProperties();
				if( targ.barImage != null )
				{
					if( targ.barImage.type != Image.Type.Filled )
					{
						targ.barImage.type = Image.Type.Filled;
						targ.barImage.fillMethod = Image.FillMethod.Horizontal;
						EditorUtility.SetDirty( targ.barImage );
					}

					barColor.colorValue = targ.barImage.color;
					serializedObject.ApplyModifiedProperties();
				}

				ImageFilledWarning.target = GetBarImageWarning();
				ImageAssigned.target = GetImageAssigned();
				ImageUnassigned.target = GetImageUnassigned();
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndFadeGroup();
		// ----- < END BAR IMAGE > ----- //

		if( EditorGUILayout.BeginFadeGroup( ImageAssigned.faded ) )
		{
			// ----- < BAR IMAGE ERROR > ----- //
			if( EditorGUILayout.BeginFadeGroup( ImageFilledWarning.faded ) )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.HelpBox( "Invalid Image Type: " + targ.barImage.type.ToString(), MessageType.Warning );
				if( GUILayout.Button( "Fix", EditorStyles.miniButton ) )
				{
					targ.barImage.type = Image.Type.Filled;
					EditorUtility.SetDirty( targ.barImage );

					ImageFilledWarning.target = GetBarImageWarning();
				}
				EditorGUILayout.EndVertical();
			}
			if( ImageAssigned.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();
			// ----- < END BAR IMAGE ERROR > ----- //

			// ----- < BAR COLORS > ----- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( colorMode, new GUIContent( "Color Mode", "The mode in which to display the color to the barImage component." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				UpdateStatusColor();
				ImageColorWarning.target = GetColorWarning();
			}

			EditorGUI.BeginChangeCheck();
			EditorGUI.indentLevel = 1;
			if( targ.colorMode == SimpleHealthBar.ColorMode.Single )
				EditorGUILayout.PropertyField( barColor, new GUIContent( "Color", "The color of this barImage." ) );
			else
				EditorGUILayout.PropertyField( barGradient, new GUIContent( "Gradient", "The color gradient of this barImage." ) );
			EditorGUI.indentLevel = 0;
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				UpdateStatusColor();
				ImageColorWarning.target = GetColorWarning();
			}

			if( GetColorWarning() )
				ImageColorWarning.target = GetColorWarning();

			if( EditorGUILayout.BeginFadeGroup( ImageColorWarning.faded ) )
			{
				EditorGUILayout.BeginVertical( "Box" );
				EditorGUILayout.HelpBox( "Image color has been modified incorrectly.", MessageType.Warning );
				EditorGUILayout.BeginHorizontal();
				if( GUILayout.Button( "Update Image", EditorStyles.miniButtonLeft ) )
				{
					targ.barImage.color = barColor.colorValue;
					EditorUtility.SetDirty( targ.barImage );
					ImageColorWarning.target = GetColorWarning();
				}
				if( GUILayout.Button( "Update Script", EditorStyles.miniButtonRight ) )
				{
					barColor.colorValue = targ.barImage.color;
					serializedObject.ApplyModifiedProperties();
					ImageColorWarning.target = GetColorWarning();
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}
			if( ImageAssigned.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();
			// ----- < END BAR COLORS > ----- //

			EditorGUILayout.Space();

			// ------- < TEXT OPTIONS > ------- //
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField( displayText, new GUIContent( "Display Text", "Determines how this bar will display text to the user." ) );
			if( EditorGUI.EndChangeCheck() )
			{
				serializedObject.ApplyModifiedProperties();
				DisplayTextOption.target = targ.displayText != SimpleHealthBar.DisplayText.Disabled;

				targ.UpdateBar( testValue, 100.0f );
				if( barText.objectReferenceValue != null )
					EditorUtility.SetDirty( targ.barText );
			}

			if( EditorGUILayout.BeginFadeGroup( DisplayTextOption.faded ) )
			{
				EditorGUI.indentLevel = 1;

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( barText, new GUIContent( "Bar Text", "The Text component to be used for the text." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					targ.UpdateTextColor( textColor );
					targ.UpdateBar( testValue, 100.0f );
					if( barText.objectReferenceValue != null )
						EditorUtility.SetDirty( targ.barText );
				}

				EditorGUI.BeginChangeCheck();
				textColor = EditorGUILayout.ColorField( new GUIContent( "Text Color", "The color of the Text component." ), textColor );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					targ.UpdateTextColor( textColor );
					if( barText.objectReferenceValue != null )
						EditorUtility.SetDirty( targ.barText );
				}

				EditorGUI.BeginChangeCheck();
				EditorGUILayout.PropertyField( additionalText, new GUIContent( "Additional Text", "Additional text to be displayed before the current information." ) );
				if( EditorGUI.EndChangeCheck() )
				{
					serializedObject.ApplyModifiedProperties();
					targ.UpdateBar( testValue, 100.0f );
					if( barText.objectReferenceValue != null )
						EditorUtility.SetDirty( targ.barText );
				}

				EditorGUI.indentLevel = 2;
				switch( targ.displayText )
				{
					case SimpleHealthBar.DisplayText.Percentage:
					{
						EditorGUILayout.LabelField( "Text Preview: " + targ.additionalText + testValue + "%" );
					}
					break;
					case SimpleHealthBar.DisplayText.CurrentValue:
					{
						EditorGUILayout.LabelField( "Text Preview: " + targ.additionalText + testValue );
					}
					break;
					case SimpleHealthBar.DisplayText.CurrentAndMaxValues:
					{
						EditorGUILayout.LabelField( "Text Preview: " + targ.additionalText + testValue + " / 100" );
					}
					break;
					default:
					{
						EditorGUILayout.LabelField( "Text Preview: Default" );
					}
					break;
				}
				EditorGUI.indentLevel = 0;
				EditorGUILayout.Space();
			}
			if( ImageAssigned.faded == 1.0f )
				EditorGUILayout.EndFadeGroup();
			// ----- < END TEXT OPTIONS > ----- //

			// ----- < TEST VALUE > ----- //
			EditorGUI.BeginChangeCheck();
			testValue = EditorGUILayout.Slider( new GUIContent( "Test Value" ), testValue, 0.0f, 100.0f );
			if( EditorGUI.EndChangeCheck() )
			{
				if( targ.barImage != null )
				{
					targ.barImage.enabled = false;
					targ.UpdateBar( testValue, 100.0f );
					targ.barImage.enabled = true;

					EditorUtility.SetDirty( targ.barImage );
				}
			}
			// ----- < END TEST VALUE > ----- //
		}
		EditorGUILayout.EndFadeGroup();

		EditorGUILayout.Space();

		Repaint();
	}

	void StoreReferences ()
	{
		targ = ( SimpleHealthBar ) target;

		ImageAssigned = new AnimBool( GetImageAssigned() );
		ImageUnassigned = new AnimBool( GetImageUnassigned() );
		ImageFilledWarning = new AnimBool( GetBarImageWarning() );
		barImage = serializedObject.FindProperty( "barImage" );

		// ----->>> COLOR //
		ImageColorWarning = new AnimBool( GetColorWarning() );
		colorMode = serializedObject.FindProperty( "colorMode" );
		barColor = serializedObject.FindProperty( "barColor" );
		barGradient = serializedObject.FindProperty( "barGradient" );

		// ----->>> TEXT //
		DisplayTextOption = new AnimBool( targ.displayText != SimpleHealthBar.DisplayText.Disabled );
		textColor = targ.barText != null ? targ.barText.color : Color.white;
		barText = serializedObject.FindProperty( "barText" );
		displayText = serializedObject.FindProperty( "displayText" );
		additionalText = serializedObject.FindProperty( "additionalText" );

		// ---- < SCRIPT REFERENCE > ---- //
		ExampleCode = new AnimBool( barName != string.Empty );

		GenerateExampleCode();
	}

	void UpdateStatusColor ()
	{
		// If the image component is null, then return.
		if( targ.barImage == null )
			return;

		// Switch statement for the color mode option. Each case handles the color according to the option.
		switch( targ.colorMode )
		{
			case SimpleHealthBar.ColorMode.Single:
			{
				targ.barImage.color = targ.barColor;
			} break;
			case SimpleHealthBar.ColorMode.Gradient:
			{
				targ.barImage.color = targ.barGradient.Evaluate( targ.GetCurrentFraction );
			} break;
		}
		EditorUtility.SetDirty( targ.barImage );
	}

	bool GetImageAssigned ()
	{
		if( targ.barImage != null )
			return true;
		return false;
	}

	bool GetImageUnassigned ()
	{
		if( targ.barImage == null )
			return true;
		return false;
	}

	bool GetBarImageWarning ()
	{
		if( targ.barImage != null && targ.barImage.type != Image.Type.Filled )
			return true;
		return false;
	}

	bool GetColorWarning ()
	{
		if( Application.isPlaying == true )
			return false;

		if( targ.barImage == null )
			return false;

		if( targ.colorMode == SimpleHealthBar.ColorMode.Single && targ.barImage.color != targ.barColor )
			return true;

		return false;
	}

	void GenerateExampleCode ()
	{
		switch( functionList )
		{
			default:
			case FunctionList.UpdateBar:
			{
				exampleCode = barName + ".UpdateBar( current, max );";
			}
			break;
			case FunctionList.UpdateColor:
			{
				exampleCode = barName + ".UpdateColor( newColor );";
			}
			break;
			case FunctionList.UpdateTextColor:
			{
				exampleCode = barName + ".UpdateTextColor( newTextColor );";
			}
			break;
		}
	}
}