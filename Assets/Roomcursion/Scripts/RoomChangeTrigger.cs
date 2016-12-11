using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomChangeTrigger : MonoBehaviour
{
	public void Awake()
	{
		parentPortal = gameObject.GetComponentInParent<PortalTile>();
	}

	public void OnTriggerEnter(
		Collider otherCollider)
	{
		if (otherCollider.CompareTag("Player"))
		{
			parentPortal.OnRoomChangeTriggered();
		}
	}

	private PortalTile parentPortal = null;
}
