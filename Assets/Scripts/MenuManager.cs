﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

	public GameObject levelSelect;
	public GameObject titleScreen;
	public GameObject aboutScreen;
	public GameObject packStore;
	public GameObject purchaseButton;
	public GameObject aboutButton;
	public GameObject backButton;
	bool aboutOpen = false;
	bool packsOpen = false;
	bool packsOnTop = false;
	public static float menuSlideSpeed = 0.4f;

    // Start is called before the first frame update
    void Awake()
    {
        titleScreen.SetActive(true);
		levelSelect.SetActive(true);
		DisableAboutScreen();
		DisablePackScreen();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OpenMainMenu() {
		DismissTitleScreen();
	}

	public void OpenLevelSelect() {
		if(!LevelSelect.levelSelectLocked) {
			StartCoroutine(levelSelect.GetComponent<LevelSelect>().FinishOpeningMenu());
		}
	}
	
	public void ToggleAboutScreen() {
		RectTransform aboutRect = aboutScreen.GetComponent<RectTransform>();
		Canvas aboutCanvas = aboutScreen.transform.parent.GetComponent<Canvas>();
		Canvas packCanvas = packStore.transform.parent.GetComponent<Canvas>();
		float rectSize = aboutRect.rect.size.y;
		if(!aboutOpen || 
				(packsOpen && aboutOpen && packsOnTop)) { 
			aboutCanvas.enabled = true;
			packCanvas.sortingOrder = 4;
			aboutCanvas.sortingOrder = 5;
			StartCoroutine(MenuManager.LerpInsetAnimation(aboutRect, -rectSize, 0, menuSlideSpeed, RectTransform.Edge.Bottom));
			aboutOpen = true;
			packsOnTop = false;
		} else {
			StartCoroutine(MenuManager.LerpInsetAnimation(aboutRect, 0, -rectSize, menuSlideSpeed, RectTransform.Edge.Bottom));
			aboutOpen = false;
			Invoke("DisableAboutScreen", menuSlideSpeed);
			if(packsOpen) packsOnTop = true;
		}
	}

	public void TogglePackStore() {
		PurchaseScreenController packStoreController = packStore.GetComponent<PurchaseScreenController>();
		Canvas aboutCanvas = aboutScreen.transform.parent.GetComponent<Canvas>();
		Canvas packCanvas = packStore.transform.parent.GetComponent<Canvas>();
		if(!packsOpen || 
				(aboutOpen && packsOpen && !packsOnTop)) {
			packCanvas.enabled = true;
			aboutCanvas.sortingOrder = 4;
			packCanvas.sortingOrder = 5;
			packStore.GetComponent<PurchaseScreenController>().OpenPurchaseMenu();
			packsOpen = true;
			packsOnTop = true;
		} else {
			packStore.GetComponent<PurchaseScreenController>().ClosePurchaseMenu();
			packsOpen = false;
			packsOnTop = false;
			Invoke("DisablePackScreen", menuSlideSpeed);
		}
	}

	void DismissTitleScreen() {
		titleScreen.transform.parent.GetComponent<Canvas>().enabled = false;
		//titleScreen.SetActive(false);
	}

	void DisableAboutScreen() {
		aboutScreen.transform.parent.GetComponent<Canvas>().enabled = false;
	}

	void DisablePackScreen() {
		packStore.transform.parent.GetComponent<Canvas>().enabled = false;
	}

	public void SlideOffMenus() {
		DismissTitleScreen();
		purchaseButton.SetActive(false);
		aboutButton.SetActive(false);
		backButton.SetActive(false);
		RectTransform levelSelectRect = levelSelect.GetComponent<RectTransform>();
		float levelSelectSize = levelSelectRect.rect.size.x;
		StartCoroutine(MenuManager.LerpInsetAnimation(levelSelectRect, 0, -levelSelectSize, menuSlideSpeed, RectTransform.Edge.Left));
	}

	public void SlideOnMenus() {
		purchaseButton.SetActive(true);
		aboutButton.SetActive(true);
		backButton.SetActive(true);
		RectTransform levelSelectRect = levelSelect.GetComponent<RectTransform>();
		float levelSelectSize = levelSelectRect.rect.size.x;
		StartCoroutine(MenuManager.LerpInsetAnimation(levelSelectRect, -levelSelectSize, 0, menuSlideSpeed, RectTransform.Edge.Left));
	}

	public void BackButtonPressed() {
		if(packsOnTop) {
			PurchaseScreenController packsController = packStore.GetComponent<PurchaseScreenController>();
			packsController.GoBack();
			if(!packsController.open) {
				packsOnTop = false;
				packsOpen = false;
			}
		} else if(aboutOpen) {
			ToggleAboutScreen();
		} else if(LevelSelect.levelSelectLocked) {
			titleScreen.transform.parent.GetComponent<Canvas>().enabled = true;
			StartCoroutine(levelSelect.GetComponent<LevelSelect>().CloseMenu());
		}
	}

	public static IEnumerator LerpInsetAnimation(RectTransform theRect, float startOffset, float targetOffset, float time, RectTransform.Edge parentEdge = RectTransform.Edge.Right) {
		Debug.Log(parentEdge);
		float rectSize = theRect.rect.size.x;
		if(parentEdge == RectTransform.Edge.Top || parentEdge == RectTransform.Edge.Bottom)
			rectSize = theRect.rect.size.y;
		for(float t = 0; t < time; t += Time.deltaTime) {
			float newInset = Mathf.SmoothStep(startOffset, targetOffset, t / time);
			theRect.SetInsetAndSizeFromParentEdge(parentEdge, newInset, rectSize);
			yield return null;
		}
		theRect.SetInsetAndSizeFromParentEdge(parentEdge, targetOffset, rectSize);
	}
}
