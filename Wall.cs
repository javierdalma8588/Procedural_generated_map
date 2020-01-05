using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Wall : MonoBehaviour
{

	private MeshRenderer[] MR;

	private float fadeDuration = .5f;
	public CameraScript cs;

	public bool isTransparent;


	void Start ()
	{
		cs = Camera.main.GetComponent<CameraScript> ();
		MR = GetComponentsInChildren<MeshRenderer> ();
	}

	void Update ()
	{
		if (!isTransparent)
			return;

		foreach (RaycastHit h in cs.hits) {
			if (h.collider.GetComponent<Wall> () == this)
				return;
		}

		OnCamRayExit ();
	}

	public void OnCamRayEnter ()
	{
		foreach (MeshRenderer mesh in MR) {
			mesh.material.DOFade (0.4f, fadeDuration);
		}
		isTransparent = true;
	}

	public void OnCamRayExit ()
	{
		foreach (MeshRenderer mesh in MR) {
			mesh.material.DOFade (1, fadeDuration);
		}
		isTransparent = false;
	}
}