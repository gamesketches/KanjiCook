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
	public int numLevels;
	bool loadingLevels = true;
	KanjiInfoFile myKanji;
	Dictionary<string, EntreeData[]> levelLookup;
	Dictionary<string, string> charReplacementDict;
	Dictionary<string, string> packAddressLookup;
	List<string> levelIds;
	string[] packsOwned;
	public LevelSelect levelSelect;
	
	[SerializeField] private AssetLabelReference levelLabel;
	private AsyncOperationHandle _levelLoadOperationHandle;
    // Start is called before the first frame update
    void Awake()
    {
		instance = this;
		levelLookup = new Dictionary<string, EntreeData[]>();
		levelIds = new List<string>();
		packsOwned = new string[] {"LevelContent"};
		BuildPackAddressLookup();
		//AddAllPacks();
		BuildCharReplacementDict();
		CheckOwnedPacks();
		StartCoroutine(LoadLevelsFromResources());
		//StartCoroutine(LoadOwnedLevels());
    }

	IEnumerator LoadLevelsFromResources() {
		loadingLevels = true;
		foreach(string pack in packsOwned) {
			yield return LoadLevelPack(pack);
			loadingLevels = false;
		}
	}

	IEnumerator LoadLevelPack(string packName, bool triggerLevelSelect = false) {
		string packAddress = packAddressLookup[packName];
		TextAsset[] levels = Resources.LoadAll<TextAsset>("Levels/" + packAddress);
			foreach(TextAsset level in levels) {
				string newID = ProcessLevelContent(level);
				levelIds.Add(newID);
				yield return null;
			}
	   if(triggerLevelSelect) {
			levelSelect.LoadNewLevels();
			}
		numLevels = levelIds.Count;
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
		if(index > levelIds.Count) {
			kanjis = new string[0];
			radicals = new string[0];
			return "none";
		}
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
			string targetKanji = GetValidatedCharacter(kanji.kanji);
			List<string> rads = new List<string>(kanji.radicals);
			for (int i = 0; i < rads.Count; i++) rads[i] = GetValidatedCharacter(rads[i]);
			tempContent.Add(new EntreeData(kanji.meanings, targetKanji, rads.ToArray(), kanji.kunyomi, kanji.onyomi));
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
		
	private void CheckOwnedPacks() {
		List<string> packsToAdd = new List<string>();
		packsToAdd.Add("LevelContent");
		for (int i = 5; i > 1; i--)
		{
			string packString = "com.bluesphere.kanjicook.jlpt" + i.ToString();
			if (PlayerPrefs.HasKey(packString))
			{
				if (PlayerPrefs.GetInt(packString) == 1)
				{
					packsToAdd.Add(packString);
				}
			}
			else PlayerPrefs.SetInt(packString, 0);
		}
		packsOwned = packsToAdd.ToArray();
	}

	private void ClearOwnedPacks() { 
		for (int i = 5; i > 1; i--)
		{
			string packString = "com.bluesphere.kanjicook.jlpt" + i.ToString();
			PlayerPrefs.SetInt(packString, 0);
		}
		PlayerPrefs.Save();
	}

	private void AddAllPacks() { 
		for (int i = 5; i > 1; i--)
		{
			string packString = "com.bluesphere.kanjicook.jlpt" + i.ToString();
			PlayerPrefs.SetInt(packString, 1);
		}
		PlayerPrefs.Save();
    }

	public void AddLevelPack(string pack) {
		List<string> ownedPacksList = new List<string>(packsOwned);
		ownedPacksList.Add(pack);
		packsOwned = ownedPacksList.ToArray();
		AddOwnedPack(pack);
		StartCoroutine(LoadLevelPack(pack, true));
	}

	public bool AlreadyOwned(string pack) { 
		if(PlayerPrefs.HasKey(pack)) {
			return PlayerPrefs.GetInt(pack) == 1;
		}
		return false;
	}

	private void AddOwnedPack(string pack) {
		string packKey = pack;
		if (!PlayerPrefs.HasKey(packKey))
		{
			Debug.Log("adding pack and key " + packKey);
			PlayerPrefs.SetInt(packKey, 1);
		}
		else if (PlayerPrefs.GetInt(packKey) == 0)
		{
			Debug.Log("adding pack " + packKey);
			PlayerPrefs.SetInt(packKey, 1);
		}
		else Debug.Log(PlayerPrefs.GetInt(packKey));
		PlayerPrefs.Save();
	}

	void BuildPackAddressLookup() {
		packAddressLookup = new Dictionary<string, string>();
		packAddressLookup.Add("LevelContent", "LevelContent");
		packAddressLookup.Add("com.bluesphere.kanjicook.jlpt5", "jlpt5");
		packAddressLookup.Add("com.bluesphere.kanjicook.jlpt4", "jlpt4");
		packAddressLookup.Add("com.bluesphere.kanjicook.jlpt3", "jlpt3");
		packAddressLookup.Add("com.bluesphere.kanjicook.jlpt2", "jlpt2");
    }

	private void BuildCharReplacementDict() {
		charReplacementDict = new Dictionary<string, string>();
		charReplacementDict.Add("\ud840\udda2", "\u4eba");
	}

	public string GetValidatedCharacter(string relevantCharacter) {
		if (charReplacementDict.ContainsKey(relevantCharacter))
		{
			relevantCharacter = charReplacementDict[relevantCharacter];
		}
		return relevantCharacter;
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

