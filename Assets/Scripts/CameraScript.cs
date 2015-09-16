using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour {
	
	public Transform target;
	public float dampTime;
	private Vector3 velocity = Vector3.zero;
	
	public float offsetX = 0;
	
	public float yStartHeight = 2;
	public Quaternion startRotation;
	
	void Start() {
		yStartHeight = transform.position.y;
	}
	
	void FixedUpdate() {
		if(target) {
			
			Vector3 destination = target.position;
			
			transform.position = Vector3.SmoothDamp(transform.position, new Vector3(destination.x - offsetX, yStartHeight, destination.z), ref velocity, dampTime);
		}
	}
}
