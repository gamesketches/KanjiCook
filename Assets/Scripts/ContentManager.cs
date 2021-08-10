using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ContentManager : MonoBehaviour
{
	public static ContentManager instance;
	public TextAsset kanjiFile;
	public int numRadicals;
	public int numKanji;
	bool loadingLevels = true;
	KanjiInfoFile myKanji;
	Dictionary<int, LanguagePair[]> levelLookup;
	
	[SerializeField] private AssetLabelReference levelLabel;
	private AsyncOperationHandle _levelLoadOperationHandle;
    // Start is called before the first frame update
    void Awake()
    {
		instance = this;
        myKanji = JsonUtility.FromJson<KanjiInfoFile>(kanjiFile.text);
		levelLookup = new Dictionary<int, LanguagePair[]>();
		StartCoroutine(LoadLevels());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	IEnumerator LoadLevels() {
		loadingLevels = true;
		if(_levelLoadOperationHandle.IsValid()) {
			Addressables.Release(_levelLoadOperationHandle);
		}
		AsyncOperationHandle<IList<TextAsset>> loadWithSingleKeyHandle = 
							Addressables.LoadAssetsAsync<TextAsset>(levelLabel, obj => {
        //Gets called for every loaded asset
			Debug.Log(obj.name);
			LanguagePair[] levelContent = ProcessLevelContent(obj);
			int levelIndex = int.Parse(obj.name.Substring(5));
			levelLookup.Add(levelIndex, levelContent);
    	});
		yield return loadWithSingleKeyHandle;
		IList<TextAsset> singleKeyResult = loadWithSingleKeyHandle.Result;
		Addressables.Release(loadWithSingleKeyHandle);
		loadingLevels = false;
	}

	public bool LevelSelectContentReady() {
		return !loadingLevels;
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

	public void GetLevelSelectContent(int index, out string[] kanjis, out string[] radicals) {
		LanguagePair[] theLevel = levelLookup[index];
		List<string> fileKanjis = new List<string>();
		List<string> fileRadicals = new List<string>();
		for(int i = 0; i < theLevel.Length; i++) {
			fileKanjis.Add(theLevel[i].literal);
			foreach(string rad in theLevel[i].components) {
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
		//KanjiInfoFile levelKanji = JsonUtility.FromJson<KanjiInfoFile>(levelFile.text);
		return ProcessLevelContent(levelFile);
		/*List<LanguagePair> tempContent = new List<LanguagePair>();
		foreach(KanjiInfo kanji in levelKanji.kanjiInfos) {
			Debug.Log("Adding kanji " + kanji.kanji);
			int randomMeaning = Random.Range(0, kanji.meanings.Length);
			for(int i = 0; i < tempContent.Count; i++) {
				if(tempContent[i].target == kanji.meanings[randomMeaning]) {
					i = -1;
					randomMeaning = Random.Range(0, kanji.meanings.Length);
				}
			}
			tempContent.Add(new LanguagePair(kanji.meanings[randomMeaning], kanji.kanji, kanji.radicals, kanji.kunyomi, kanji.onyomi));
		}
		return tempContent.ToArray();*/
	}

	LanguagePair[] ProcessLevelContent(TextAsset file) {
		KanjiInfoFile levelKanji = JsonUtility.FromJson<KanjiInfoFile>(file.text);
		List<LanguagePair> tempContent = new List<LanguagePair>();
		foreach(KanjiInfo kanji in levelKanji.kanjiInfos) {
			Debug.Log("Adding kanji " + kanji.kanji);
			int randomMeaning = Random.Range(0, kanji.meanings.Length);
			for(int i = 0; i < tempContent.Count; i++) {
				if(tempContent[i].target == kanji.meanings[randomMeaning]) {
					i = -1;
					randomMeaning = Random.Range(0, kanji.meanings.Length);
				}
			}
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

