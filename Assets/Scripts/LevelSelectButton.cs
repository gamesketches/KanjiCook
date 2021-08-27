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

    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
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

	public void Clear() {
		Debug.Log(kanjis.childCount);
		for(int i = 0; i < kanjis.childCount; i++) {
			Destroy(kanjis.GetChild(i).gameObject);
		}
		radicals.text = "";
	}

	public void OnPointerClick(PointerEventData pointerEventData) {
		if(interactable)
			StartCoroutine(SelectLevel());
	}

	IEnumerator SelectLevel() {
		Image backgroundImage = GetComponentInChildren<Image>();
		float flashTime = 0.4f;
		for(float t = 0; t < flashTime; t += Time.deltaTime) {
			 backgroundImage.color = Color.Lerp(Color.white, Color.black, Mathf.PingPong(t, flashTime / 2) / (flashTime / 2));
			yield return null;
		}
		AppManager.instance.SelectLevel(levelIndex);
	}
}
