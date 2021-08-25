using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	RequestQueueManager requestQueue;
	public static EntreeData[] targetWords;
	List<EntreeData> wordBag;
	Text scoreTally;
	public TextAsset KanjiData;
	public ContentManager contentManager;
	public ResultModalController resultModal;
	public CookingPotBehavior cookingPot;
	public Transform menu;
	public GameObject GameMenuCanvas;
	public GameObject entreePrefab;
	public float requestInterval = 5;
	float requestTimer = 4.5f;
	public float levelDuration;
	float levelTimer;
	bool gameStarted = false;
	string levelFileName;
	int levelIndex;

    // Start is called before the first frame update
    void Awake()
    {
		instance = this;
		levelTimer = 0;
	//	GameMenuCanvas.SetActive(true);
		resultModal.gameObject.SetActive(false);
		wordBag = new List<EntreeData>();
		requestQueue = GameObject.Find("RequestQueue").GetComponent<RequestQueueManager>();
		scoreTally = GameObject.Find("Score").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
		if(gameStarted) {
			requestTimer += Time.deltaTime;
			if(requestTimer >= requestInterval) {
				MakeNewRequest();
				requestTimer = 0;
			}
			levelTimer += Time.deltaTime;
			if(levelTimer > levelDuration) {
				gameStarted = false;
				CleanUpGameplay();
				ShowResults();
				Debug.Log("Your score is " + scoreTally.text);
			}
		}
    }

	public void StartGame() {
		gameStarted = true;
		menu.transform.parent.gameObject.SetActive(false);
	}

	public void LevelSetup() {
		levelTimer = 0;
		scoreTally.text = "X 0";
		//LoadKanji();
		SetUpRadicals();
		ShowWordMenu();
	}

	public void LoadLevel(string levelName) {
		levelFileName = levelName;
		LoadKanji();
		LevelSetup();
	}

	public void LoadLevel(int lvlIndex) {
		levelIndex = lvlIndex;
		GetLoadedKanji();
		LevelSetup();
	}

	public void RestartLevel() {
		gameStarted = false;
		requestQueue.ClearRequests();
		levelTimer = 0;
		requestTimer = 4.5f;
		scoreTally.text = "X 0";
		ShowWordMenu();
	}

	public void OpenLevelSelect() {
		CleanUpGameplay();
	}

	void ShowWordMenu() {
		menu.transform.parent.gameObject.SetActive(true);
		//menu.CrossFadeAlpha(1, 1.4f, false);
	}

	void GetLoadedKanji() {
		targetWords = contentManager.GetLevelContent(levelIndex);
		for(int i = 0; i < targetWords.Length; i++) {
			targetWords[i] = TrimEntreeData(targetWords[i]);
		}
		BuildDuJourLevel();
	}

	void LoadKanji() {
		//targetWords = contentManager.CreateGameContent();
		targetWords = contentManager.LoadLevelContent(levelFileName);
		BuildDuJourLevel();	
	}

	void BuildDuJourLevel() {
		menu.parent.GetComponentInChildren<LevelSelectButton>().Initialize(levelIndex, false);
		foreach(EntreeData pairing in targetWords) {
			GameObject entreeListing = Instantiate<GameObject>(entreePrefab, menu);
			entreePrefab.GetComponent<EntreeBehavior>().Initialize(pairing);
		}
	}

	void SetUpRadicals() {
		List<string> radicals = new List<string>();
		foreach(EntreeData pair in targetWords) {
			foreach(string radical in pair.components) {
				if(radicals.IndexOf(radical) == -1) {
					radicals.Add(radical);
				}
			}
		}
		GameObject[] radicalButtons = GameObject.FindGameObjectsWithTag("RadicalButton");
		if(radicals.Count > radicalButtons.Length) { 
			Debug.LogError("There are too many radicals for the number of buttons");
		} else {
			for(int i = 0; i < radicalButtons.Length; i++) {
				if(i < radicals.Count) {
					radicalButtons[i].GetComponent<DraggableKanji>().SetRadical(radicals[i]);
				} else {
					radicalButtons[i].SetActive(false);
				}
			}
		}
	}

	void MakeNewRequest() {
		if(wordBag.Count == 0) {
			foreach(EntreeData pairing in targetWords) {
				wordBag.Add(pairing);
			}
		}
		int diceRoll = Mathf.FloorToInt(Random.Range(0, wordBag.Count));
		string request = wordBag[diceRoll].meanings[0];
		wordBag.RemoveAt(diceRoll);
		requestQueue.ReceiveRequest(request);
	}

	public void ClearRequest(Text answer, EntreeData result) {
		if(requestQueue.SatisfiesRequest(result.meanings[0])) {
			StartCoroutine(requestQueue.ClearRequest(answer, result.meanings[0]));
			Debug.Log(scoreTally.text.Substring(1));
			scoreTally.text = "X " + (int.Parse(scoreTally.text.Substring(1)) + 1).ToString();
		}
	}
	
	public void CleanUpGameplay() {
		gameStarted = false;
		requestQueue.ClearRequests();
		cookingPot.ClearIngredients();
	}

	void ShowResults() {
		resultModal.gameObject.SetActive(true);
		int score = int.Parse(scoreTally.text.Substring(1));
		int attempts = 1;
		float successRate = ((float)score / (float) attempts) * 100;
		string performanceDescription = "You cooked " + score.ToString() + " dishes!\n" + 
										"You made " + attempts.ToString() + " attempts\n" + 
										"and " + successRate + "% of them were correct!\n" + 
										"You have potential\nKeep up the good work!";
		resultModal.DisplayResults(score, performanceDescription);
	}

	public EntreeData TrimEntreeData(EntreeData theData) {
		EntreeData newData = new EntreeData();
		newData.literal = theData.literal;
		newData.components = theData.components;
		if(theData.meanings.Length > 1) {
			newData.meanings = new string[] {theData.meanings[Random.Range(0, theData.meanings.Length)]};
		} else newData.meanings = theData.meanings;
		if(theData.kunyomi.Length > 1) {
			newData.kunyomi = new string[] {theData.kunyomi[Random.Range(0, theData.kunyomi.Length)]};
		} else newData.kunyomi = theData.kunyomi;
		if(theData.onyomi.Length > 1) {
			newData.onyomi = new string[] {theData.onyomi[Random.Range(0, theData.onyomi.Length)]};
		} else newData.onyomi = theData.onyomi;
		return newData;
	}

	public EntreeData RecipeLookup(string[] components) {
		foreach(EntreeData listing in targetWords) {
			if(listing.components.Length == components.Length) {
				bool match = true;
				foreach(string component in components) {
					if(ArrayUtility.IndexOf(listing.components, component) == -1) {
						match = false;
					}
				}
				if(match) {
					return listing;
				}
			}
		}
		return new EntreeData(new string[] {"failure"}, "駄目", new string[0], new string[0], new string[0]);
	}
}

public class EntreeData {
	public string[] meanings;
	public string literal;
	public string[] components;
	public string[] kunyomi;
	public string[] onyomi;
	
	public EntreeData() {
		meanings = new string[0];
		literal = "";
		components = new string[0];
		kunyomi = new string[0];
		onyomi = new string[0];
	}
		
	public EntreeData(string[] kanjiMeanings, string entreeLiteral, string[] entreeComponents, string[] entreeKunyomi, string[] entreeOnyomi) {
		meanings = kanjiMeanings;
		components = entreeComponents;
		literal = entreeLiteral;
		kunyomi = entreeKunyomi;
		onyomi = entreeOnyomi;
	}
}

