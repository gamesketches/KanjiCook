using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraggableKanji : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	RectTransform canvas;
	public GameObject draggedKanji;
	GameObject kanjiCopy;
	public string character;
	Text displayText;
    // Start is called before the first frame update
    void Awake()
    {
        canvas = transform.parent as RectTransform;
        displayText = GetComponentInChildren<Text>();
		displayText.text = character;
		GameManager.GameEnded += GameEnded;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void SetRadical(string newRadical) {
		displayText.text = newRadical;
		character = newRadical;
	}

	public void OnBeginDrag(PointerEventData eventData) {
		Vector3 newPos;
		kanjiCopy = Instantiate<GameObject>(draggedKanji, transform.parent);
		Text kanjiText = kanjiCopy.GetComponentInChildren<Text>();
		kanjiText.text = character;
		kanjiText.color = AppManager.instance.secondaryColor;
		//kanjiCopy.GetComponentInChildren<Text>().text = character;
		kanjiCopy.transform.position = transform.position;
        if(RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas, eventData.position, 
																		eventData.pressEventCamera, out newPos))
        {
            kanjiCopy.transform.position = newPos;
        }
	}
	
	public void OnDrag(PointerEventData eventData)
    {
        Vector3 newPos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas, eventData.position, eventData.pressEventCamera, out newPos))
        {
            kanjiCopy.transform.position = newPos;
        }
		if(OverCookingPot(eventData.position, eventData.pressEventCamera)) {
			CookingPotBehavior.instance.HighlightPot(true);
		} else {
			CookingPotBehavior.instance.HighlightPot(false);
		}
    }

	public void OnEndDrag(PointerEventData eventData) {
		RectTransform targetRect = CookingPotBehavior.instance.hitRect;
		if(OverCookingPot(eventData.position, eventData.pressEventCamera)) {
			CookingPotBehavior.instance.AddIngredient(character);
        }
		Destroy(kanjiCopy);
	}

	void GameEnded() {
		StartCoroutine(ClearTextAfterDelay(0.7f));
	}

	IEnumerator ClearTextAfterDelay(float delay) {
		yield return new WaitForSeconds(delay);
		displayText.text = "";
		character = "";
	}

	bool OverCookingPot(Vector2 dragPos, Camera eventCamera) {
		RectTransform targetRect = CookingPotBehavior.instance.hitRect;
		return RectTransformUtility.RectangleContainsScreenPoint(targetRect, dragPos,
																	eventCamera);
	}
}
