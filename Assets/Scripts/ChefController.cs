using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefController : MonoBehaviour
{
	public Transform chefArm;
	public Transform chefForearm;
	public bool cooking;
	float forearmRotation;
	public float rotationInterval;
	public float rotationSpeed;
	public float translationInterval;
	public float translationSpeed;
    // Start is called before the first frame update
    void Start()
    {
		GameManager.GameEnded += GameEnded;
    }

	public void StartCooking() {
		cooking = true;
		StartCoroutine(CookingAnimation());
	}
	
	public void StopCooking() {
		cooking = false;
	}

	IEnumerator CookingAnimation() {
		float cookingTime = 0;
		while (cooking && GameManager.gameStarted)
		{
			float rotationAmount = Mathf.PingPong(cookingTime * rotationSpeed, rotationInterval);
			chefArm.transform.rotation = Quaternion.Euler(0, 0, rotationAmount);
			cookingTime += Time.deltaTime;
			yield return null;
		}
		
		Quaternion curRotation = chefArm.transform.rotation;
		for(float t = 0; t < 0.4f; t += Time.deltaTime) {
			chefArm.transform.rotation = Quaternion.Lerp(curRotation, Quaternion.identity, t / 0.4f);
			yield return null;
		}
		chefArm.transform.rotation = Quaternion.identity;
    }

	/*IEnumerator CookingAnimation() {
		float cookingTime = 0;
		Vector3 startPos = chefArm.transform.position;
		while(cooking && GameManager.gameStarted) {
			float rotationAmount = Mathf.PingPong(cookingTime * rotationSpeed, rotationInterval);
			chefArm.transform.rotation = Quaternion.Euler(0, 0, rotationAmount); 
			chefArm.transform.position = startPos + new Vector3( Mathf.PingPong(cookingTime * translationSpeed, translationInterval), 0, 0);
			chefForearm.transform.rotation = Quaternion.Euler(0, 0, rotationAmount/2);
			cookingTime += Time.deltaTime;
			yield return null;
		}
		Quaternion curRotation = chefArm.transform.rotation;
		for(float t = 0; t < 0.4f; t += Time.deltaTime) {
			chefArm.transform.rotation = Quaternion.Lerp(curRotation, Quaternion.identity, t / 0.4f);
			yield return null;
		}
		chefArm.transform.rotation = Quaternion.identity;
		chefForearm.transform.rotation = Quaternion.identity;
		chefArm.transform.position = startPos;
	}
	*/

	void GameEnded() {
		cooking = false;
	}
}
