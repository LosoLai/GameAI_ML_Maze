using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Grid Controller instantiates and manages Waypoints, as well as receiving requests to calculate A* paths
///  from one game object to another.
/// </summary>

public class GridController : MonoBehaviour {

	// The dimensions of the gri
	public int xSize;
	public int zSize;
	public float offset;

	// The ground object to calc the height of current location
	public Terrain ground;

	// The prefab to spawn
	public GameObject waypoint;

	//
	private List<GameObject> waypoints = new List<GameObject> ();
	private GameObject[,] grid;

	void Awake() {
		GenerateGrid ();
	}
		
	// 
	private void GenerateGrid() {
		grid = new GameObject[xSize + 1, zSize + 1];

		// Build the grid
		for (int i = 0, x = 0; x <= xSize; x++) {
			for (int z = 0; z <= zSize; z++, i++) {
				float _x = (x * 2) - offset;
				float _z = (z * 2) - offset;
				float _y = ground.SampleHeight (new Vector3 (_x, 0f, _z));
				GameObject newWaypoint = Instantiate (waypoint, new Vector3 (_x, _y + 0.5f, _z), Quaternion.identity);
				newWaypoint.GetComponent<WaypointController> ().SetID (i);
				grid [x,z] = newWaypoint;
			}
		}

		// After they are all built, go back and set the neighbours
		for (int x = 0; x <= xSize; x++) {
			for (int z = 0; z <= zSize; z++) {
				// Get the WaypointController on each, and set it to the List<Waypoints>
				grid [x,z].GetComponent<WaypointController> ().SetNeighbours (GetNeighbours (x, z));
			}
		}	
	}

	// Reset all the waypoints back to
	// Used in the demos to render the waypoints, color them, and repeat on update.
	public void ResetColors() {
		for (int x = 0; x <= xSize; x++) {
			for (int z = 0; z <= zSize; z++) {
				grid [x, z].GetComponent<Renderer> ().material.color = new Color (255, 255, 255);
			}
		}
	}

	// Calculate the neighbours for a waypoint
	private List<GameObject> GetNeighbours(int x, int z) {
		List<GameObject> neighbours = new List<GameObject> ();
		if (x > 0) {
			neighbours.Add (grid [x - 1, z]); // MIDDLE LEFT

			if (z > 0) {
				neighbours.Add (grid [x - 1, z - 1]); // TOP LEFT
			}
			if (z < zSize) {
				neighbours.Add (grid [x - 1, z + 1]); // BOTTOM LEFT
			}
		}
		if (x < xSize) {
			neighbours.Add (grid [x + 1, z]); // MIDDLE RIGHT

			if (z > 0) {
				neighbours.Add (grid [x + 1, z - 1]); // TOP RIGHT
			}
			if (z < zSize) {
				neighbours.Add (grid [x + 1, z + 1]); // BOTTOM RIGHT
			}
		}
		if (z > 1) {
			neighbours.Add (grid [x, z - 1]); // TOP MIDDLE
		}
		if (z < zSize) {
			neighbours.Add (grid [x, z + 1]); // BOTTOM MIDDLE
		}
		return neighbours;
	}

	// Used to get from the GameObject to a Waypoint, so we can A* across the grid.
	// Because the GameObject wont have all the functions we need on it.
	private GameObject GetNearestWaypoint(Transform obj) {
		// Nearest waypoint by rounding off the float portion and using that as the index.
		// Basically just reverse the math in instantiating them.
		int x = (int)(Mathf.Round (obj.transform.position.x) + offset) / 2;
		int z = (int)(Mathf.Round(obj.transform.position.z) + offset) / 2;

		// Just sanity checks in case we have the player right on the edge of the maps.
		if (x < 0) {
			x = 0;
		} else if (x > xSize) {
			x = xSize;
		}
		if (z < 0) {
			z = 0;
		} else if (z > zSize) {
			z = zSize;
		}
		return grid[x, z];
	}
		
