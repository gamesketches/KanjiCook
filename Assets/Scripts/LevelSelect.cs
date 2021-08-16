using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelSelect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public GameObject levelButtonPrefab;
	Image menuImage;
	RectTransform canvas;
	RectTransform rectTransform;
	float startOffset;
	public float startingRotation;
	public Transform scrollView;
	public int levelsToLoad;
	public static float lerpProportion;
	public static bool levelSelectLocked;
	public AnimationCurve scaleCurve;
	public AnimationCurve openCurve;
    // Start is called before the first frame update
    IEnumerator Start()
    {
		levelSelectLocked = false;
		lerpProportion = 0;
		canvas = transform.parent.GetComponent<RectTransform>();
		rectTransform = GetComponent<RectTransform>();
		menuImage = GetComponent<Image>();
		startOffset = rectTransform.offsetMax.y;
		transform.rotation = Quaternion.Euler(0, 0, startingRotation);
		while( !ContentManager.instance.LevelSelectContentReady()) yield return null;
		int levelCount = 1;
		for(levelCount = 1; levelCount < levelsToLoad; levelCount++) {
			GameObject levelButton = Instantiate(levelButtonPrefab);
			levelButton.transform.parent = scrollView; 
			levelButton.transform.localRotation = Quaternion.identity;
			levelButton.GetComponentInChildren<LevelSelectButton>().Initialize(levelCount);
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void LevelLoadCallback(int level) {
		Debug.Log("loading level " + level.ToString());
	}

	public void OnBeginDrag(PointerEventData eventData) {
		Debug.Log("Dragging Menu");
	}

	public void OnDrag(PointerEventData eventData) {
		Vector3 newPos;
		Vector3 oldPos = transform.position;
		Vector2 dragDelta = eventData.delta;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas, eventData.position, eventData.pressEventCamera, out newPos))
        {
			
			if(levelSelectLocked) return;
			newPos = transform.position;
			newPos.y += dragDelta.y;
            transform.position = newPos;
			rectTransform.ForceUpdateRectTransforms();
			Quaternion curRotation = transform.rotation;
			float curProportion = rectTransform.offsetMax.y / startOffset;
			transform.rotation = Quaternion.Lerp(Quaternion.identity, curRotation, curProportion);
			if(rectTransform.offsetMax.y > 0) {
				transform.position = oldPos;
				lerpProportion = 1;
				levelSelectLocked = true;
			} else {
				lerpProportion = 1 - curProportion;
			}
			float scaleSize = scaleCurve.Evaluate(lerpProportion);
			transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
        }
	}

	public void OnEndDrag(PointerEventData eventData) {
		if(rectTransform.offsetMax.y < 0) {
			StartCoroutine(FinishOpeningMenu());
		}
		Debug.Log("Ending Drag");
	}

	public IEnumerator FinishOpeningMenu() {
		float xOffset = rectTransform.offsetMax.x;
		rectTransform.offsetMax = new Vector2(xOffset, 0);
		rectTransform.ForceUpdateRectTransforms();
		Vector3 targetPos = transform.position;
		rectTransform.offsetMax = new Vector2(xOffset, startOffset);
		rectTransform.ForceUpdateRectTransforms();
		Vector3 startingPos = transform.position;
		float scaleSize = scaleCurve.Evaluate(lerpProportion);
		for(float t = lerpProportion; t < 1; t += Time.deltaTime) {
			transform.position = Vector3.Lerp(startingPos, targetPos, t);
			lerpProportion = t;
			scaleSize = scaleCurve.Evaluate(lerpProportion);
			transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
			yield return null;
		}
		lerpProportion = 1;
		transform.position = targetPos;
		scaleSize = scaleCurve.Evaluate(lerpProportion);
		transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
	}
			
}
