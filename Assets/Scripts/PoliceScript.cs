using UnityEngine;
using System.Collections;

public class PoliceScript : MonoBehaviour {

	Rigidbody[] rbs;
	public GameObject bullet;
	public float speed = 1;
	public bool alive = true;
	private bool readyToFire = true;
	public float fireRate = 0.5f;
	public float range = 10;
	private Transform player;
	public float changeDirDelay = 4;
	public float rotateSpeed = 3;
	public float maxDistanceToWall = 2;
	private Quaternion targetRotation;
	public AudioSource[] hurts;

	private Vector3 dir;
	private Quaternion lookRotation;
	
	void Start () {
		rbs = GetComponentsInChildren<Rigidbody>();
		player = GameObject.FindGameObjectWithTag ("Player").transform;
		for(int i = 0; i < rbs.Length; i++) {
			//rbs[i].isKinematic = true;
			rbs[i].constraints = RigidbodyConstraints.FreezeAll;
		}
		StartCoroutine (ChangeDirection());
	}

	void Update () {
		if (alive) {
			if(GetLineOfSight()) {
				dir = (player.position - transform.position).normalized;
				lookRotation = Quaternion.LookRotation (dir);
				transform.rotation = lookRotation;

				if (Vector3.Distance (player.position, transform.position) < range) {
					Fire ();
					GetComponent<Animator> ().SetBool ("Walking", false);
				} else {
					transform.Translate (new Vector3 (0, 0, speed) * Time.deltaTime);
					GetComponent<Animator> ().SetBool ("Walking", true);
				}
			} else {
				GetComponent<Animator> ().SetBool ("Walking", true);
				//Movement
				transform.Translate (new Vector3(0, 0, speed) * Time.deltaTime);
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
				
				//Raycast
				RaycastHit hit;
				if(Physics.Raycast(transform.position, transform.forward, out hit, maxDistanceToWall, LayerMask.GetMask("Enviroment"))) {
					FlipDirection();
				}
			}
		}
	}
	
	IEnumerator ChangeDirection() {
		yield return new WaitForSeconds(changeDirDelay);
		targetRotation = Quaternion.Euler(new Vector3(0, Random.rotation.eulerAngles.y, 0));
		StartCoroutine(ChangeDirection());
	}
	
	public void FlipDirection() {
		transform.RotateAround (transform.position, transform.up, 180f);
		targetRotation = transform.rotation;
	}

	public bool GetLineOfSight() {
		RaycastHit hit;
		Vector3 rayDirection = player.position - transform.position;
		if (Physics.Raycast (transform.position, rayDirection, out hit)) {
			if (hit.transform == player) {
				return true;
			} else {
				return false;
			}
		} else {
			return false;
		}
	}

	public void Kill() {
		if (alive){
			PlayDeathSound();
			alive = false;
			ToggleRagdoll ();
			StartCoroutine(Remove ());
		}
	}
	
	public void ToggleRagdoll() {
		for(int i = 0; i < rbs.Length; i++) {
			//rbs[i].isKinematic = !rbs[i].isKinematic;
			rbs[i].constraints = RigidbodyConstraints.None;
		}	
		GetComponent<Animator> ().enabled = !GetComponent<Animator> ().enabled;
	}

	private void Fire() {
		if (readyToFire) {
			GameObject b = Instantiate (bullet, transform.position, transform.rotation) as GameObject;
			b.GetComponent<BulletScript>().friendly = false;
			StartCoroutine(SetReadyToFire());
			readyToFire = false;
		}
	}

	IEnumerator SetReadyToFire() {
		yield return new WaitForSeconds(fireRate);
		readyToFire = true;
	}

	IEnumerator Remove() {
		yield return new WaitForSeconds(GameObject.FindGameObjectWithTag("Global").GetComponent<GameManager>().despawnDelay);
		if (GameObject.FindGameObjectWithTag("Global").GetComponent<GameManager>().despawnBodies) {
			
			Collider[] cls = GetComponentsInChildren<Collider>();
			
			for(int i = 0; i < cls.Length; i++) {
				cls[i].enabled = false;
			}
			
			Destroy(this.gameObject, 1.2f);
		}
	}

	public void PlayDeathSound() {
		hurts [Random.Range (0, hurts.Length)].Play ();
	}
}
