using UnityEngine;
using System.Collections;

public class PlatformMover : MonoBehaviour {

	public GameObject platform; // reference to the platform to move

	public GameObject[] myWaypoints; // this is an array of all the waypoints. I can set the amount of waypoints I want

	public float moveSpeed = 5f; // platform move speed
	public float waitAtWaypointTime = 1f; // time the platform will wait at the waypoint before moving to the next one

	public bool loop = true; // check if platform is looping or not

	Transform _transform;
	int myWaypointIndex = 0; // used as index for My_Waypoints
	float MoveTime; //variable that determines the length of time it takes to move
	bool isMoving = true; //boolean to check is the platform is moving or not

	void Start () { //The start function will determine what the platform it will target and the values of MoveTime and isMoving 
		_transform = platform.transform;
		MoveTime = 0f;
		isMoving = true;
	}
	
	// game loop
	void Update () {
		// if the game time is greater than MoveTime, then start moving
		if (Time.time >= MoveTime) {
			PlatformMovement();//Call platform movement function
		}
	}

	void PlatformMovement() {
		// if there are waypoints and the platform can move...
		if ((myWaypoints.Length != 0) && (isMoving)) {

			// ...move towards first waypoint chosen.
			_transform.position = Vector3.MoveTowards(_transform.position, myWaypoints[myWaypointIndex].transform.position, moveSpeed * Time.deltaTime);

			// if the platform is close enough to waypoint, make it's new target the next waypoint
			if(Vector3.Distance(myWaypoints[myWaypointIndex].transform.position, _transform.position) <= 0) {
				myWaypointIndex++;
				MoveTime = Time.time + waitAtWaypointTime;
			}
			
			// reset waypoint back to 0 for looping, otherwise flag not moving for not looping
			if(myWaypointIndex >= myWaypoints.Length) {
				if (loop)
					myWaypointIndex = 0;
				else
					isMoving = false;
			}
		}
	}
}
