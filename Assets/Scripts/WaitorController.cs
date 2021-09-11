using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitorController : MonoBehaviour
{
	public static WaitorController instance;
	public float waitorInterval;
	bool animating;
	float offscreenTimer;
	Vector3 waitorPosition;
	public float waitorTravelTime;
	public GameObject characterSprite;
	public Text character;
	RectTransform rectTransform;

    // Start is called before the first frame update
    void Awake()
    {
        offscreenTimer = waitorInterval;
		waitorPosition = transform.position;
		transform.position += new Vector3(10, 0, 0);
		rectTransform = GetComponent<RectTransform>();
		//MenuManager.LerpInsetAnimation(rectTransform, 0, , Time.deltaTime);
		float rectSize = rectTransform.rect.size.x;
		rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, -rectSize * 2, rectSize);
		instance = this;
		characterSprite.SetActive(false);
		character.text = "";
    }

    // Update is called once per frame
    void Update()
    {
    }

	public void PickUpOrder(string literal = "字") {
		if(animating) return;
		character.text = literal;
		animating = true;
		//StartCoroutine(WaitorWalksIn());
		StartCoroutine(WaitorWalksInCanvas());
	}

	IEnumerator WaitorWalksInCanvas() {
		character.gameObject.SetActive(false);
		transform.localScale = Vector3.one;
		yield return MenuManager.LerpInsetAnimation(rectTransform, -rectTransform.rect.size.x * 2, 0, waitorTravelTime);
		character.gameObject.SetActive(true);
		yield return new WaitForSeconds(1);
		transform.localScale = new Vector3(-1, 1, 1);
		yield return MenuManager.LerpInsetAnimation(rectTransform, 0, -rectTransform.rect.size.x * 2, waitorTravelTime);
		animating = false;
		character.text = "";
	}

	IEnumerator WaitorWalksIn() {
		characterSprite.SetActive(false);
		Vector3 offscreenPos = transform.position;
		transform.localScale = Vector3.one;
		for(float t = 0; t < waitorTravelTime; t += Time.deltaTime) {
			transform.position = Vector3.Lerp(offscreenPos, waitorPosition, t / waitorTravelTime);
			yield return null;
		}
		characterSprite.SetActive(true);
		yield return new WaitForSeconds(1);
		transform.localScale = new Vector3(-1, 1, 1);
		for(float t = 0; t < waitorTravelTime; t += Time.deltaTime) {
			transform.position = Vector3.Lerp(waitorPosition, offscreenPos, t / waitorTravelTime);
			yield return null;
		}
		animating = false;
		character.text = "";
	}
}
