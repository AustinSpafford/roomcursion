using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameplayManager : MonoBehaviour
{
	public static GameplayManager Instance
	{
		get
		{
			if (instance == null)
			{
				var instances = Component.FindObjectsOfType<GameplayManager>();

				if (instances.Count() == 0)
				{
					throw new System.InvalidOperationException("No GameplayManagers could be found.");
				}
				
				if (instances.Count() > 1)
				{
					throw new System.InvalidOperationException("There's more than one GameplayManager!");
				}

				instance = instances.Single();
			}

			return instance;
		}
	}

	public Room ActiveRoom { get; private set; }

	public PortalTile ActiveOriginPortal { get; private set; }
	public PortalTile ActiveDestinationPortal { get; private set; }

	public void Awake()
	{
		activeRoomLayerIndex = LayerMask.NameToLayer("Active Room");
		nextRoomLayerIndex = LayerMask.NameToLayer("Next Room");
		previousRoomLayerIndex = LayerMask.NameToLayer("Previous Room");

		player = Component.FindObjectsOfType<PlayerGravity>().Single();

		ActiveRoom = Component.FindObjectsOfType<Room>().Single();
		allRooms.Add(ActiveRoom);

		ApplyRoomRestrictedLighting(ActiveRoom.transform, activeRoomLayerIndex);
	}

	public void CreateRoomForPortal(
		PortalTile originPortal)
	{
		GameObject newRoomObject = Object.Instantiate<GameObject>(ActiveRoom.gameObject);

		Room newRoom = newRoomObject.GetComponent<Room>();

		allRooms.Add(newRoom);		
		
		ApplyRoomRestrictedLighting(newRoom.transform, nextRoomLayerIndex);

		newRoom.CreatedByPortal = originPortal;
		
		string destinationPortalTileName = originPortal.DestinationPortalTile.TileName;
		PortalTile destinationPortal = 
			newRoom.GetComponentsInChildren<PortalTile>()
				.Where(elem => (elem.TileName == destinationPortalTileName))
				.Single();

		// Transform the new room so the portals align.
		{
			// Work in terms of room-coordinates.				
			newRoom.transform.position = Vector3.zero;

			Quaternion desiredNewRoomRotation = (
				Quaternion.AngleAxis(180.0f, Vector3.right) *
				ActiveRoom.transform.rotation *
				Quaternion.Inverse(destinationPortal.transform.rotation));			

			// Perform the initial portal-alignment rotation.
			newRoom.transform.rotation = (newRoom.transform.rotation * desiredNewRoomRotation);

			// Rotate the new room so the room-centers are vertically aligned.
			{
				Vector3 projectedOriginPortalToRoomCenter = 
					Vector3.ProjectOnPlane(
						(originPortal.transform.position - originPortal.ParentRoom.transform.position),
						Vector3.up);

				Vector3 projectedDestinationPortalToRoomCenter = 
					Vector3.ProjectOnPlane(
						(destinationPortal.transform.position - destinationPortal.ParentRoom.transform.position),
						Vector3.up);

				float correctionAngle = 
					Vector3.Angle(
						projectedDestinationPortalToRoomCenter,
						projectedOriginPortalToRoomCenter);

				Quaternion roomCenterAlignmentRotation =
					Quaternion.AngleAxis(correctionAngle, Vector3.up);
					
				newRoom.transform.rotation = (roomCenterAlignmentRotation * newRoom.transform.rotation);
			}

			// Positionally align the portals
			newRoom.transform.position += 
				(originPortal.transform.position - destinationPortal.transform.position);
		}
	}

	public void ChangeRoomThroughPortal(
		PortalTile originPortal)
	{
		Room destinationRoom = allRooms.Where(elem => (elem.CreatedByPortal == originPortal)).Single();
		
		ApplyRoomRestrictedLighting(ActiveRoom.transform, previousRoomLayerIndex);

		ActiveRoom = destinationRoom;
		
		ApplyRoomRestrictedLighting(ActiveRoom.transform, activeRoomLayerIndex);

		// Forget the creater, since it's no longer needed, and it'll be destroyed once the portal closes.
		ActiveRoom.CreatedByPortal = null; 

		foreach (Room room in allRooms)
		{
			room.transform.position += (room.RoomEdgeLength * Vector3.up);
		}

		player.transform.position += (ActiveRoom.RoomEdgeLength * Vector3.up);
	}

	private static GameplayManager instance = null;

	private int activeRoomLayerIndex = -1;
	private int nextRoomLayerIndex = -1;
	private int previousRoomLayerIndex = -1;

	private List<Room> allRooms = new List<Room>();

	private PlayerGravity player = null;

	private static void ApplyRoomRestrictedLighting(
		Transform root,
		int layerIndex)
	{
		SetLayerRecursive(root, layerIndex);

		foreach (Light light in root.GetComponentsInChildren<Light>())
		{
			light.cullingMask = (1 << layerIndex);
		}
	}

	private static void SetLayerRecursive(
		Transform root,
		int layerIndex)
	{
		root.gameObject.layer = layerIndex;

		int childCount = root.transform.childCount;

		for (int childIndex = 0;
			childIndex < childCount;
			++childIndex)
		{
			SetLayerRecursive(root.transform.GetChild(childIndex), layerIndex);
		}
	}
}
