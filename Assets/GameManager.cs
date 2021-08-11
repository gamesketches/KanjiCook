using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	RequestQueueManager requestQueue;
	public static LanguagePair[] targetWords;
	List<LanguagePair> wordBag;
	Text scoreTally;
	public TextAsset KanjiData;
	public ContentManager contentManager;
	public ResultModalController resultModal;
	public CookingPotBehavior cookingPot;
	public Text menu;
	public GameObject GameMenuCanvas;
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
		GameMenuCanvas.SetActive(true);
		resultModal.gameObject.SetActive(false);
		wordBag = new List<LanguagePair>();
		requestQueue = GameObject.Find("RequestQueue").GetComponent<RequestQueueManager>();
		scoreTally = GameObject.Find("Score").GetComponent<Text>();
		menu.CrossFadeAlpha(0, 0.01f, true);
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
				resultModal.gameObject.SetActive(true);
				resultModal.DisplayRating(int.Parse(scoreTally.text.Substring(1)));
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
		if(!LevelSelect.levelSelectLocked) return;
		GameMenuCanvas.SetActive(false);
		levelFileName = levelName;
		LoadKanji();
		LevelSetup();
	}

	public void LoadLevel(int lvlIndex) {
		if(!LevelSelect.levelSelectLocked) return;
		GameMenuCanvas.SetActive(false);
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
		gameStarted = false;
		requestQueue.ClearRequests();
		GameMenuCanvas.SetActive(true);
	}

	void ShowWordMenu() {
		menu.transform.parent.gameObject.SetActive(true);
		menu.CrossFadeAlpha(1, 1.4f, false);
	}

	void CleanUpGameplay() {
		requestQueue.ClearRequests();
		cookingPot.ClearIngredients();
	}

	void GetLoadedKanji() {
		targetWords = contentManager.GetLevelContent(levelIndex);
		BuildDuJourLevel();
	}

	void LoadKanji() {
		//targetWords = contentManager.CreateGameContent();
		targetWords = contentManager.LoadLevelContent(levelFileName);
		BuildDuJourLevel();	
	}

	void BuildDuJourLevel() {
		string menuDisplay = "";
		foreach(LanguagePair pairing in targetWords) {
			menuDisplay += pairing.target + " = " + pairing.literal + ", made from: " + pairing.components[0];
			for(int i = 1; i < pairing.components.Length; i++) {
				menuDisplay += " + " + pairing.components[i];
			}
			menuDisplay += "\n";
			wordBag.Add(pairing);
		}
		menu.text = menuDisplay;
	}

	void SetUpRadicals() {
		List<string> radicals = new List<string>();
		foreach(LanguagePair pair in targetWords) {
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
			foreach(LanguagePair pairing in targetWords) {
				wordBag.Add(pairing);
			}
		}
		int diceRoll = Mathf.FloorToInt(Random.Range(0, wordBag.Count));
		string request = wordBag[diceRoll].target;
		wordBag.RemoveAt(diceRoll);
		requestQueue.ReceiveRequest(request);
	}

	public void ClearRequest(Text answer, LanguagePair result) {
		if(requestQueue.SatisfiesRequest(result.target)) {
			StartCoroutine(requestQueue.ClearRequest(answer, result.target));
			Debug.Log(scoreTally.text.Substring(1));
			scoreTally.text = "X " + (int.Parse(scoreTally.text.Substring(1)) + 1).ToString();
		}
	}

	public LanguagePair RecipeLookup(string[] components) {
		foreach(LanguagePair listing in targetWords) {
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
		return new LanguagePair("failure", "駄目", new string[0], new string[0], new string[0]);
	}
}

public class LanguagePair {
	public string target;
	public string literal;
	public string[] components;
	public string[] kunyomi;
	public string[] onyomi;
	
	public LanguagePair(string pairTarget, string pairLiteral, string[] pairComponents, string[] pairKunyomi, string[] pairOnyomi) {
		target = pairTarget;
		components = pairComponents;
		literal = pairLiteral;
		kunyomi = pairKunyomi;
		onyomi = pairOnyomi;
	}
}

