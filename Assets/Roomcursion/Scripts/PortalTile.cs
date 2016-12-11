using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PortalTile : MonoBehaviour
{
	public PortalTile DestinationPortalTile = null;

	public Room ParentRoom { get; private set; }

	public string TileName { get; private set; }

	public bool DebugShouldOpen = false;

	public void Awake()
	{
		ParentRoom = GetComponentInParent<Room>();

		TileName = (transform.parent.parent.name + "_" + transform.parent.name);
	}

	public void Start()
	{
		if (ParentRoom.IsActiveRoom && DebugShouldOpen)
		{
			StartPortalOpening();
		}
	}

	public void OnRoomChangeTriggered()
	{
		GameplayManager.Instance.ChangeRoomThroughPortal(this);
	}

	private void StartPortalOpening()
	{
		GameplayManager.Instance.CreateRoomForPortal(this);
	}

	private void CompletePortalOpening()
	{
	}
}
