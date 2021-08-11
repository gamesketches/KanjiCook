using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour, IPointerClickHandler
{
	string levelFile;
	int levelIndex;
	Text kanjis;
	Text radicals;

    // Start is called before the first frame update
    void Awake()
    {
        Text[] texts = transform.parent.GetComponentsInChildren<Text>();
		Debug.Log(texts.Length);
		kanjis = texts[0];
		radicals = texts[1];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void Initialize(string id, string levelName) {
		levelFile = id;
		string[] levelKanjis = new string[0];
		string[] rads = new string[0];
		ContentManager.instance.GetLevelSelectContent(levelFile, out levelKanjis, out rads);
		foreach(string kanji in levelKanjis) {
			kanjis.text += " " + kanji;
		}
		foreach(string radical in rads) {
			radicals.text += " " + radical;
		}
		//GetComponentInChildren<Text>().text = levelName;
	}

	public void Initialize(int id) {
		string[] levelKanjis = new string[0];
		string[] rads = new string[0];
		levelIndex = id;
		ContentManager.instance.GetLevelSelectContent(id, out levelKanjis, out rads);
		foreach(string kanji in levelKanjis) {
			kanjis.text += " " + kanji;
		}
		foreach(string radical in rads) {
			radicals.text += " " + radical;
		}
	}

	public void OnPointerClick(PointerEventData pointerEventData) {
		StartCoroutine(SelectLevel());
		//GameManager.instance.LoadLevel(levelFile);
	}

	IEnumerator SelectLevel() {
		Image backgroundImage = GetComponentInChildren<Image>();
		float flashTime = 0.4f;
		for(float t = 0; t < flashTime; t += Time.deltaTime) {
			 backgroundImage.color = Color.Lerp(Color.white, Color.black, Mathf.PingPong(t, flashTime / 2) / (flashTime / 2));
			yield return null;
		}
		GameManager.instance.LoadLevel(levelIndex);
	}
}
