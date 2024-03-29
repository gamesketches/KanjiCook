﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseScreenController : MonoBehaviour
{

	public static PurchaseScreenController instance;
	public GameObject listingPrefab;
	public GameObject levelPrefab;
	public GameObject dottedLinePrefab;
	public Transform scrollView;
	public Transform detailScrollView;
	public RectTransform detailRectTransform;
	RectTransform rectTransform;
	bool showingDetails;
	bool screenTransitionMutex;
	public bool open;
	List<GameObject> levelListings;
	
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
		showingDetails = false;
		screenTransitionMutex = false;
		open = false;
		rectTransform = GetComponent<RectTransform>();
		float rectWidth = rectTransform.rect.size.x;
		rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, -rectWidth, rectWidth);
		rectWidth = detailRectTransform.rect.size.x;
		detailRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, -rectWidth, rectWidth); 
		FillPurchaseMenu();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void FillPurchaseMenu() {
		CreateNewPurchase("JLPT 5 Pack", "24 Levels with JLPT5 kanji", "$0.99", "com.bluesphere.kanjicook.jlpt5");
		CreateNewPurchase("JLPT 4 Pack", "50 Levels with JLPT4 kanji", "$1.99", "com.bluesphere.kanjicook.jlpt4");
		CreateNewPurchase("JLPT 3 Pack", "144 Levels with JLPT3 kanji", "$4.99", "com.bluesphere.kanjicook.jlpt3");
		CreateNewPurchase("JLPT 2 Pack", "148 Levels with JLPT2 kanji", "$4.99", "com.bluesphere.kanjicook.jlpt2");
	}

	void CreateNewPurchase(string title, string subTitle, string price, string iapCode) {
		GameObject packListing = Instantiate<GameObject>(listingPrefab, scrollView);
		packListing.GetComponent<BundleListingController>().Initialize(title, subTitle, price, iapCode);
		packListing.transform.SetSiblingIndex(packListing.transform.parent.childCount - 3);
		GameObject dottedLine = Instantiate<GameObject>(dottedLinePrefab, scrollView);
		dottedLine.transform.SetSiblingIndex(scrollView.childCount - 3);
	}
		

	public void ShowDetails(string packId) {
		for(int i = 0; i < scrollView.childCount; i++) {
			BundleListingController bundle = scrollView.GetChild(i).gameObject.GetComponent<BundleListingController>();
			if(bundle.packName == packId) {
				FillOutDetailScreen(bundle);
				SlideInDetailScreen();
				return;
			} 
		}	
	}

	void FillOutDetailScreen(BundleListingController bundle) {
		BundleListingController pageBundle = detailScrollView.GetChild(0).GetComponent<BundleListingController>();
		pageBundle.Initialize(bundle.packName, bundle.packDescription, bundle.packPrice, bundle.packId);
		for(int i = 1; i < 10; i++) {
			GameObject levelButton = Instantiate<GameObject>(levelPrefab, detailScrollView);
			levelButton.GetComponentInChildren<LevelSelectButton>().Initialize(i, false);
		}
		showingDetails = true;
	}

	void SlideInDetailScreen() {
		StartCoroutine(LerpInsetAnimation(detailRectTransform, -detailRectTransform.rect.size.x, 0, 0.4f));
		MenuManager.instance.PlayPageTurnSound();
	}
	
	void SlideOutDetailScreen() {
		StartCoroutine(LerpInsetAnimation(detailRectTransform, 0, -detailRectTransform.rect.size.x, 0.4f));
		MenuManager.instance.PlayPageTurnSound();
	}

	void ClearDetailScreen() {
		int numChildren = detailScrollView.childCount - 1;
		for(int i = 0; i < numChildren; i++) {
			Destroy(detailScrollView.GetChild(1).gameObject);
		}
		showingDetails = false;
	}

	public void OpenPurchaseMenu() {
		StartCoroutine(LerpInsetAnimation(rectTransform, -rectTransform.rect.size.x, 0, 0.4f));
		open = true;
	}	

	public void ClosePurchaseMenu() {
		if(showingDetails) {
			StartCoroutine(LerpInsetAnimation(detailRectTransform, 0, -detailRectTransform.rect.size.x, 0.6f, false));
			showingDetails = false;
		}
		open = false;
		StartCoroutine(LerpInsetAnimation(rectTransform, 0, -rectTransform.rect.size.x, 0.4f));
	}

	IEnumerator LerpInsetAnimation(RectTransform theRect, float startOffset, float targetOffset, float time, bool useMutex = true) {
		if(useMutex) {
			if(screenTransitionMutex) {
				while(screenTransitionMutex) yield return null;
			}
			screenTransitionMutex = true;
		}
		float rectSize = rectTransform.rect.size.x;
		for(float t = 0; t < time; t += Time.deltaTime) {
			float newInset = Mathf.SmoothStep(startOffset, targetOffset, t / time);
			theRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, newInset, rectSize);
			yield return null;
		}
		theRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, targetOffset, rectSize);
		if(useMutex) screenTransitionMutex = false;
	}

	public void GoBack() {
		if(showingDetails) {
			SlideOutDetailScreen();
			Invoke("ClearDetailScreen", 0.5f);
		} else if(open){
			ClosePurchaseMenu();
		}
	}
		
}
