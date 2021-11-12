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
	public Color ingredientColor;
	AudioSource[] audioSources;
	public AudioClip serviceBell;
	public AudioClip[] cookingSounds;
	public ChefController chef;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
		curImage = GetComponent<Image>();
		ingredients = new List<string>();
		audioSources = GetComponents<AudioSource>();
		//hitRect = GetComponent<RectTransform>();
		GameManager.GameEnded += GameEnded;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void AddIngredient(string character) {
		if(resultSpot.text != "")
			resultSpot.text = "";
		ingredients.Add(character);
		GameObject newChar = Instantiate<GameObject>(characterPrefab, hitRect.transform);
		newChar.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-0.3f, 0.3f) * hitRect.rect.width,
															 Random.Range(-0.3f, 0.3f) * hitRect.rect.height);
		newChar.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-15, 15));
		Text charText = newChar.GetComponent<Text>();
		charText.text = character;
		charText.color = ingredientColor;
		PlayCookingSound();
	}

	public void CombineIngredients() {
		if(GameManager.gameStarted) {
			StartCoroutine(CookKanji());
		}
	}

	IEnumerator CookKanji() {
		float cookingTimer = 0;
		float panAnimationRange = 3;
		chef.cooking = true;
		Vector3 startPosition = transform.position;
		RectTransform[] ingredientRects = hitRect.transform.GetComponentsInChildren<RectTransform>();
		while(cookingTimer < cookingTime) {
			cookingTimer += Time.deltaTime;
			Vector3 nextPos = startPosition + new Vector3(Random.Range(-panAnimationRange, panAnimationRange), Random.Range(-panAnimationRange, panAnimationRange), 0);
			float lerpTime = (Random.value * 5) * Time.deltaTime;
			if(lerpTime > cookingTime - cookingTimer) lerpTime = cookingTime - cookingTimer;
			Vector3 curPos = transform.position;
			//hitRect.transform.Rotate(0, 0, 4);
			for(int i = 0; i < ingredientRects.Length; i++) {
				float lerpProportion = cookingTimer / cookingTime;
				ingredientRects[i].anchoredPosition = Vector2.Lerp(ingredientRects[i].anchoredPosition, Vector2.zero, lerpProportion);
				lerpProportion /= 2;
				//ingredientRects[i].transform.localScale = Vector3.one - new Vector3(lerpProportion, lerpProportion, lerpProportion);
			}
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
		chef.cooking = false;
		hitRect.transform.localScale = Vector3.one;
		GameManager.instance.ClearRequest(resultSpot, resultPair);
		ClearIngredients();
		FinishCookingSounds();
		hitRect.transform.rotation = Quaternion.identity;
		if(resultPair.literal == "駄目") {
			yield return new WaitForSeconds(0.8f);
			if(resultSpot.text == "駄目")
				resultSpot.text = "";
		}
	}

	void PlayCookingSound() {
		int diceRoll = Mathf.FloorToInt(Random.value * cookingSounds.Length);
		for(int i = 0; i < audioSources.Length; i++) {
			if(!audioSources[i].isPlaying) {
				audioSources[i].clip = cookingSounds[diceRoll];
				audioSources[i].Play();
				return;
			}
		}
	}

	void FinishCookingSounds() {
		for(int i = 0; i < audioSources.Length; i++) {
			audioSources[i].Stop();
		}
		audioSources[0].PlayOneShot(serviceBell, 0.6f);
	}

	string RecipeLookup() {
		foreach(EntreeData listing in GameManager.targetWords) {
			if(listing.components.Length == ingredients.Count) {
				bool match = true;
				List<string> componentList = new List<string>(listing.components);
				foreach(string component in ingredients) {
					if(componentList.IndexOf(component) == -1) {
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
		foreach(Transform t in hitRect.transform) {
			Destroy(t.gameObject);
		}
		for(int i = 0; i < audioSources.Length; i++) {
			audioSources[i].Stop();
		}
	}

	void GameEnded() {
		ClearIngredients();
		FinishCookingSounds();
	}
}
