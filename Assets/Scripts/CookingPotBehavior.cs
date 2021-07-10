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
		newChar.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.value * hitRect.rect.width / 2.5f, Random.value * hitRect.rect.height / 2.5f);
		newChar.transform.rotation = Quaternion.Euler(0, 0, Random.value * 360);
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
			transform.position = startPosition + new Vector3(Random.Range(-panAnimationRange, panAnimationRange), Random.Range(-panAnimationRange, panAnimationRange), 0);
			yield return null;
		}
		transform.position = startPosition;
		resultSpot.text = RecipeLookup();
		ChefController.instance.cooking = false;
		ingredients.Clear();
		GameManager.instance.ClearRequest(resultSpot);
		foreach(Transform t in transform) {
			Destroy(t.gameObject);
		}
	}

	string RecipeLookup() {
		foreach(LanguagePair listing in GameManager.targetWords) {
			if(listing.components.Length == ingredients.Count) {
				bool match = true;
				foreach(string component in ingredients) {
					if(ArrayUtility.IndexOf(listing.components, component) == -1) {
						match = false;
					}
				}
				if(match) return listing.target;
			}
		}
		return "駄目";
	}
}
