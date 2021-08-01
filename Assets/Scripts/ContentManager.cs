using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ContentManager : MonoBehaviour
{
	public static ContentManager instance;
	public TextAsset kanjiFile;
	public int numRadicals;
	public int numKanji;
	KanjiInfoFile myKanji;
    // Start is called before the first frame update
    void Awake()
    {
		instance  = this;
        myKanji = JsonUtility.FromJson<KanjiInfoFile>(kanjiFile.text);
		Debug.Log(myKanji.kanjiInfos.Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void GetLevelSelectContent(string filename, out string[] kanjis, out string[] radicals) {
		TextAsset levelFile = Resources.Load<TextAsset>(filename);
		KanjiInfoFile levelKanji = JsonUtility.FromJson<KanjiInfoFile>(levelFile.text);
		List<string> fileKanjis = new List<string>();
		List<string> fileRadicals = new List<string>();
		foreach(KanjiInfo kanji in levelKanji.kanjiInfos) {
			fileKanjis.Add(kanji.kanji);
			foreach(string rad in kanji.radicals) {
				if(fileRadicals.IndexOf(rad) == -1) {
					fileRadicals.Add(rad);
				}
			}
		}
		kanjis = fileKanjis.ToArray();
		radicals = fileRadicals.ToArray();
	}

	public LanguagePair[] LoadLevelContent(string filename) {
		TextAsset levelFile = Resources.Load<TextAsset>(filename);
		KanjiInfoFile levelKanji = JsonUtility.FromJson<KanjiInfoFile>(levelFile.text);
		List<LanguagePair> tempContent = new List<LanguagePair>();
		foreach(KanjiInfo kanji in levelKanji.kanjiInfos) {
			Debug.Log("Adding kanji " + kanji.kanji);
			int randomMeaning = Random.Range(0, kanji.meanings.Length);
			tempContent.Add(new LanguagePair(kanji.meanings[randomMeaning], kanji.kanji, kanji.radicals, kanji.kunyomi, kanji.onyomi));
		}
		return tempContent.ToArray();
	}

	public LanguagePair[] CreateGameContent() {
		List<LanguagePair> tempContent = new List<LanguagePair>();
		List<string> curRadicals = new List<string>();
		while(tempContent.Count < numKanji) {
			LanguagePair newPair = FindNewPair(curRadicals.ToArray());
			if(tempContent.IndexOf(newPair) == -1) {
				tempContent.Add(newPair);
				curRadicals.AddRange(newPair.components);
			}
		}
		return tempContent.ToArray();
	}

	LanguagePair FindNewPair(string[] curRadicals) {
		int numRadComponents;
		KanjiInfo pickedKanji;
		do {
			pickedKanji = myKanji.kanjiInfos[Mathf.FloorToInt(Random.value * (myKanji.kanjiInfos.Length - 1))];
			numRadComponents = pickedKanji.radicals.Length;
			foreach(string rad in pickedKanji.radicals) {
				if(ArrayUtility.Contains(curRadicals, rad)) numRadComponents--;
			}
		} while(curRadicals.Length + numRadComponents > numRadicals);
		return new LanguagePair(pickedKanji.meanings[0], pickedKanji.kanji, pickedKanji.radicals, pickedKanji.kunyomi, pickedKanji.onyomi);
	}
		
}

[System.Serializable]
public class KanjiInfoFile {
	public KanjiInfo[] kanjiInfos;
}

[System.Serializable]
public class KanjiInfo {
	public string[] meanings;
	public string kanji;
	public string[] radicals;
	public string[] kunyomi;
	public string[] onyomi;
}
