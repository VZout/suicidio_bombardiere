using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

	private Rigidbody body;
	public GameObject police;
	public bool friendly = true;
	public float speed = 3;
	public int damage = 10;
	public ParticleSystem[] particleSystems;
	
	void Start () {

		if (friendly) {
			gameObject.layer = 10;
		}
		else {
			gameObject.layer = 13;
		}

		Physics.IgnoreLayerCollision (9, 10);
		Physics.IgnoreLayerCollision (11, 13);
		Physics.IgnoreLayerCollision (10, 13);

		body = GetComponent<Rigidbody> ();
		body.AddForce (transform.forward * speed, ForceMode.VelocityChange);
	}

	void OnCollisionEnter (Collision coll) {
		if (coll.gameObject.layer == 8) {
			KillBullet();
		}
		if (coll.gameObject.layer == 9 && !friendly) {
			KillBullet();
			coll.gameObject.GetComponent<PlayerScript>().Hurt(damage);
		}
		if(coll.gameObject.layer == 11 && friendly) {
			KillBullet();
			if(coll.gameObject.GetComponentInParent<PoliceScript>().alive) {
				GameObject.FindGameObjectWithTag("Global").GetComponent<GameManager>().score += 10;
				coll.gameObject.GetComponentInParent<PoliceScript>().Kill();
				GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
				Instantiate(police, spawnPoints[Random.Range(0,spawnPoints.Length)].transform.position , Quaternion.Euler(0,Random.Range(0,360),0));
				Instantiate(police, spawnPoints[Random.Range(0,spawnPoints.Length)].transform.position , Quaternion.Euler(0,Random.Range(0,360),0));
			}
		}
		if(coll.gameObject.layer == 12 && friendly) {
			KillBullet();
			if(coll.gameObject.GetComponentInParent<CivilianScript>().alive) {
				GameObject.FindGameObjectWithTag("Global").GetComponent<GameManager>().score += 10;
				coll.gameObject.GetComponentInParent<CivilianScript>().Kill();
				GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
				Instantiate(police, spawnPoints[Random.Range(0,spawnPoints.Length)].transform.position , Quaternion.Euler(0,Random.Range(0,360),0));
			}
		}
	}

	void KillBullet() {
		for(int i = 0; i < particleSystems.Length; i++) {
			particleSystems[i].enableEmission = false;
		}
		
		GetComponent<Collider>().enabled = false;
		GetComponent<Renderer>().enabled = false;
		GetComponent<Light>().enabled = false;
		
		Destroy(this.gameObject, 1.5f);
	}
}
