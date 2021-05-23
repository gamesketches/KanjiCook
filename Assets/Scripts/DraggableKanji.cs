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
	public char character;
	Text displayText;
    // Start is called before the first frame update
    void Start()
    {
        canvas = transform.parent as RectTransform;
        displayText = GetComponentInChildren<Text>();
		displayText.text = character.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OnBeginDrag(PointerEventData eventData) {
		Vector3 newPos;
		kanjiCopy = Instantiate<GameObject>(draggedKanji);
		kanjiCopy.GetComponentInChildren<Text>().text = character.ToString();
		kanjiCopy.GetComponent<Image>().color = gameObject.GetComponent<Image>().color;
		kanjiCopy.transform.parent = transform.parent;
		kanjiCopy.transform.position = transform.position;
        if(RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas, eventData.position, eventData.pressEventCamera, out newPos))
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
    }

	public void OnEndDrag(PointerEventData eventData) {
		Vector3 newPos;
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle(CookingPotBehavior.instance.hitRect, eventData.position, eventData.pressEventCamera, out newPos))
        {
			Destroy(kanjiCopy);
			CookingPotBehavior.instance.AddIngredient(character);
        }
	}
}
