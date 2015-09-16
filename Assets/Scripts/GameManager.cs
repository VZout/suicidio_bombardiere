using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	public bool despawnBodies = true;
	public float despawnDelay = 10;
	public int score;
	public GameObject scoreText;

	void Start() {
		DontDestroyOnLoad (this.gameObject);
	}

	void Update() {
		if (Application.loadedLevel == 1) {
			scoreText = GameObject.FindGameObjectWithTag("UIScore");
			scoreText.GetComponent<Text>().text = "Score: " + score;
		}

		else if (Application.loadedLevel == 2) {
			scoreText = GameObject.FindGameObjectWithTag("UIScore");
			scoreText.GetComponent<Text>().text = "Score: " + score;
		}
	}
}
