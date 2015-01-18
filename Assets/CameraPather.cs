using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraPather : MonoBehaviour {

	public Vector3[] path;
	public float pathProgress;

	public float targetTime;
	private float speed;
	public float lookPos;

	private Transform pointer;
	private float future;

	// Use this for initialization
	void Start () {
		pointer = new GameObject ("Pointer").transform;
	}

	public void StartMaze () {
		speed = CalculatePathLength () / targetTime;
	}

	float CalculatePathLength () {
		float length = 0;
		for (int i=0;i<path.Length-1;i++) {
			length += Vector3.Distance (path[i], path[i+1]);
		}
		return length;
	}
	
	// Update is called once per frame
	void Update () {
		if (Vector3.Distance (pointer.position, path[path.Length-1]) > 1) {
			Vector3 prevPath = path[Mathf.FloorToInt (pathProgress)];
			Vector3 nextPath = path[Mathf.CeilToInt  (pathProgress)];
			
			pointer.position = Vector3.Lerp (prevPath, nextPath, pathProgress % 1);

			float mul = Time.deltaTime / Vector3.Distance (prevPath, nextPath);
			pathProgress += mul * speed;

			prevPath = path[Mathf.FloorToInt (pathProgress + lookPos * mul)];
			nextPath = path[Mathf.CeilToInt  (pathProgress + lookPos * mul)];

			mul = Time.deltaTime / Vector3.Distance (prevPath, nextPath);
			float future = pathProgress + lookPos * mul;

			transform.position = Vector3.Lerp (transform.position, pointer.position, speed / 5 * Time.deltaTime);

			pointer.rotation = Quaternion.RotateTowards (pointer.rotation, (Quaternion.LookRotation ((Vector3.Lerp (prevPath, nextPath, future) - transform.position).normalized)), speed * 10 * Time.deltaTime);
			Debug.DrawLine (transform.position, Vector3.Lerp (prevPath, nextPath, future % 1));
			Debug.DrawLine (transform.position, pointer.forward * 5);
			transform.rotation = Quaternion.Slerp (transform.rotation, pointer.rotation, speed / 3 * Time.deltaTime);
			transform.GetChild (0).rotation = Quaternion.Slerp (transform.GetChild (0).rotation, Quaternion.LookRotation ((pointer.position - transform.GetChild (0).position).normalized), speed * Time.deltaTime);
		}

		if (Input.GetButtonDown ("Fire1")) Application.LoadLevel (Application.loadedLevel);
		if (Input.GetButtonDown ("Fire2")) Application.Quit ();
	}

	void OnDrawGizmos () {
		if (pointer) Gizmos.DrawLine (transform.position, pointer.position);
		Gizmos.DrawSphere (transform.position, 0.5f);
	}

	void FindDummyPath (int pathTries) {
		List<Vector3> newPath = new List<Vector3>();
		newPath.Add (Vector3.zero);

		for (int i = 1; i < pathTries; i++) {

			int distance = pathTries;

			Vector3 tar = Random.insideUnitSphere * distance;
			RaycastHit hit;

			if (!Physics.Raycast (new Ray (newPath[i-1], (tar - newPath[i-1]).normalized), out hit, distance)) {
				newPath.Add (tar);
			}else{
				i--;
			}
		}

		path = newPath.ToArray ();
		Debug.Log ("Added " + newPath.Count + " waypoints");
	}

}
