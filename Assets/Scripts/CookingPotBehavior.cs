using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEditor;

public class CookingPotBehavior : MonoBehaviour
{
	public static CookingPotBehavior instance;
	public RectTransform hitRect;
	public GameObject characterPrefab;
	List<string> ingredients;
	public Text resultSpot;
	Image curImage;
	public float cookingTime;
	bool cooking;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
		curImage = GetComponent<Image>();
		ingredients = new List<string>();
		hitRect = GetComponent<RectTransform>();
		cooking = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void AddIngredient(string character) {
		ingredients.Add(character);
		GameObject newChar = Instantiate<GameObject>(characterPrefab, transform);
		newChar.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-0.1f, 0.1f) * hitRect.rect.width,
															 Random.Range(0.2f, 0.4f) * hitRect.rect.height);
		newChar.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-30, 30));
		Text charText = newChar.GetComponent<Text>();
		charText.text = character;
		charText.color = Color.white;
	}

	public void CombineIngredients() {
		StartCoroutine(CookKanji());
	}

	IEnumerator CookKanji() {
		float cookingTimer = 0;
		float panAnimationRange = 3;
		ChefController.instance.cooking = true;
		Vector3 startPosition = transform.position;
		while(cookingTimer < cookingTime) {
			cookingTimer += Time.deltaTime;
			Vector3 nextPos = startPosition + new Vector3(Random.Range(-panAnimationRange, panAnimationRange), Random.Range(-panAnimationRange, panAnimationRange), 0);
			float lerpTime = (Random.value * 5) * Time.deltaTime;
			if(lerpTime > cookingTime - cookingTimer) lerpTime = cookingTime - cookingTimer;
			Vector3 curPos = transform.position;
			for(float t = 0; t < lerpTime; t += Time.deltaTime) {
				transform.position = Vector3.Lerp(curPos, nextPos, t / lerpTime);
				cookingTimer += Time.deltaTime;
				yield return null;
			}
			yield return null;
		}
		transform.position = startPosition;
		EntreeData resultPair = GameManager.instance.RecipeLookup(ingredients.ToArray());
		resultSpot.text = resultPair.literal;
		ChefController.instance.cooking = false;
		GameManager.instance.ClearRequest(resultSpot, resultPair);
		ClearIngredients();
	}

	string RecipeLookup() {
		foreach(EntreeData listing in GameManager.targetWords) {
			if(listing.components.Length == ingredients.Count) {
				bool match = true;
				foreach(string component in ingredients) {
					if(ArrayUtility.IndexOf(listing.components, component) == -1) {
						match = false;
					}
				}
				if(match) {
					return listing.meanings[0];
				}
			}
		}
		return "駄目";
	}

	public void ClearIngredients() {
		ingredients.Clear();
		foreach(Transform t in transform) {
			Destroy(t.gameObject);
		}
	}
}
