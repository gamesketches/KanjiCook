using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestBehavior : MonoBehaviour
{
	public Color startColor;
	public Color warningColor;
	public Color direColor;
	Image backgroundImage;
	GameObject fulfillStamp;
	Text displayText;
	public static float requestTime = 10;
	static float stampSize = 1.5f;
	static float stampAnimationSize = 1.9f;
	static float fulfillAnimationTime = 0.1f;
	float requestTimer;

    // Start is called before the first frame update
    void Awake()
    {
        backgroundImage = GetComponent<Image>();
		fulfillStamp = transform.GetChild(1).gameObject;
		fulfillStamp.SetActive(false);
		displayText = GetComponentInChildren<Text>();
		requestTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        requestTimer += Time.deltaTime;
		if(requestTimer < requestTime * 0.33f) {
			backgroundImage.color = startColor;
		} else if(requestTimer < requestTime * 0.66f) {
			backgroundImage.color = warningColor;
		} else {
			backgroundImage.color = direColor;
		}
    }

	public void Initialize(string target) {
		displayText.text = target;
		backgroundImage.color = startColor;
		requestTimer = 0;
	}

	public bool RequestFulfilled(string answer) {
		return answer == displayText.text;
	}

	public void PlayFulfilledAnimation() {
		StartCoroutine(FulfillAnimation());
	}

	IEnumerator FulfillAnimation() {
		fulfillStamp.SetActive(true);
		Vector3 startSize = new Vector3(stampAnimationSize, stampAnimationSize, stampAnimationSize);
		Vector3 targetSize = new Vector3(stampSize, stampSize, stampSize);
		for(float t = 0; t < fulfillAnimationTime; t += Time.deltaTime) {
			fulfillStamp.transform.localScale = Vector3.Lerp(startSize, targetSize, t / fulfillAnimationTime);
			yield return null;
		}
		yield return new WaitForSeconds(0.8f);
		gameObject.SetActive(false);
	}

	public void ClearText() {
		displayText.text = "";
	}

	void OnEnable() {
		fulfillStamp.SetActive(false);
	}
}
