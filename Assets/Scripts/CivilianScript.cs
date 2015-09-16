using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CivilianScript : MonoBehaviour {

	Rigidbody[] rbs;
	public float speed = 1;
	public bool alive = true;
	public float changeDirDelay = 4;
	public float rotateSpeed = 3;
	public float maxDistanceToWall = 2;
	private Quaternion targetRotation;

	public AudioSource[] hurts;
	
	void Start () {
		targetRotation = transform.rotation;
		rbs = GetComponentsInChildren<Rigidbody>();

		for(int i = 0; i < rbs.Length; i++) {
			rbs[i].constraints = RigidbodyConstraints.FreezeAll;
		}

		StartCoroutine(ChangeDirection());
	}

	void Update () {
		if (alive) {
			//Movement
			transform.Translate (new Vector3(speed, 0, 0) * Time.deltaTime);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

			//Raycast
			RaycastHit hit;
			if(Physics.Raycast(transform.position, transform.right, out hit, maxDistanceToWall, LayerMask.GetMask("Enviroment"))) {
				FlipDirection();
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
			rbs[i].constraints = RigidbodyConstraints.None;
		}
		GetComponent<Animator> ().enabled = !GetComponent<Animator> ().enabled;
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
