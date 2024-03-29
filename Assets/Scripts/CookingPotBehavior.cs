﻿using System.Collections;
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
	public GameObject tutorialText;
	Image curImage;
	public Sprite normalSprite;
	public Sprite errorSprite;
	public float cookingTime;
	public Color ingredientColor;
	AudioSource[] audioSources;
	public AudioClip serviceBell;
	public AudioClip[] cookingSounds;
	public ChefController chef;
	bool tutorialTextShown;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
		curImage = GetComponent<Image>();
		ingredients = new List<string>();
		audioSources = GetComponents<AudioSource>();
		//hitRect = GetComponent<RectTransform>();
		GameManager.GameEnded += GameEnded;
		tutorialTextShown = false;
		tutorialText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void AddIngredient(string character) {
		if (resultSpot.text != "") {
			resultSpot.text = "";
			curImage.sprite = normalSprite;
		}
		ingredients.Add(character);
		if(!tutorialTextShown && ingredients.Count > 1) {
			tutorialText.SetActive(true);
			tutorialTextShown = true;
		}
		GameObject newChar = Instantiate<GameObject>(characterPrefab, hitRect.transform);
		newChar.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(-0.3f, 0.3f) * hitRect.rect.width,
															 Random.Range(-0.1f, 0.3f) * hitRect.rect.height);
		newChar.transform.rotation = Quaternion.Euler(0, 0, Random.Range(-15, 15));
		Text charText = newChar.GetComponent<Text>();
		charText.text = character;
		charText.color = ingredientColor;
		PlayCookingSound();
		HighlightPot(false);
	}

	public void CombineIngredients() {
		if(GameManager.gameStarted) {
			StartCoroutine(CookKanji());
		}
	}

	IEnumerator CookKanji() {
		resultSpot.color = AppManager.instance.secondaryColor;
		float cookingTimer = 0;
		float panAnimationRange = 3;
		chef.StartCooking();
		tutorialText.SetActive(false);
		Vector3 startPosition = transform.position;
		RectTransform[] ingredientRects = hitRect.transform.GetComponentsInChildren<RectTransform>();
		while(cookingTimer < cookingTime) {
			cookingTimer += Time.deltaTime;
			Vector3 nextPos = startPosition + new Vector3(Random.Range(-panAnimationRange, panAnimationRange), Random.Range(-panAnimationRange, panAnimationRange), 0);
			float lerpTime = (Random.value * 5) * Time.deltaTime;
			if(lerpTime > cookingTime - cookingTimer) lerpTime = cookingTime - cookingTimer;
			Vector3 curPos = transform.position;
			for(int i = 1; i < ingredientRects.Length; i++) {
				float lerpProportion = cookingTimer / cookingTime;
				ingredientRects[i].position = Vector2.Lerp(ingredientRects[i].transform.position, resultSpot.transform.position, lerpProportion);
				lerpProportion /= 2;
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
		chef.StopCooking();
		hitRect.transform.localScale = Vector3.one;
		GameManager.instance.ClearRequest(resultSpot, resultPair);
		ClearIngredients();
		FinishCookingSounds();
		hitRect.transform.rotation = Quaternion.identity;
		if(resultPair.literal == "駄目") {
			curImage.sprite = errorSprite;
			resultSpot.color = Color.black;
			yield return new WaitForSeconds(0.8f);
			if (resultSpot.text == "駄目") {
				resultSpot.text = "";
				curImage.sprite = normalSprite;
			}
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
		tutorialTextShown = false;
	}

	public void HighlightPot(bool highlight) { 
		if(highlight && curImage.color == Color.white) {
			curImage.color = AppManager.instance.secondaryColor;
		} else if(!highlight && curImage.color != Color.white) {
			curImage.color = Color.white;
		}
	}
}
