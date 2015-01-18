using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MazeBuilder : MonoBehaviour {

	static private Vector3[] verts;
	static private int[] tris;
	static private Vector3[] norms;
	static private Vector2[] uvs;
	static private int index;

	public static GameObject maze;
 
	public static void BuildMaze (Block[,,] blocks) {

		Debug.Log ("Building maze");

		maze = GameObject.Find ("Maze");

		int width  = blocks.GetLength (0);
		int height = blocks.GetLength (1);
		int depth  = blocks.GetLength (2);

		int amount = blocks.Length;

		verts = new Vector3[amount * 24];
		tris = new int[amount * 36];
		norms = new Vector3[verts.Length];
		uvs = new Vector2[verts.Length];

		//MeshRenderer meshRen = maze.GetComponent<MeshRenderer>();
		MeshCollider meshCol = maze.GetComponent<MeshCollider>();
		MeshFilter   meshFil = maze.GetComponent<MeshFilter>  ();


		for (int z = 0; z < depth; z++) {
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {

					Block loc = blocks[x,y,z];

					if (!(!loc.up && !loc.down && !loc.right && !loc.left && !loc.forward && !loc.backward)) {

						if (!loc.up) 		BuildFace (x, y + 1, z + 1, x, y + 1, z, x + 1, y + 1, z, x + 1, y + 1, z + 1, Vector3.down, 2);
						if (!loc.down) 		BuildFace (x, y, z, x, y, z + 1, x + 1, y, z + 1, x + 1, y, z, Vector3.up, 0);
						if (!loc.right)	 	BuildFace (x + 1,y , z + 1, x + 1, y + 1, z + 1, x + 1, y + 1, z, x + 1, y, z, Vector3.left, 1);
						if (!loc.left)  	BuildFace (x, y, z, x, y + 1, z, x, y + 1, z + 1, x, y, z + 1, Vector3.right, 1);
						if (!loc.forward) 	BuildFace (x, y, z + 1, x, y + 1, z + 1, x + 1, y + 1, z + 1, x + 1, y, z + 1, Vector3.forward, 1);
						if (!loc.backward)  BuildFace (x + 1, y, z, x + 1, y + 1, z, x, y + 1, z, x, y, z, Vector3.back, 1);

					}

				}
			}
		}

		Mesh mesh = new Mesh ();

		mesh.vertices 	= verts;
		mesh.triangles 	= tris;
		mesh.normals 	= norms;
		mesh.uv 		= uvs;

		mesh.RecalculateBounds ();
		mesh.RecalculateNormals ();

		meshFil.mesh = mesh;
		meshCol.sharedMesh = meshFil.mesh;
	}

	static void BuildFace (int x, int y, int z,int ux,int uy,int uz,int urx,int ury,int urz,int rx,int ry,int rz, Vector3 n, int tex) {

		verts[index * 4 + 0] = new Vector3 (x,y,z); //1,1,0
		verts[index * 4 + 1] = new Vector3 (ux,uy,uz); //1,0,0
		verts[index * 4 + 2] = new Vector3 (urx,ury,urz); //0,0,0
		verts[index * 4 + 3] = new Vector3 (rx,ry,rz); //0,1,0

		tris[index * 6 + 0] = (index * 4 + 1);
		tris[index * 6 + 1] = (index * 4 + 3);
		tris[index * 6 + 2] = (index * 4 + 0);
		
		tris[index * 6 + 3] = (index * 4 + 1);
		tris[index * 6 + 4] = (index * 4 + 2);
		tris[index * 6 + 5] = (index * 4 + 3);

		float s = 1f/4f;

		norms[index * 4 + 0] = n;
		norms[index * 4 + 1] = n;
		norms[index * 4 + 2] = n;
		norms[index * 4 + 3] = n;

		uvs[index * 4 + 0] = (new Vector2 (s * (float)tex,0));
		uvs[index * 4 + 1] = (new Vector2 (s * (float)tex,1));
		uvs[index * 4 + 2] = (new Vector2 (s * (float)tex + s,1));
		uvs[index * 4 + 3] = (new Vector2 (s * (float)tex + s,0));

		index++;

	}

	void OnDrawGizmos () {
		for (int i=0;i<verts.Length;i++) {
			Gizmos.DrawSphere (verts[i], 0.25f);
		}
	}
}
