using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour, IPointerClickHandler
{
	string levelFile;
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

	public void OnPointerClick(PointerEventData pointerEventData) {
		GameManager.instance.LoadLevel(levelFile);
	}
}
