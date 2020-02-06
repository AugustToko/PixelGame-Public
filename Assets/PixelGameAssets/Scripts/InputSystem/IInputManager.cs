namespace PixelGameAssets.Scripts.InputSystem
{
	public interface IInputManager {

		bool isEnabled { get; set; }
		bool GetButton (int playerId, InputAction action);
		bool GetButtonDown (int playerId, InputAction action);
		bool GetButtonUp (int playerId, InputAction action);
		float GetAxis (int playerId, InputAction action);
	}
}	
