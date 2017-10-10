using System;
using UnityEngine;

// this comes from the Unity Standard Assets
namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        public Transform target;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;
		// private variables
        float m_OffsetZ;
        Vector3 m_LastTargetPosition;
        Vector3 m_CurrentVelocity;
        Vector3 m_LookAheadPos;

        public Vector3 minCameraPos;
        public Vector3 maxCameraPos;
        public bool cameraboundaries;
        private void Start()
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            m_LastTargetPosition = target.position;
            m_OffsetZ = (transform.position - target.position).z;
            transform.parent = null;

			// if target not set, then set it to the player


			if (target==null)
				Debug.LogError("Target not set on Camera2DFollow.");

        }
		private void Update()
        {
			if (target == null)
				return;

            // only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (target.position - m_LastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                m_LookAheadPos = lookAheadFactor*Vector3.right*Mathf.Sign(xMoveDelta);
            }
            else
            {
				m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime*lookAheadReturnSpeed);
            }

            Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward*m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

            transform.position = newPos;

            m_LastTargetPosition = target.position;

            if (cameraboundaries)
            {
                transform.position = new Vector3(Mathf.Clamp(transform.position.x, minCameraPos.x, maxCameraPos.x),
                    Mathf.Clamp(transform.position.y, minCameraPos.y, maxCameraPos.y),
                    Mathf.Clamp(transform.position.z, minCameraPos.z, maxCameraPos.z));
            }
        }
    }
}
