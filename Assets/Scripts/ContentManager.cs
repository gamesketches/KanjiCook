using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ContentManager : MonoBehaviour
{
	public static ContentManager instance;
	public int numRadicals;
	public int numKanji;
	bool loadingLevels = true;
	KanjiInfoFile myKanji;
	Dictionary<string, EntreeData[]> levelLookup;
	List<string> levelIds;
	string[] packsOwned;
	
	[SerializeField] private AssetLabelReference levelLabel;
	private AsyncOperationHandle _levelLoadOperationHandle;
    // Start is called before the first frame update
    void Awake()
    {
		instance = this;
		levelLookup = new Dictionary<string, EntreeData[]>();
		packsOwned = new string[] {"LevelContent", "jlpt5", "jlpt4", "jlpt3", "jlpt2", "jlpt1"};
		StartCoroutine(LoadLevelsFromResources());
		//StartCoroutine(LoadOwnedLevels());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	IEnumerator LoadLevelsFromResources() {
		loadingLevels = true;
		int levelCount = 1;
		foreach(string pack in packsOwned) {
			TextAsset[] levels = Resources.LoadAll<TextAsset>("Levels/" + pack);
			foreach(TextAsset level in levels) {
				string newID = ProcessLevelContent(level);
				levelIds.Add(newID);
				/*EntreeData[] levelContent = ProcessLevelContent(level);
				levelLookup.Add(levelCount, levelContent);
				packLookup.Add(levelCount, pack);
				levelCount++;*/
			}
			yield return null;
			loadingLevels = false;
		}
	}

	IEnumerator LoadOwnedLevels() {
		loadingLevels = true;
		foreach(string pack in packsOwned) {
			yield return StartCoroutine(LoadLevelsByLabel(pack));
			loadingLevels = false;
		}
	}

	IEnumerator LoadLevelsByLabel(string label) {
		AsyncOperationHandle<IList<TextAsset>> loadWithSingleKeyHandle = Addressables.LoadAssetsAsync<TextAsset>(label, obj =>
			{
				//Gets called for every loaded asset
				Debug.Log(obj.name);
				levelIds.Add(ProcessLevelContent(obj));
				//int levelIndex = int.Parse(obj.name.Substring(5));
				//levelLookup.Add(levelIndex, levelContent);
			});
		yield return loadWithSingleKeyHandle;
		IList<TextAsset> singleKeyResult = loadWithSingleKeyHandle.Result;
		Addressables.Release(loadWithSingleKeyHandle);
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
			levelIds.Add(ProcessLevelContent(obj));
			//int levelIndex = int.Parse(obj.name.Substring(5));
			//levelLookup.Add(levelIndex, levelContent);
    	});
		yield return loadWithSingleKeyHandle;
		IList<TextAsset> singleKeyResult = loadWithSingleKeyHandle.Result;
		Addressables.Release(loadWithSingleKeyHandle);
		loadingLevels = false;
	}

	public bool LevelSelectContentReady() {
		return !loadingLevels;
	}

	public void GetLevelSelectContent(string uuid, out string[] kanjis, out string[] radicals) {
		EntreeData[] theLevel = levelLookup[uuid];
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

	public string GetLevelSelectContent(int index, out string[] kanjis, out string[] radicals) {
		string levelUUID = levelIds[index];
		EntreeData[] theLevel = levelLookup[levelUUID];
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
		return levelUUID;
		//packName = packLookup[index];
	}

	/*public bool HasLevelIndex(int index) {
		return levelLookup.ContainsKey(index);
	}*/

	public EntreeData[] LoadLevelContent(string uuid) {
		return levelLookup[uuid];
		//return ProcessLevelContent(levelFile);
	}

	public EntreeData[] GetLevelContent(string levelId) {
		return levelLookup[levelId];
	}

	/*public string LevelPackLookup(int levelIndex) {
		return packLookup[levelIndex];
	}*/

	/*EntreeData[] ProcessLevelContent(TextAsset file) {
		KanjiInfoFile levelKanji = JsonUtility.FromJson<KanjiInfoFile>(file.text);
		List<EntreeData> tempContent = new List<EntreeData>();
		foreach(KanjiInfo kanji in levelKanji.kanjiInfos) {
			//int randomMeaning = Random.Range(0, kanji.meanings.Length);
			/*for(int i = 0; i < tempContent.Count; i++) {
				if(tempContent[i].meanings == kanji.meanings[randomMeaning]) {
					i = -1;
					randomMeaning = Random.Range(0, kanji.meanings.Length);
				}
			}
			tempContent.Add(new EntreeData(kanji.meanings, kanji.kanji, kanji.radicals, kanji.kunyomi, kanji.onyomi));
		}
		return tempContent.ToArray();
	}*/

	string ProcessLevelContent(TextAsset file) {
		KanjiInfoFile levelKanji = JsonUtility.FromJson<KanjiInfoFile>(file.text);
		List<EntreeData> tempContent = new List<EntreeData>();
		foreach(KanjiInfo kanji in levelKanji.kanjiInfos) {
			tempContent.Add(new EntreeData(kanji.meanings, kanji.kanji, kanji.radicals, kanji.kunyomi, kanji.onyomi));
		}
		levelLookup[levelKanji.uuid] = tempContent.ToArray();
		return levelKanji.uuid;
	}

	public EntreeData[] CreateGameContent() {
		List<EntreeData> tempContent = new List<EntreeData>();
		List<string> curRadicals = new List<string>();
		while(tempContent.Count < numKanji) {
			EntreeData newPair = FindNewPair(curRadicals.ToArray());
			if(tempContent.IndexOf(newPair) == -1) {
				tempContent.Add(newPair);
				curRadicals.AddRange(newPair.components);
			}
		}
		return tempContent.ToArray();
	}

	EntreeData FindNewPair(string[] curRadicals) {
		int numRadComponents;
		KanjiInfo pickedKanji;
		do {
			pickedKanji = myKanji.kanjiInfos[Mathf.FloorToInt(Random.value * (myKanji.kanjiInfos.Length - 1))];
			numRadComponents = pickedKanji.radicals.Length;
			foreach(string rad in pickedKanji.radicals) {
				for(int i = 0; i < curRadicals.Length; i++) {
					if(curRadicals[i] == rad) numRadComponents--;
				}
			}
		} while(curRadicals.Length + numRadComponents > numRadicals);
		return new EntreeData(pickedKanji.meanings, pickedKanji.kanji, pickedKanji.radicals, pickedKanji.kunyomi, pickedKanji.onyomi);
	}
		

	public void AddLevelPack(string pack) {
		List<string> ownedPacksList = new List<string>(packsOwned);
		ownedPacksList.Add(pack);
		packsOwned = ownedPacksList.ToArray();
	}
}

[System.Serializable]
public class KanjiInfoFile {
	public KanjiInfo[] kanjiInfos;
	public string uuid;
}

[System.Serializable]
public class KanjiInfo {
	public string[] meanings;
	public string kanji;
	public string[] radicals;
	public string[] kunyomi;
	public string[] onyomi;
}

