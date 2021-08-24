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
	RectTransform rectTransform;
	
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
		rectTransform = GetComponent<RectTransform>();
		float rectWidth = rectTransform.rect.size.x;
		rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, -rectWidth, rectWidth);
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
		foreach(BundleListingController bundle in scrollView.GetComponentsInChildren<BundleListingController>()) {
			if(bundle.packName == packId) {
				AddBundleContents();
			} else bundle.gameObject.SetActive(false);
		}	
	}

	void AddBundleContents() {
		for(int i = 1; i < 10; i++) {
			GameObject levelButton = Instantiate<GameObject>(levelPrefab, scrollView);
			levelButton.GetComponentInChildren<LevelSelectButton>().Initialize(i, false);
		}
	}

	public IEnumerator OpenPurchaseMenu() {
		float rectSize = rectTransform.rect.size.x;
		for(float t = 0; t < 1; t += Time.deltaTime) {
			float newInset = Mathf.SmoothStep(-rectSize, 0, t);
			rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, newInset, rectSize);
			yield return null;
		}
		rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, rectSize);
	}	

	public IEnumerator ClosePurchaseMenu() {
		float rectSize = rectTransform.rect.size.x;
		for(float t = 0; t < 1; t += Time.deltaTime) {
			float newInset = Mathf.SmoothStep(0, -rectSize, t);
			rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, newInset, rectSize);
			yield return null;
		}
		rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, -rectSize, rectSize);
	}

}
