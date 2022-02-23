using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour, IPointerClickHandler
{
	string uuid;
	int levelIndex;
	public Transform kanjis;
	public Text radicals;
	public GameObject kanjiPrefab;
	public bool interactable;
	public Image[] stars;
	Color flashColor;
	public Sprite filledStar;
	float instancePosition;
	float scrollRectUpperBound;
	float scrollRectLowerBound;
	public static float scrollRectPosition;
	public static float scrollWindowSize;
	public static float scrollTopPadding = 0.3f;
	public static float scrollBottomPadding;

    // Start is called before the first frame update
    void Awake()
    {
		flashColor = AppManager.instance.primaryColor;
		flashColor.a = 0.2f;
		GameManager.GameEnded += UpdatePreviousScore;
    }

    // Update is called once per frame
    void Update()
    {
		float windowBottom = scrollRectPosition - scrollWindowSize;
		if(interactable) {
			if(Mathf.Abs(scrollRectPosition - instancePosition) < scrollWindowSize + scrollBottomPadding) {
				kanjis.gameObject.SetActive(true);
				//radicals.gameObject.SetActive(true);
			} else {
				kanjis.gameObject.SetActive(false);
				//radicals.gameObject.SetActive(false);
			}
		}
    }

	public void Initialize(int id, bool isInteractable = true) {
		string[] levelKanjis = new string[0];
		string[] rads = new string[0];
		interactable = isInteractable;
		levelIndex = id;
		uuid = ContentManager.instance.GetLevelSelectContent(id, out levelKanjis, out rads);
		if(uuid == "none") {
			gameObject.SetActive(false);
		}
		foreach(string kanji in levelKanjis) {
			GameObject newKanji = Instantiate(kanjiPrefab, kanjis);
			newKanji.GetComponentInChildren<Text>().text = kanji;
		}
		/*foreach(string radical in rads) {
			radicals.text += " " + radical;
		}*/
		int prevScore = ProgressTracker.instance.GetScoreForLevel(uuid);
		for(int i = 0; i < prevScore; i++) {
			stars[i].sprite = filledStar;
			stars[i].color = AppManager.instance.secondaryColor;
		}
	}

	public void Initialize(string levelUuid, bool isInteractable = true) {
		uuid = levelUuid;
		string[] levelKanjis = new string[0];
		string[] rads = new string[0];
		interactable = isInteractable;
		ContentManager.instance.GetLevelSelectContent(levelUuid, out levelKanjis, out rads);
		foreach(string kanji in levelKanjis) {
			GameObject newKanji = Instantiate(kanjiPrefab, kanjis);
			newKanji.GetComponentInChildren<Text>().text = kanji;
		}
		/*foreach(string radical in rads) {
			radicals.text += " " + radical;
		}*/
		UpdatePreviousScore();
	}

	void UpdatePreviousScore() {
		int prevScore = ProgressTracker.instance.GetScoreForLevel(uuid);
		for(int i = 0; i < prevScore; i++) {
			stars[i].sprite = filledStar;
			stars[i].color = AppManager.instance.secondaryColor;
		}
	}

	public void CalculateInstancePosition(float elementSize, int position) {
		instancePosition = 1 - (elementSize * position) - (elementSize / 2);
		scrollRectUpperBound = instancePosition + (3 * elementSize);
		scrollRectLowerBound = instancePosition - (3 * elementSize);
		if(scrollRectUpperBound > 1) scrollRectLowerBound -= scrollRectUpperBound - 1;
		if(scrollRectLowerBound < 0) scrollRectUpperBound += -scrollRectLowerBound;
	}

	public void Clear() {
		Debug.Log(kanjis.childCount);
		for(int i = 0; i < kanjis.childCount; i++) {
			Destroy(kanjis.GetChild(i).gameObject);
		}
		//radicals.text = "";
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
		Color baseColor = backgroundImage.color;
		float flashTime = 0.4f;
		for(float t = 0; t < flashTime; t += Time.deltaTime) {
			 backgroundImage.color = Color.Lerp(baseColor, flashColor, Mathf.PingPong(t, flashTime / 2) / (flashTime / 2));
			yield return null;
		}
		backgroundImage.color = baseColor;
		AppManager.instance.SelectLevel(uuid);
	}
}
