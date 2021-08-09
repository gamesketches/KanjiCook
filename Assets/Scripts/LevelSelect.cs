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
    // Start is called before the first frame update
    IEnumerator Start()
    {
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
			newPos = transform.position;
			newPos.y += dragDelta.y;
            transform.position = newPos;
			rectTransform.ForceUpdateRectTransforms();
			Quaternion curRotation = transform.rotation;
			transform.rotation = Quaternion.Lerp(Quaternion.identity, curRotation, rectTransform.offsetMax.y / startOffset);
			if(rectTransform.offsetMax.y > 0) {
				transform.position = oldPos;
			}
        }
	}

	public void OnEndDrag(PointerEventData eventData) {
		Debug.Log("Ending Drag");
	}
}
