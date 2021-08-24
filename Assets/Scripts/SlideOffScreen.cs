using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideOffScreen : MonoBehaviour
{
	public RectTransform.Edge slideDirection;
	public float slideSpeedMultiplier = 1;
	RectTransform rectTransform;
	float startInset;
	float rectSize;
	public float endingInset;

    // Start is called before the first frame update
    void Awake()
    {
		rectTransform = GetComponent<RectTransform>();
        CalibrateOffset();
    }

    // Update is called once per frame
    void Update()
    {
		rectTransform.SetInsetAndSizeFromParentEdge(slideDirection, Mathf.SmoothStep(startInset, endingInset, LevelSelect.lerpProportion), rectSize);
    }

	void CalibrateOffset() {
		switch(slideDirection) {
			case RectTransform.Edge.Left:
				startInset = Screen.width * rectTransform.anchorMin.x;
				rectSize = rectTransform.rect.size.x;
				break;
			case RectTransform.Edge.Right:
				startInset = Screen.width - (Screen.width * rectTransform.anchorMax.x);
				rectSize = rectTransform.rect.size.x;
				break;
			case RectTransform.Edge.Top:
				startInset = Screen.height - (Screen.height * rectTransform.anchorMax.y);
				rectSize = rectTransform.rect.size.y;
				break;
			case RectTransform.Edge.Bottom:
				startInset = Screen.height * rectTransform.anchorMin.y;
				rectSize = rectTransform.rect.size.y;
				break;
		}
	}
}
