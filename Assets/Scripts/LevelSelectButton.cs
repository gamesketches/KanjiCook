using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour, IPointerClickHandler
{
	string levelFile;
	int levelIndex;
	public Transform kanjis;
	public Text radicals;
	public GameObject kanjiPrefab;
	public bool interactable;
	public Color flashColor;
	float instancePosition;
	float scrollRectUpperBound;
	float scrollRectLowerBound;
	public static float scrollRectPosition;
	public static float scrollWindowSize;

    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
		float windowBottom = scrollRectPosition - scrollWindowSize;
		if(Mathf.Abs(scrollRectPosition - instancePosition) < scrollWindowSize) {
			kanjis.gameObject.SetActive(true);
			radicals.gameObject.SetActive(true);
		} else {
			kanjis.gameObject.SetActive(false);
			radicals.gameObject.SetActive(false);
		}
    }

	public void Initialize(string id, string levelName, bool isInteractable = true) {
		interactable = isInteractable;
		levelFile = id;
		string[] levelKanjis = new string[0];
		string[] rads = new string[0];
		ContentManager.instance.GetLevelSelectContent(levelFile, out levelKanjis, out rads);
		foreach(string kanji in levelKanjis) {
			GameObject newKanji = Instantiate(kanjiPrefab, kanjis);
			newKanji.GetComponentInChildren<Text>().text = kanji;
		}
		foreach(string radical in rads) {
			radicals.text += " " + radical;
		}
		//GetComponentInChildren<Text>().text = levelName;
	}

	public void Initialize(int id, bool isInteractable = true) {
		string[] levelKanjis = new string[0];
		string[] rads = new string[0];
		interactable = isInteractable;
		levelIndex = id;
		ContentManager.instance.GetLevelSelectContent(id, out levelKanjis, out rads);
		foreach(string kanji in levelKanjis) {
			GameObject newKanji = Instantiate(kanjiPrefab, kanjis);
			newKanji.GetComponentInChildren<Text>().text = kanji;
		}
		foreach(string radical in rads) {
			radicals.text += " " + radical;
		}
	}

	public void CalculateInstancePosition(float elementSize, int position) {
		instancePosition = 1 - (elementSize * position) - (elementSize / 2);
		scrollRectUpperBound = instancePosition + (3 * elementSize);
		scrollRectLowerBound = instancePosition - (3 * elementSize);
		if(scrollRectUpperBound > 1) scrollRectLowerBound -= scrollRectUpperBound - 1;
		if(scrollRectLowerBound < 0) scrollRectUpperBound += -scrollRectLowerBound;
		Debug.Log("Position: " + position.ToString());
		Debug.Log("Instance Position: " + instancePosition.ToString());
		Debug.Log("UpperBound: " + scrollRectUpperBound.ToString());
		Debug.Log("LowerBound: " + scrollRectLowerBound.ToString());
	}

	public void Clear() {
		Debug.Log(kanjis.childCount);
		for(int i = 0; i < kanjis.childCount; i++) {
			Destroy(kanjis.GetChild(i).gameObject);
		}
		radicals.text = "";
	}

	public void OnPointerClick(PointerEventData pointerEventData) {
		if(!LevelSelect.levelSelectLocked) {
			transform.root.GetComponent<MenuManager>().OpenLevelSelect();
			return;
		}
		if(interactable)
			StartCoroutine(SelectLevel());
	}

	IEnumerator SelectLevel() {
		Image backgroundImage = GetComponentInChildren<Image>();
		float flashTime = 0.4f;
		for(float t = 0; t < flashTime; t += Time.deltaTime) {
			 backgroundImage.color = Color.Lerp(Color.white, flashColor, Mathf.PingPong(t, flashTime / 2) / (flashTime / 2));
			yield return null;
		}
		backgroundImage.color = Color.white;
		AppManager.instance.SelectLevel(levelIndex);
	}
}
