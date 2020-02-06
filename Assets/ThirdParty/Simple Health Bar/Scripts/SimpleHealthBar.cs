/* Written by Kaz Crowe */
/* SimpleHealthBar.cs */
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[AddComponentMenu( "UI/Simple Health Bar/Simple Health Bar" )]
public class SimpleHealthBar : MonoBehaviour
{	
	// COLOR OPTIONS //
	public Image barImage;
	public enum ColorMode
	{
		Single,
		Gradient
	}
	public ColorMode colorMode;
	public Color barColor = Color.white;
	public Gradient barGradient = new Gradient();

	// TEXT OPTIONS //
	public enum DisplayText
	{
		Disabled,
		Percentage,
		CurrentValue,
		CurrentAndMaxValues
	}
	public DisplayText displayText;
	public Text barText;
	public string additionalText = string.Empty;

	// PRIVATE VARIABLES AND GET FUNCTIONS //
	float _currentFraction = 1.0f;
	/// <summary>
	/// Returns the percentage value that was calculated when the bar was updated. This number will not be current with the Smooth Fill option.
	/// </summary>
	public float GetCurrentFraction
	{
		get
		{
			return _currentFraction;
		}
	}

	/// <summary>
	/// The stored max value that the user entered.
	/// </summary>
	float _maxValue = 0.0f;

	/// <summary>
	/// This float stores the target amount of fill. This value is current with Fill Constraints.
	/// </summary>
	float targetFill = 0.0f;


	/// <summary>
	/// Displays the text.
	/// </summary>
	void DisplayTextHandler ()
	{
		// If the user does not want text to be displayed, or the text component is null, then return.
		if( displayText == DisplayText.Disabled || barText == null )
			return;

		// Switch statement for the displayText option. Each option will display the correct text for the set option.
		switch( displayText )
		{
			case DisplayText.Percentage:
			{
				barText.text = additionalText + ( GetCurrentFraction * 100 ).ToString( "F0" ) + "%";
			}break;
			case DisplayText.CurrentValue:
			{
				barText.text = additionalText + ( GetCurrentFraction * _maxValue ).ToString( "F0" );
			}break;
			case DisplayText.CurrentAndMaxValues:
			{
				barText.text = additionalText + ( GetCurrentFraction * _maxValue ).ToString( "F0" ) + " / " + _maxValue.ToString();
			}break;
		}
	}

	/// <summary>
	/// Update the color of the bar according to the gradient.
	/// </summary>
	void UpdateGradient ()
	{
		// If the color mode is set to Gradient, then apply the current gradient color.
		if( colorMode == ColorMode.Gradient )
			barImage.color = barGradient.Evaluate( GetCurrentFraction );
	}

	/// <summary>
	/// Updates the options.
	/// </summary>
	void UpdateOptions ()
	{
		UpdateGradient();
		DisplayTextHandler();
	}

	#region PUBLIC FUNCTIONS
	/// <summary>
	/// Updates the health bar with the current and max values.
	/// </summary>
	/// <param name="currentValue">The current value of the bar.</param>
	/// <param name="maxValue">The maximum value of the bar.</param>
	public void UpdateBar ( float currentValue, float maxValue )
	{
		// If the bar image is left unassigned, then return.
		if( barImage == null )
			return;
			
		// Fix the value to be a percentage.
		_currentFraction = currentValue / maxValue;

		// If the value is greater than 1 or less than 0, then fix the values to being min/max.
		if( _currentFraction < 0 || _currentFraction > 1 )
			_currentFraction = _currentFraction < 0 ? 0 : 1;

		// Store the target amount of fill according to the users options.
		targetFill = _currentFraction;

		// Store the values so that other functions used can reference the maxValue.
		_maxValue = maxValue;

		// Then just apply the target fill amount.
		barImage.fillAmount = targetFill;

		// Call the functions for the options.
		UpdateOptions();
	}

	/// <summary>
	/// Updates the color of the bar with the target color.
	/// </summary>
	/// <param name="targetColor">The target color to apply to the bar.</param>
	public void UpdateColor ( Color targetColor )
	{
		// If the color is not set to single, then return.
		if( colorMode != ColorMode.Single || barImage == null )
			return;

		// Set the bar color to the new target color and apply it to the bar.
		barColor = targetColor;
		barImage.color = barColor;
	}

	/// <summary>
	/// Updates the gradient of the bar with the target gradient.
	/// </summary>
	/// <param name="targetGradient">The target gradient to apply to the bar.</param>
	public void UpdateColor ( Gradient targetGradient )
	{
		// If the color is not set to gradient, then return.
		if( colorMode != ColorMode.Gradient || barImage == null )
			return;

		barGradient = targetGradient;
		UpdateGradient();
	}

	/// <summary>
	/// Updates the color of the text associated with the bar.
	/// </summary>
	/// <param name="targetColor">The target color to apply to the text component.</param>
	public void UpdateTextColor ( Color targetColor )
	{
		// If the user is not wanting the text to be displayed, or the text component is not assigned, then return.
		if( displayText == DisplayText.Disabled || barText == null)
			return;

		// Set the text color to the new target color and apply it to the text component.
		barText.color = targetColor;
	}
	#endregion
}