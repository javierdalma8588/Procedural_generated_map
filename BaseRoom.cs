using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BaseRoom : MonoBehaviour
{
	[Space]
	[Header ("Room Exits")]
	public bool north;
	public bool east;
	public bool south;
	public bool west;

	[Space]
	[Header ("Room Components")]
	public BoneSpawner boneSpawner;
	public RoomLights[] roomLights;
	private List<Light> lightsList = new List<Light> ();
	public Exit[] exits;
	protected GameObject roomImage;
	public LightWalls[] lightWalls;
	public List<MinimapDoor> minimapDoorImages = new List<MinimapDoor> ();

	protected bool isPedroHere;
	public bool isCombatInside;

	void Awake ()
	{
		exits = GetComponentsInChildren<Exit> ();
		boneSpawner = GetComponentInChildren<BoneSpawner> ();
		roomLights = GetComponentsInChildren<RoomLights> ();
		roomImage = transform.FindChild ("MiniMapImage").gameObject;
		roomImage.layer = LayerMask.NameToLayer ("MinimapInvisivble");
		lightWalls = GetComponentsInChildren<LightWalls> ();

		foreach (LightWalls wall in lightWalls) {
			wall.gameObject.SetActive (false);
		}
			
		Light lights;

		foreach (RoomLights light in roomLights) {
			lights = light.GetComponent<Light> ();
			lightsList.Add (lights);
		}

	
		foreach (Light l in lightsList) {
			l.DOIntensity (0, 3);
		}

		StartCoroutine (GetDoors ());
		GetDoors ();
	}

	IEnumerator GetDoors ()
	{
		yield return new WaitForSeconds (2.0f);
		foreach (Exit ex in exits) {
			MinimapDoor image;
			image = ex.GetComponentInChildren<MinimapDoor> ();
			minimapDoorImages.Add (image);
		}
	}

	public int[,] grid {
		get {
			if (_grid == null) {
				_grid = new int[,] {
					{ 0, 			east ? 1 : 0, 				0 },
					{ south ? 1 : 0, 1, 			north ? 1 : 0 },
					{ 0, 			west ? 1 : 0, 				0 }
				};
			}

			return _grid;
		}
	}

	private int[,] _grid;

	internal float rotation;

	public void Rotate ()
	{
		//Could be more optimized but who cares
		int[,] newGrid = new int[3, 3];
		for (int i = 0; i < 3; ++i) {
			for (int j = 0; j < 3; ++j) {
				newGrid [i, j] = grid [2 - j, i];
			}
		}
		_grid = newGrid;
		transform.Rotate (0, 90, 0);
	}

	public virtual void  OnTriggerEnter (Collider coll)
	{
		if (coll.tag == "Player") {
			isPedroHere = true;
			roomImage.layer = LayerMask.NameToLayer ("MinimapVisible");
			foreach (Light lights in lightsList) {
				lights.DOIntensity (1.6f, 3);
			}
			foreach (LightWalls wall in lightWalls) {
				wall.gameObject.SetActive (true);
			}
			if (minimapDoorImages != null) {
				foreach (MinimapDoor door in minimapDoorImages) {
					door.gameObject.layer = LayerMask.NameToLayer ("MinimapVisible");
				}
			}
		}
	}

	void OnTriggerExit (Collider coll)
	{
		if (coll.tag == "Player") {
			foreach (Light lights in lightsList) {
				lights.DOIntensity (0, 3);
			}
		}
	}
}
