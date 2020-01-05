using UnityEngine;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class NormalRoom : BaseRoom
{
	public EnemyEncounter chosenEncounter;
	public EnemyEncounter[] enemyEncounter;

	protected bool hasPlayedSound;

	void Start ()
	{
		StartCoroutine (PickEnemyEncounter ());
	}

	void Update ()
	{
		if (chosenEncounter != null) {
			if (chosenEncounter.alebrijes.Count.Equals (0)) {
				isCombatInside = false;

				if (!hasPlayedSound) {
					hasPlayedSound = true;
					AkSoundEngine.PostEvent ("UI_Level_Complete", this.gameObject);
				}

				foreach (LightWalls wall in lightWalls) {
					wall.gameObject.SetActive (false);
				}
					
			} else {
				isCombatInside = true;
				if (chosenEncounter.alebrijes.Count.Equals (2)) {
					foreach (AlebrijeTest alebrije in chosenEncounter.alebrijes) {
						alebrije.minimapAlebrijeImage.gameObject.layer = LayerMask.NameToLayer ("MinimapVisible");
					}
				}                
			}			
		}
	}

	IEnumerator PickEnemyEncounter ()
	{
		yield return new WaitForSeconds (5.0f);
		int r = Random.Range (0, enemyEncounter.Length);
		EnemyEncounter encounterChosen = enemyEncounter [r];
		chosenEncounter = (EnemyEncounter)Instantiate (encounterChosen, this.gameObject.transform.position, this.gameObject.transform.rotation);
		chosenEncounter.transform.SetParent (gameObject.transform);
	}
}