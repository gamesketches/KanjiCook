using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelSelect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
	public GameObject levelButtonPrefab;
	Image menuImage;
	RectTransform canvas;
	RectTransform rectTransform;
	float startOffset;
	Vector3 startingScale;
	Vector2 startOffsetMax;
	Vector2 startOffsetMin;
	public float startingRotation;
	public Transform scrollView;
	public int levelsToLoad;
	public static float lerpProportion;
	public static bool levelSelectLocked;
	public Sprite blankMenu;
	public AnimationCurve scaleCurve;
	public AnimationCurve openCurve;
	ScrollRect scrollRect;

    // Start is called before the first frame update
    void Start()
    {
		levelSelectLocked = false;
		lerpProportion = 0;
		canvas = transform.parent.GetComponent<RectTransform>();
		rectTransform = GetComponent<RectTransform>();
		menuImage = GetComponent<Image>();
		scrollRect = GetComponentInChildren<ScrollRect>();
		scrollRect.vertical = false;
		startOffsetMax = rectTransform.offsetMax;
		startOffsetMin = rectTransform.offsetMin;
		startingScale = transform.localScale;
		startOffset = rectTransform.offsetMax.y;
		transform.rotation = Quaternion.Euler(0, 0, startingRotation);
		transform.GetChild(0).gameObject.SetActive(false);
		transform.GetChild(1).gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

	IEnumerator LoadLevelListings() {
		while( !ContentManager.instance.LevelSelectContentReady()) yield return null;
		Debug.Log("Loading Levels");
		int levelCount = 1;
		List<LevelSelectButton> levelButtons = new List<LevelSelectButton>();
		for(levelCount = 1; levelCount < levelsToLoad; levelCount++) {
			if(ContentManager.instance.HasLevelIndex(levelCount)) {
				GameObject levelButton = Instantiate(levelButtonPrefab, scrollView);
				levelButton.transform.localRotation = Quaternion.identity;
				LevelSelectButton buttonBehavior = levelButton.GetComponentInChildren<LevelSelectButton>();
				buttonBehavior.Initialize(levelCount);
				levelButtons.Add(buttonBehavior);
				//levelButton.GetComponentInChildren<LevelSelectButton>().Initialize(levelCount);
			}
		}
		yield return null;
		float scrollRectHeight = scrollView.GetComponent<RectTransform>().rect.height;
		float elementHeight = (levelButtons[0].transform.parent.GetComponent<RectTransform>().rect.height + 50) / scrollRectHeight;
		float windowSize = 7 * elementHeight;
		LevelSelectButton.scrollRectPosition = 1;
		LevelSelectButton.scrollWindowSize = windowSize;
		for(int i = 0; i < levelButtons.Count; i++) {
			levelButtons[i].CalculateInstancePosition(elementHeight, i);
		}
		/*HorizontalLayoutGroup[] hLayouts = scrollView.GetComponentsInChildren<HorizontalLayoutGroup>();
		foreach(HorizontalLayoutGroup hori in hLayouts) {
			hori.enabled = false;
		}*/
	}

	void LevelLoadCallback(int level) {
		Debug.Log("loading level " + level.ToString());
	}

	public void OnPointerClick(PointerEventData eventData) {
		if(!levelSelectLocked) StartCoroutine(FinishOpeningMenu());
	}

	public void OnScrollRectChange(Vector2 newPosition) {
		LevelSelectButton.scrollRectPosition = newPosition.y;
	}

	public void OnBeginDrag(PointerEventData eventData) {
		Debug.Log("Dragging Menu");
		if(levelSelectLocked) StartCoroutine(CloseMenu());
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
				rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, 0);
				rectTransform.offsetMin = new Vector2(rectTransform.offsetMax.x, 0);
				lerpProportion = 1;
				levelSelectLocked = true;
				scrollRect.vertical = true;
				transform.root.GetComponent<MenuManager>().DismissTitleScreen();
				StartCoroutine(LoadLevelListings());
				SwapFakeMenu();
			} else {
				lerpProportion = 1 - curProportion;
			}
			float scaleSize = scaleCurve.Evaluate(lerpProportion);
			transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
        }
	}

	public void OnEndDrag(PointerEventData eventData) {
		if(!levelSelectLocked && rectTransform.offsetMax.y < 0) {
			StartCoroutine(FinishOpeningMenu());
		}
		Debug.Log("Ending Drag");
	}

	public IEnumerator FinishOpeningMenu() {
		levelSelectLocked = true;
		float totalCurveTime = openCurve.keys[openCurve.length - 1].time;
		Vector2 currentOffsetMax = rectTransform.offsetMax;
		Vector2 currentOffsetMin = rectTransform.offsetMin;
		Quaternion curRotation = transform.rotation;
		MenuManager.instance.PlayPageTurnSound();
		for(float t = lerpProportion * totalCurveTime; t < totalCurveTime; t += Time.deltaTime) {
			lerpProportion = t / totalCurveTime;
			float newOffsetMax = Mathf.SmoothStep(currentOffsetMax.y, 0, openCurve.Evaluate(lerpProportion));
			rectTransform.offsetMax = new Vector2(currentOffsetMax.x, newOffsetMax);
			float newOffsetMin = Mathf.SmoothStep(currentOffsetMin.y, 0, openCurve.Evaluate(lerpProportion));
			rectTransform.offsetMin = new Vector2(currentOffsetMin.x, newOffsetMin);
			transform.rotation = Quaternion.Lerp(curRotation, Quaternion.identity, lerpProportion);
			float scaleSize = scaleCurve.Evaluate(lerpProportion);
			transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
			yield return null;
		}
		lerpProportion = 1;
		rectTransform.offsetMax = new Vector2(currentOffsetMax.x, 0);
		rectTransform.offsetMin = new Vector2(currentOffsetMin.x, 0);
		transform.localScale = Vector3.one;
		transform.rotation = Quaternion.identity;
		scrollRect.vertical = true;
		StartCoroutine(LoadLevelListings());
		SwapFakeMenu();
		transform.root.GetComponent<MenuManager>().DismissTitleScreen();
	}

	public IEnumerator CloseMenu() {
		float startingCurveTime = openCurve.keys[openCurve.length - 1].time;
		Vector2 currentOffsetMax = rectTransform.offsetMax;
		Vector2 currentOffsetMin = rectTransform.offsetMin;
		Quaternion targetRotation = Quaternion.Euler(0, 0, startingRotation);
		SwapFakeMenu();
		for(float t = startingCurveTime; t > 0; t -= Time.deltaTime) {
			lerpProportion = t / startingCurveTime;
			float newOffsetMax = Mathf.SmoothStep(startOffsetMax.y, 0, openCurve.Evaluate(lerpProportion));
			rectTransform.offsetMax = new Vector2(currentOffsetMax.x, newOffsetMax);
			float newOffsetMin = Mathf.SmoothStep(startOffsetMin.y, 0, openCurve.Evaluate(lerpProportion));
			rectTransform.offsetMin = new Vector2(currentOffsetMin.x, newOffsetMin);
			transform.rotation = Quaternion.Lerp(targetRotation, Quaternion.identity, lerpProportion);
			float scaleSize = scaleCurve.Evaluate(lerpProportion);
			transform.localScale = new Vector3(scaleSize, scaleSize, scaleSize);
			yield return null;
		}
		lerpProportion = 0;
		rectTransform.offsetMax = startOffsetMax;
		rectTransform.offsetMin = startOffsetMin;
		transform.localScale = Vector3.one;
		transform.rotation = targetRotation;
		transform.localScale = startingScale;
		levelSelectLocked = false;
		scrollRect.vertical = false;
	}

	void SwapFakeMenu() {
		Sprite temp = menuImage.sprite;
		GameObject header = transform.GetChild(0).gameObject;
		GameObject scrollRect = transform.GetChild(1).gameObject;
		header.SetActive(!header.activeSelf);
		scrollRect.SetActive(!scrollRect.activeSelf);
		menuImage.sprite = blankMenu;
		blankMenu = temp;
	}
}
