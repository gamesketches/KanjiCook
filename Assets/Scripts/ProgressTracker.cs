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

	public void UpdateLevelInfo(int level, int score) {
		curProgress.UpdateLevel(level, score);
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
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/playerProgress.gd", FileMode.Open);
			curProgress = (PlayerProgress)bf.Deserialize(file);
			file.Close();
			PrintCurProgress();
    	} else {
			curProgress = new PlayerProgress();
		}
	}

	void PrintCurProgress() {
		for(int i = 0; i < curProgress.playerProgress.Length; i++) {
			Debug.Log("Level " + i.ToString() + ": " + curProgress.playerProgress[i].numStars.ToString());
		}
	}
}

[System.Serializable]
public class PlayerProgress {
	public LevelProgress[] playerProgress;

	public PlayerProgress() {
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
}

[System.Serializable]
public class LevelProgress {
	public int numStars;

	public LevelProgress() {
		this.numStars = 0;
	}

	public LevelProgress(int stars) {
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
