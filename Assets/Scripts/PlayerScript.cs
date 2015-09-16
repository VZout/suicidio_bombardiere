using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {

	public GameObject explosion;
	public GameObject bullet;

	public int health = 100;

	public float rotationSmoothing = 5;
	public float speed = 3;
	public float alternativeSpeed = 2;
	public float maxSpeed = 5;
	public float fireRate = 0.2f;

	private Vector3 lookTarget;
	private Rigidbody body;
	private Animator anim;

	private bool inputH = false;
	private bool inputV = false;

	private bool readyToFire = true;

	public float bombRange = 5;

	private bool alive = true;

	public Slider healthSlider;
	
	void Start ()
	{
		body = GetComponent<Rigidbody> ();
		anim = GetComponent<Animator> ();
	}

	void Update() {
		healthSlider.value = health;
		if (alive) {
			if (inputH || inputV)
				anim.SetBool ("Walking", true);
			else
				anim.SetBool ("Walking", false);

			if(Input.GetKeyDown(KeyCode.F5))
				GameObject.FindGameObjectWithTag("Global").GetComponent<GameManager>().despawnBodies = !GameObject.FindGameObjectWithTag("Global").GetComponent<GameManager>().despawnBodies;

			if (Input.GetKey (KeyCode.W) || Input.GetKeyDown (KeyCode.UpArrow)) {
				MoveForward ();
				inputH = true;
			} else if (Input.GetKey (KeyCode.S) || Input.GetKeyDown (KeyCode.DownArrow)) {
				MoveBackward ();
				inputH = true;
			} else {
				inputH = false;
			}

			if (Input.GetKey (KeyCode.A) || Input.GetKeyDown (KeyCode.LeftArrow)) {
				MoveRight ();
				inputV = true;
			} else if (Input.GetKey (KeyCode.D) || Input.GetKeyDown (KeyCode.RightArrow)) {
				MoveLeft ();
				inputV = true;
			} else {
				inputV = false;
			}
		
			if (Input.GetKeyDown (KeyCode.Space))
				Explosion ();

			if (Input.GetMouseButton (0))
				Fire ();

			if(health <= 0 )
				Application.LoadLevel(2);
		}

	}

	void FixedUpdate() {
		if (alive) {
			LookTowardsMouse ();

			if (body.velocity.magnitude > maxSpeed) {
				body.velocity = body.velocity.normalized * maxSpeed;
			}
		}
	}

	void LookTowardsMouse() {
		var ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit)) {
			lookTarget = hit.point;
		}
		
		Vector3 lookDelta = (hit.point-transform.position);
		Quaternion targetRot = Quaternion.LookRotation(lookDelta);
		transform.rotation = Quaternion.Lerp (transform.rotation, targetRot, rotationSmoothing * Time.deltaTime);
		transform.rotation = Quaternion.Euler (0, transform.rotation.eulerAngles.y, 0);
	}

	public void MoveForward() {
		body.AddForce(transform.forward * speed * Time.deltaTime, ForceMode.Impulse);
	}

	public void MoveBackward() {
		body.AddForce(-transform.forward * alternativeSpeed * Time.deltaTime, ForceMode.Impulse);
	}

	public void MoveLeft() {
		body.AddForce(transform.right * alternativeSpeed * Time.deltaTime, ForceMode.Impulse);
	}

	public void MoveRight() {
		body.AddForce(-transform.right * alternativeSpeed * Time.deltaTime, ForceMode.Impulse);
	}

	public void Explosion() {
		Instantiate (explosion, transform.position, transform.rotation);
		this.gameObject.SetActive (false);
		alive = false;

		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");

		for (int i = 0; i < enemies.Length; i++) {
			if (Vector3.Distance (transform.position, enemies [i].transform.position) <= bombRange) {
				if (enemies [i].gameObject.layer == 11) {
					enemies [i].GetComponent<PoliceScript> ().Kill ();	
					GameObject.FindGameObjectWithTag("Global").GetComponent<GameManager>().score += 10;
				}
				if (enemies [i].gameObject.layer == 12) {
					enemies [i].GetComponent<CivilianScript> ().Kill ();
					GameObject.FindGameObjectWithTag("Global").GetComponent<GameManager>().score += 10;
				}
			}
		}

		Invoke ("GameOver", 5);
	}

	public void Fire() {
		if (readyToFire) {
			Instantiate (bullet, new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z), transform.rotation);
			StartCoroutine(SetReadyToFire());
			readyToFire = false;
		}
	}

	IEnumerator SetReadyToFire() {
		yield return new WaitForSeconds(fireRate);
		readyToFire = true;
	}

	private void GameOver() {
		Application.LoadLevel(2);
	}

	public void Hurt(int amount) {
		health -= amount;
	}

}
