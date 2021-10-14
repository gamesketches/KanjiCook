using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class ProgressTracker : MonoBehaviour
{
	public static ProgressTracker instance;
	PlayerProgress curProgress;
    // Start is called before the first frame update
    void Start()
    {
		instance = this;
        LoadProgressData();
    }

	public void UpdateLevelInfo(string uuid, int score) {
		curProgress.UpdateLevel(uuid, score);
		SaveProgressData();
	}

	void SaveProgressData() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create (Application.persistentDataPath + "/playerProgress.gd");
		bf.Serialize(file, curProgress);
		file.Close();
	}

	void LoadProgressData() {
		if(File.Exists(Application.persistentDataPath + "/playerProgress.gd")) {
			Debug.Log(Application.persistentDataPath);
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/playerProgress.gd", FileMode.Open);
			curProgress = (PlayerProgress)bf.Deserialize(file);
			file.Close();
    	} else {
			curProgress = new PlayerProgress();
		}
	}

	void PrintCurProgress() {
		try {
			for(int i = 0; i < curProgress.levels.Length; i++) {
				LevelProgress level = curProgress.levels[i];
				Debug.Log(level.uuid);
				Debug.Log(level.numStars);
			}
		} catch {
			Debug.LogWarning("Tried to print from progress file, probably an old format");
		}
	}

	public int GetScoreForLevel(string levelId) {
		for(int i = 0; i < curProgress.levels.Length; i++) {
			if(curProgress.levels[i].uuid == levelId) {
				return curProgress.levels[i].numStars;
			}
		}
		return 0;
	}
}

[System.Serializable]
public class PlayerProgress {
	public LevelProgress[] levels;

	public PlayerProgress() {
		levels = new LevelProgress[0];
	}

	public void UpdateLevel(string levelId, int score) {
		for(int i = 0; i < levels.Length; i++) {
			if(levels[i].uuid == levelId) {
				levels[i].UpdateScore(score);
				return;
			}
		}
		List<LevelProgress> temp = new List<LevelProgress>(levels);
		temp.Add(new LevelProgress(levelId, score));
		levels = temp.ToArray();
		/*for(int i = 0; i < packs.Length; i++) {
			if(packs[i].packName == packName) {
				packs[i].UpdateLevel(level, score);
				return;
			}
		}
		List<PackProgress> temp = new List<PackProgress>(packs);
		PackProgress newPack = new PackProgress(packName);
		newPack.UpdateLevel(level, score);
		temp.Add(newPack);
		packs = temp.ToArray();*/
	}
		
}

/*[System.Serializable]
public class PackProgress {
	public string packName;
	public LevelProgress[] playerProgress;

	public PackProgress(string packName = "LevelContent") {
		playerProgress = new LevelProgress[0];
	}

	public void UpdateLevel(int level, int score) {
		if(level > playerProgress.Length) {
			LevelProgress[] tempArray = new LevelProgress[level];
			for(int i = 0; i < level - 1; i++) {
				if(i < playerProgress.Length) {
					tempArray[i] = playerProgress[i];
				} else {
					tempArray[i] = new LevelProgress();
				}
			}
			tempArray[level - 1] = new LevelProgress(score);
			playerProgress = tempArray;
		} else {
			playerProgress[level - 1].UpdateScore(score);
		}
	}
}*/

[System.Serializable]
public class LevelProgress {
	public int numStars;
	public string uuid;

	public LevelProgress(string uuid) {
		this.numStars = 0;
		this.uuid = uuid;
	}

	public LevelProgress(int stars) {
		this.numStars = stars;
	}
	
	public LevelProgress(string uuid, int stars) {
		this.uuid = uuid;
		this.numStars = stars;
	}

	public void UpdateScore(int score) {
		Debug.Log("CurScore: " + numStars.ToString());
		Debug.Log("NewScore: " + score.ToString());
		if(score > numStars) {
			numStars = score;
		}
	}
}
