using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerGravity : MonoBehaviour
{
	public float LocalGroundY = 0.0f;
	public float GravityScalar = 1.0f;

	public void Update()
	{
		if (transform.localPosition.y > LocalGroundY)
		{
			localVelocity += (Physics.gravity.y * Time.deltaTime * GravityScalar);
			
			Vector3 localPosition = transform.localPosition;
			localPosition.y += (localVelocity * Time.deltaTime);
			transform.localPosition = localPosition;
		}

		if (transform.position.y <= LocalGroundY)
		{
			localVelocity = 0.0f;

			Vector3 localPosition = transform.localPosition;
			localPosition.y = LocalGroundY;
			transform.localPosition = localPosition;
		}
	}

	private float localVelocity = 0.0f;
}