	// Built using https://en.wikipedia.org/wiki/A*_search_algorithm#Pseudocode
	public List<Vector3> AStar(GameObject origin, GameObject end) {
		
		// This is the list of nodes that we return.
		List<Vector3> path = new List<Vector3> (); 
		Dictionary<GameObject, GameObject> cameFrom = new Dictionary<GameObject, GameObject>();

		List<GameObject> closedSet = new List<GameObject>();
		PriorityQueue openSet = new PriorityQueue ();
		Dictionary<GameObject, float> g = new Dictionary<GameObject, float>();

		GameObject start = GetNearestWaypoint (origin.transform);
		Debug.DrawLine (origin.transform.position, start.transform.position, Color.green, 5);
		//
		openSet.Push (start, 0);
		g.Add(start, 0);

		bool sort; // for triggering openSet.Sort();

		// Safety counter to prevent infinite loops
		int counter = 999;

		// While we have open nodes
		while (!openSet.IsEmpty () && counter > 0) {
			counter--;

			GameObject current = openSet.Pop (); 

			// If we found the end
			if (Vector3.Distance(current.transform.position, end.transform.position) < 2f) {
				// Then begin backtracing until we get to the start
				while (current != start) {
					path.Add (current.transform.position);
					GameObject parent = cameFrom[current];
					current = parent;
				}
				path.Reverse();
				return path;
			}

			// If we got here, then it is a waypoint.
			WaypointController current_controller = current.GetComponent<WaypointController> ();

			// Graduate this node to the closed set.
			closedSet.Add(current);
			sort = false;
		
			foreach (GameObject neighbour in current_controller.GetNeighbours ()) {
				WaypointController neighbour_controller = neighbour.GetComponent<WaypointController> ();

				// If this neighbour is currently colliding with an obstacle
				if (!neighbour_controller.IsValidNeighbour (current.transform.position.y)) {
					continue;
				}

				// If this neighbour is already in the closed set
				if (closedSet.Contains(neighbour)) {
					continue;
				}

				neighbour.GetComponent<Renderer> ().material.color = new Color (0, 0, 255);

				// It's possible this new path is better
				float tentative = g[current] + Vector3.Distance(current.transform.position, neighbour.transform.position);

				// We've already seen it, and that time it was better.
				if (g.ContainsKey(neighbour) && tentative >= g[neighbour]) {
					continue;
				}

				cameFrom [neighbour] = current;
				g[neighbour] = tentative;

				// This push is condition in the PriorityQueue class, so just Push unlike 
				openSet.Push(neighbour, tentative + Vector3.Distance(neighbour.transform.position, end.transform.position));
				sort = true; // We made a change, therefore we need to sort.
			}

			// If we had a neighbour that changed something
			// Then request a sort of the priority queue
			if (sort) {
				openSet.Sort ();
			}
		}

		// We should never get here. That means either;
		// (1) Counter ran out, or (2) No path exists.
		print ("Counter ran out at " + counter);
		return path;
	}
}


// Wrote this for a* because it isn' a built-in
class PriorityQueue {
	private List<GameObject> waypoints = new List<GameObject>();
	private List<float> distances = new List<float>();

	// Check if the waypoint is already in there, if so update
	// Useful because checking if c(x) + h(x) <= g(x)
	//  is already done in the main a* algorithm
	public void Push(GameObject waypoint, float distance) {
		int i = waypoints.IndexOf (waypoint);
		if (i == -1) {
			waypoints.Add (waypoint);
			distances.Add (distance);
		} else {
			waypoints [i] = waypoint;
			distances [i] = distance;
		}
	}

	// Returns the item with the lowest priority
	public GameObject Pop() {
		GameObject waypoint = waypoints [0];
		waypoints.RemoveAt(0);
		distances.RemoveAt(0);
		return waypoint;
	}

	// This is for the "Does a path really exist" check 
	public bool IsEmpty() {
		return waypoints.Count == 0;
	}

	// Used in Quicksort function below to swap indices
	private void Swap(int a, int b) {
		GameObject _w = waypoints [a];
		waypoints [a] = waypoints [b];
		waypoints [b] = _w;

		float _d = distances [a];
		distances [a] = distances [b];
		distances [b] = _d;
	}

	// Request a sort
	public void Sort() {
		Quicksort (0, waypoints.Count - 1);
	}

	// Recursive quick sort
	// Adapted from snipd.net/quicksort-in-c
	private void Quicksort(int left, int right) {
		int l = left;
		int r = right;
		float pivot = distances [(l + r) / 2];

		while (l <= r) {
			while (distances [l] < pivot) {
				l++;
			}
			while (distances [r] > pivot) {
				r--;
			}

			if (l <= r) {
				Swap (l, r);
				l++;
				r--;
			}
		}

		if (left < r) {
			Quicksort (left, r);
		}
		if (l < right) {
			Quicksort (l, right);
		}
	}
}