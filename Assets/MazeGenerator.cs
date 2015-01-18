using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeGenerator : MonoBehaviour {

	public Block[,,] blocks;

	public intVector3 start;
	public intVector3 end;

	private int width;
	private int height;
	private int depth;

	public GameObject winCube;
	public float scale;

	public List<Vector3> path = new List<Vector3>();

	public intVector3[] nearby;

	// Use this for initialization
	void Start () {
		InitializeBlocks ();
		GenerateMaze ();
		MazeBuilder.BuildMaze (blocks);
	}

	void InitializeBlocks () {

		Debug.Log ("Initializing blocks");

		blocks = new Block[13,13,13];

		width  = blocks.GetLength (0);
		height = blocks.GetLength (1);
		depth  = blocks.GetLength (2);

		for (int z = 0; z < depth; z++) {
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {

					blocks[x,y,z] = new Block ();
				
				}
			}
		}
	}

	public intVector3 current = new intVector3 ();
	
	void GenerateMaze () {
		Debug.Log ("Generating Maze");

		GenerateRoads (true);
		GenerateSubroads ();
		ExportPathToCameraPather ();
		PlaceWinCube ();
	}

	void PlaceWinCube () {
		winCube.transform.position = new Vector3 (end.x, end.y, end.z) * scale + new Vector3 (scale, scale, scale) / 2;
	}

	void GenerateRoads (bool isStarting) {

		current = new intVector3 (start.x, start.y, start.z);
		if (isStarting) ChangeBlock (current.x, current.y, current.z, Vector3.back);
		if (isStarting) path.Add (new Vector3 (current.x, current.y, current.z));
		
		int maxLoops = 2000;
		while ( IsWithinMaze (current.x, current.y, current.z) && maxLoops > 0) {

			intVector3 direction = nearby[Random.Range (0, nearby.Length)];

			if (IsEmpty (current.x + direction.x, current.y + direction.y, current.z + direction.z) && IsWithinMaze (current.x + direction.x, current.y + direction.y, current.z + direction.z)) {
				ChangeBlock (current.x,current.y,current.z, new Vector3 (direction.x, direction.y, direction.z));
				ChangeBlock (current.x + direction.x,current.y + direction.y,current.z + direction.z, new Vector3 (-direction.x, -direction.y, -direction.z));

				current = new intVector3 (current.x + direction.x, current.y + direction.y, current.z + direction.z);

				if (isStarting) {
					path.Add (new Vector3 (current.x, current.y, current.z) * scale + new Vector3 (scale, scale, scale) / 2);
					end = new intVector3 (current.x, current.y, current.z);
				}
			}

			maxLoops--;

		}

	}

	void GenerateSubroads () {
		int maxLoops = 2000;
		while (IsAnyEmpty () && maxLoops > 0) {
			Vector3 point = path[Random.Range (0, path.Count)];
			start.x = (int)point.x;
			start.y = (int)point.y;
			start.z = (int)point.z;
			GenerateRoads (false);
			maxLoops--;
		}
	}

	bool IsWithinMaze (int x, int y, int z) {
		if (x < 0 || x >= width) return false;
		if (y < 0 || y >= height) return false;
		if (z < 0 || z >= depth) return false;
		return true;
	}

	bool IsEmpty (int x, int y, int z) {
		if (IsWithinMaze (x,y,z) ) {
			Block loc = blocks[x,y,z];
			if (!(!loc.up && !loc.down && !loc.right && !loc.left && !loc.forward && !loc.backward)) return false;
		}
		return true;
	}

	void ChangeBlock (int x, int y, int z, Vector3 direction) {

		if (IsWithinMaze (x,y,z)) {
			Block block = blocks[x,y,z];

			if (direction.x  > 0.1) block.right    = true;
			if (direction.x < -0.1) block.left     = true;
			if (direction.y  > 0.1) block.up       = true;
			if (direction.y < -0.1) block.down     = true;
			if (direction.z  > 0.1) block.forward  = true;
			if (direction.z < -0.1) block.backward = true;
		}

	}

	bool IsAnyEmpty () {
		for (int z = 0; z < depth; z++) {
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					if (IsEmpty (x,y,z)) return true;
				}
			}
		}

		return false;
	}

	void ExportPathToCameraPather () {
		CameraPather cam = Camera.main.GetComponent<CameraPather>();
		cam.path = path.ToArray ();
		cam.StartMaze ();
	}

	void OnDrawGizmos () {

		for (int i=0;i<path.Count-1;i++) {
			Gizmos.DrawLine (path[i],path[i+1]);
		}

	}

	// There is a 100% chance that shit's mispilled

}
