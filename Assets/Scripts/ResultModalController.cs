using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultModalController : MonoBehaviour
{
	public Image[] stars;
	public Text performanceText;
	public Color disabledStar;
	public string colorString;
	string colorTag;
	Color enabledStar;
	public float slideInTime;
	public float initialDelay;
	public float starFillDelay;
	public float stretchSize;
	public float stretchTime;
	public Sprite emptyStar;
	public Sprite filledStar;
	RectTransform rectTransform;
    // Start is called before the first frame update
    void Awake()
    {
    	colorTag = "<color=" + colorString + ">";
		foreach(Image star in stars) star.color = disabledStar;
		rectTransform = GetComponent<RectTransform>();
		float rectHeight = rectTransform.rect.size.y;
		rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, -rectHeight, rectHeight);
		enabledStar = AppManager.instance.secondaryColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void DisplayResults(int score, int attempts, bool allKanji) {
		int numStars = 0;
		float successRate;
		if(attempts == 0) successRate = 0;
		else 
			successRate = ((float)score / (float) attempts) * 100;
		if(successRate > 90f) numStars++;
		if(score > 12) numStars++;
		if(allKanji) numStars++;
		string performanceString = "You cooked " + colorTag + score.ToString() + "</color> dishes!\n" + 
										"You made " + colorTag + attempts.ToString() + "</color> attempts\n" + 
										"and " + colorTag + successRate.ToString("F2") + "%</color> of them were correct!\n" + 
										"You have potential\nKeep up the good work!";

		for(int i = 0; i < stars.Length; i++) {
			stars[i].color = disabledStar;
			stars[i].sprite = emptyStar;
			if(i < numStars) {
				StartCoroutine(FillStarAnimation(stars[i], initialDelay + (starFillDelay * i)));
			}
		}
		performanceText.text = performanceString;
		ProgressTracker.instance.UpdateLevelInfo(GameManager.levelId, numStars);
	}

	IEnumerator FillStarAnimation(Image theStar, float delayOffset=0) {
		yield return new WaitForSeconds(delayOffset);
		theStar.color = enabledStar;
		theStar.sprite = filledStar;
		Vector3 startSize = theStar.transform.localScale;
		Vector3 targetSize = new Vector3(stretchSize,stretchSize,stretchSize);
		for(float t = 0; t < stretchTime * 2; t += Time.deltaTime) {
			theStar.transform.localScale = Vector3.Lerp(startSize, targetSize, Mathf.PingPong(t, stretchTime) / stretchTime);
			yield return null;
		}
		theStar.transform.localScale = startSize;
	}

	public void CloseResultModal() {
		gameObject.SetActive(false);
	}

	public void Retry() {
		GameManager.instance.RestartLevel();
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
