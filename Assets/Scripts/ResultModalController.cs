using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultModalController : MonoBehaviour
{
	public Image[] stars;
	public Text performanceText;
	public Color disabledStar;
	public Color enabledStar;
	public float slideInTime;
	RectTransform rectTransform;
    // Start is called before the first frame update
    void Awake()
    {
		foreach(Image star in stars) star.color = disabledStar;
		rectTransform = GetComponent<RectTransform>();
		float rectHeight = rectTransform.rect.size.y;
		rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, -rectHeight, rectHeight);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void DisplayResults(int rating, string performanceString) {
		for(int i = 0; i < stars.Length; i++) {
			stars[i].color = i < rating ? enabledStar : disabledStar;
		}
		performanceText.text = performanceString;
	}

	public void CloseResultModal() {
		gameObject.SetActive(false);
	}

	public void Retry() {
		GameManager.instance.LevelSetup();
		CloseResultModal();
	}

	public void LevelSelect() {
		AppManager.instance.OpenLevelSelect();
		CloseResultModal();
	}

	void OnEnable() {
		foreach(Image star in stars) star.color = disabledStar;
		StartCoroutine(SlideInResultModal());
	}	

	IEnumerator SlideInResultModal() {
		float rectHeight = rectTransform.rect.size.y;
		for(float t = 0; t < slideInTime; t += Time.deltaTime) {
			float insetAmount = Mathf.SmoothStep(-rectHeight, 0, t / slideInTime);
			rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, insetAmount, rectHeight);
			yield return null;
		}
		rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, 0, rectHeight);
	}
		
}
