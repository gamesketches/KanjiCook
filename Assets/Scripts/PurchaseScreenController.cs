using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PurchaseScreenController : MonoBehaviour
{

	public static PurchaseScreenController instance;
	public GameObject listingPrefab;
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
		GameObject packListing = Instantiate<GameObject>(listingPrefab);
		packListing.transform.parent = scrollView;
		Text[] textFields = packListing.GetComponentsInChildren<Text>();
		textFields[1].text = "Summer Salads";
		textFields[2].text = "99 levels of fun";
		textFields[4].text = "$0.99";
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
