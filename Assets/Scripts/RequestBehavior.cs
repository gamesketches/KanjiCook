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
	Text displayText;
	public static float requestTime = 10;
	float requestTimer;

    // Start is called before the first frame update
    void Awake()
    {
        backgroundImage = GetComponent<Image>();
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
	}

	public bool RequestFulfilled(string answer) {
		return answer == displayText.text;
	}
}
