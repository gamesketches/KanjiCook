using System.Collections;
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
	bool aboutOpen = false;
	bool packsOpen = false;

    // Start is called before the first frame update
    void Awake()
    {
        titleScreen.SetActive(true);
		levelSelect.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OpenMainMenu() {
		DismissTitleScreen();
	}
	
	public void ToggleAboutScreen() {
		RectTransform aboutRect = aboutScreen.GetComponent<RectTransform>();
		Canvas aboutCanvas = aboutScreen.transform.parent.GetComponent<Canvas>();
		Canvas packCanvas = packStore.transform.parent.GetComponent<Canvas>();
		float rectSize = aboutRect.rect.size.y;
		if(!aboutOpen || 
				(packsOpen && aboutOpen && aboutCanvas.sortingOrder < packCanvas.sortingOrder)) { 
			packCanvas.sortingOrder = 2;
			aboutCanvas.sortingOrder = 3;
			StartCoroutine(MenuManager.LerpInsetAnimation(aboutRect, -rectSize, 0, 0.4f, RectTransform.Edge.Bottom));
			aboutOpen = true;
		} else {
			StartCoroutine(MenuManager.LerpInsetAnimation(aboutRect, 0, -rectSize, 0.4f, RectTransform.Edge.Bottom));
			aboutOpen = false;
		}
	}

	public void TogglePackStore() {
		PurchaseScreenController packStoreController = packStore.GetComponent<PurchaseScreenController>();
		Canvas aboutCanvas = aboutScreen.transform.parent.GetComponent<Canvas>();
		Canvas packCanvas = packStore.transform.parent.GetComponent<Canvas>();
		if(!packsOpen || 
				(aboutOpen && packsOpen && packCanvas.sortingOrder < aboutCanvas.sortingOrder)) {
			aboutCanvas.sortingOrder = 2;
			packCanvas.sortingOrder = 3;
			packStore.GetComponent<PurchaseScreenController>().OpenPurchaseMenu();
			packsOpen = true;
		} else {
			packStore.GetComponent<PurchaseScreenController>().ClosePurchaseMenu();
			packsOpen = false;
		}
	}

	void DismissTitleScreen() {
		titleScreen.SetActive(false);
	}

	public void SlideOffMenus() {
		DismissTitleScreen();
		purchaseButton.SetActive(false);
		aboutButton.SetActive(false);
		RectTransform levelSelectRect = levelSelect.GetComponent<RectTransform>();
		float levelSelectSize = levelSelectRect.rect.size.x;
		StartCoroutine(MenuManager.LerpInsetAnimation(levelSelectRect, 0, -levelSelectSize, 0.4f, RectTransform.Edge.Left));
	}

	public void SlideOnMenus() {
		purchaseButton.SetActive(true);
		aboutButton.SetActive(true);
		RectTransform levelSelectRect = levelSelect.GetComponent<RectTransform>();
		float levelSelectSize = levelSelectRect.rect.size.x;
		StartCoroutine(MenuManager.LerpInsetAnimation(levelSelectRect, -levelSelectSize, 0, 0.4f, RectTransform.Edge.Left));
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
