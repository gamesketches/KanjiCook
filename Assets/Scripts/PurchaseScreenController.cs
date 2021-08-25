using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseScreenController : MonoBehaviour
{

	public static PurchaseScreenController instance;
	public GameObject listingPrefab;
	public GameObject levelPrefab;
	public Transform scrollView;
	public Transform detailScrollView;
	public RectTransform detailRectTransform;
	RectTransform rectTransform;
	bool showingDetails;
	List<GameObject> levelListings;
	
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
		showingDetails = false;
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
		GameObject packListing = Instantiate<GameObject>(listingPrefab, scrollView);
		packListing.GetComponent<BundleListingController>().Initialize("Summer Salads", "99 Levels of fun", "$0.99");
		packListing = Instantiate<GameObject>(listingPrefab, scrollView);
		packListing.GetComponent<BundleListingController>().Initialize("Fall Soups", "98 Levels of fun", "$0.99");
	}

	public void ShowDetails(string packId) {
		for(int i = 0; i < scrollView.childCount; i++) {
			BundleListingController bundle = scrollView.GetChild(i).gameObject.GetComponent<BundleListingController>();
			if(bundle.packName == packId) {
				FillOutDetailScreen();
				SlideInDetailScreen();
				return;
			} 
		}	
	}

	void FillOutDetailScreen() {
		for(int i = 1; i < 10; i++) {
			GameObject levelButton = Instantiate<GameObject>(levelPrefab, detailScrollView);
			levelButton.GetComponentInChildren<LevelSelectButton>().Initialize(i, false);
		}
		showingDetails = true;
	}

	void SlideInDetailScreen() {
		StartCoroutine(LerpInsetAnimation(detailRectTransform, -detailRectTransform.rect.size.x, 0, 1));
	}
	
	void SlideOutDetailScreen() {
		StartCoroutine(LerpInsetAnimation(detailRectTransform, 0, -detailRectTransform.rect.size.x, 1));
	}

	void ClearDetailScreen() {
		int numChildren = detailScrollView.childCount;
		for(int i = 0; i < numChildren; i++) {
			Destroy(detailScrollView.GetChild(0).gameObject);
		}
		showingDetails = false;
	}

	public void OpenPurchaseMenu() {
		StartCoroutine(LerpInsetAnimation(rectTransform, -rectTransform.rect.size.x, 0, 1));
	}	

	public void ClosePurchaseMenu() {
		StartCoroutine(LerpInsetAnimation(rectTransform, 0, -rectTransform.rect.size.x, 1));
	}

	IEnumerator LerpInsetAnimation(RectTransform theRect, float startOffset, float targetOffset, float time) {
		float rectSize = rectTransform.rect.size.x;
		for(float t = 0; t < time; t += Time.deltaTime) {
			float newInset = Mathf.SmoothStep(startOffset, targetOffset, t);
			rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, newInset, rectSize);
			yield return null;
		}
		rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, targetOffset, rectSize);
	}

	public void GoBack() {
		if(showingDetails) {
			SlideOutDetailScreen();
			Invoke("ClearDetailScreen", 1.1f);
		} else {
			ClosePurchaseMenu();
		}
	}
		
}
