using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Room : MonoBehaviour
{
	public float RoomEdgeLength = 1.0f;

	public PortalTile CreatedByPortal = null;

	public bool IsActiveRoom { get { return (this == GameplayManager.Instance.ActiveRoom); } }
}
