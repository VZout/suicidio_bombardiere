using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

	public void PlayGame() {
		Application.LoadLevel (1);
	}
	
	public void QuitGame() {
		Application.Quit ();
	}

	public void OpenLevel(int i) {
		Application.LoadLevel (i);
	}
}
