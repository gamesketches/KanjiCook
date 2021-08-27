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

    // Start is called before the first frame update
    void Awake()
    {
        titleScreen.SetActive(true);
		levelSelect.SetActive(true);
		aboutScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OpenMainMenu() {
		DismissTitleScreen();
	}
	
	public void ToggleAboutScreen() {
		if(!aboutScreen.activeSelf) { 
			aboutScreen.SetActive(true);
		} else {
			aboutScreen.SetActive(false);
		}
	}

	public void TogglePackStore() {
		PurchaseScreenController packStoreController = packStore.GetComponent<PurchaseScreenController>();
		if(!packStoreController.open) {
			packStore.GetComponent<PurchaseScreenController>().OpenPurchaseMenu();
		} else {
			packStore.GetComponent<PurchaseScreenController>().ClosePurchaseMenu();
		}
	}

	void DismissTitleScreen() {
		titleScreen.SetActive(false);
	}

	public void SlideOffMenus() {
		DismissTitleScreen();
		RectTransform levelSelectRect = levelSelect.GetComponent<RectTransform>();
		float levelSelectSize = levelSelectRect.rect.size.x;
		StartCoroutine(MenuManager.LerpInsetAnimation(levelSelectRect, 0, -levelSelectSize, 0.4f, RectTransform.Edge.Left));
	}

	public void SlideOnMenus() {
		RectTransform levelSelectRect = levelSelect.GetComponent<RectTransform>();
		float levelSelectSize = levelSelectRect.rect.size.x;
		StartCoroutine(MenuManager.LerpInsetAnimation(levelSelectRect, -levelSelectSize, 0, 0.4f, RectTransform.Edge.Left));
	}

	public static IEnumerator LerpInsetAnimation(RectTransform theRect, float startOffset, float targetOffset, float time, RectTransform.Edge parentEdge = RectTransform.Edge.Right) {
		Debug.Log(parentEdge);
		float rectSize = theRect.rect.size.x;
		for(float t = 0; t < time; t += Time.deltaTime) {
			float newInset = Mathf.SmoothStep(startOffset, targetOffset, t / time);
			theRect.SetInsetAndSizeFromParentEdge(parentEdge, newInset, rectSize);
			yield return null;
		}
		theRect.SetInsetAndSizeFromParentEdge(parentEdge, targetOffset, rectSize);
	}
}
