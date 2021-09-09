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
	List<string> foundWords;
	Text scoreTally;
	public TextAsset KanjiData;
	public ContentManager contentManager;
	public ResultModalController resultModal;
	public CookingPotBehavior cookingPot;
	public Transform menu;
	public GameObject GameMenuCanvas;
	public GameObject entreePrefab;
	public Text countdownClock;
	public float requestInterval = 5;
	float requestTimer = 4.5f;
	public int attempts;
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
		resultModal.gameObject.SetActive(false);
		wordBag = new List<EntreeData>();
		requestQueue = GameObject.Find("RequestQueue").GetComponent<RequestQueueManager>();
		scoreTally = GameObject.Find("Score").GetComponentInChildren<Text>();
		menu.transform.parent.gameObject.SetActive(true);
		foundWords = new List<string>();
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
		if(!gameStarted) StartCoroutine(BeginCountdown());
		DismissDuJourMenu();
	}

	IEnumerator BeginCountdown() {
		yield return new WaitForSeconds(0.8f);
		countdownClock.enabled = true;
		for(int i = 3; i > 0; i--) {
			countdownClock.text = i.ToString();
			yield return new WaitForSeconds(0.8f);
		}
		countdownClock.text = "Go!";
		yield return new WaitForSeconds(0.4f);
		countdownClock.enabled = false;
		gameStarted = true;
	}

	public void LevelSetup() {
		levelTimer = 0;
		attempts = 0;
		scoreTally.text = "X 0";
		//LoadKanji();
		SetUpRadicals();
		ShowDuJourMenu(0.0f);
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
		Debug.Log("Restart level called");
		ShowDuJourMenu();
		gameStarted = false;
		requestQueue.ClearRequests();
		levelTimer = 0;
		attempts = 0;
		requestTimer = 4.5f;
		scoreTally.text = "X 0";
	}

	public void OpenLevelSelect() {
		CleanUpGameplay();
	}
	
	void EnableDuJourMenu(){
		menu.parent.gameObject.SetActive(true);
	}

	void DisableDujourMenu() {
		menu.parent.gameObject.SetActive(false);
	}

	public void ShowDuJourMenu(float lerpTime = 0.4f) {
		EnableDuJourMenu();
		RectTransform menuRect = menu.transform.parent.GetComponent<RectTransform>();
		float rectSize = menuRect.rect.size.y;
		StartCoroutine(MenuManager.LerpInsetAnimation(menuRect, -rectSize, 0, lerpTime, RectTransform.Edge.Top));
	}

	void DismissDuJourMenu(float lerpTime = 0.4f) {
		RectTransform menuRect = menu.transform.parent.GetComponent<RectTransform>();
		float rectSize = menuRect.rect.size.y;
		StartCoroutine(MenuManager.LerpInsetAnimation(menuRect, 0, -rectSize, lerpTime, RectTransform.Edge.Top));
		Invoke("DisableDujourMenu", lerpTime);
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
		attempts++;
		if(requestQueue.SatisfiesRequest(result.meanings[0])) {
			StartCoroutine(requestQueue.ClearRequest(answer, result.meanings[0]));
			Debug.Log(scoreTally.text.Substring(1));
			scoreTally.text = "X " + (int.Parse(scoreTally.text.Substring(1)) + 1).ToString();
			if(foundWords.IndexOf(result.literal) == -1) foundWords.Add(result.literal);
		} 
	}
	
	public void CleanUpGameplay() {
		gameStarted = false;
		requestQueue.ClearRequests();
		cookingPot.ClearIngredients();
		foundWords.Clear();
	}

	public void ClearDuJourMenu() {
		menu.parent.GetComponentInChildren<LevelSelectButton>().Clear();
		for(int i = 1; i < menu.childCount; i++) {
			Destroy(menu.GetChild(i).gameObject);
		}
		Invoke("DismissDuJourMenu", 0.5f);
	}	

	void ShowResults() {
		resultModal.gameObject.SetActive(true);
		int score = int.Parse(scoreTally.text.Substring(1));
		resultModal.DisplayResults(score, attempts, foundWords.Count == targetWords.Length);
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
				List<string> componentList = new List<string>(listing.components);
				foreach(string component in components) {
					if(componentList.IndexOf(component) == -1) {
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

