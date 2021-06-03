using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ContentManager : MonoBehaviour
{
	public TextAsset kanjiFile;
	public int numRadicals;
	public int numKanji;
	KanjiInfoFile myKanji;
    // Start is called before the first frame update
    void Start()
    {
        myKanji = JsonUtility.FromJson<KanjiInfoFile>(kanjiFile.text);
    }

    // Update is called once per frame
    void Update()
    {
        
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
			pickedKanji = myKanji.kanjiInfos[Mathf.FloorToInt(Random.value * myKanji.kanjiInfos.Length)];
			numRadComponents = pickedKanji.radicals.Length;
			foreach(string rad in pickedKanji.radicals) {
				if(ArrayUtility.Contains(curRadicals, rad)) numRadComponents--;
			}
		} while(curRadicals.Length + numRadComponents > numRadicals);
		return new LanguagePair(pickedKanji.meanings[0], pickedKanji.kanji, pickedKanji.radicals);
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
}
