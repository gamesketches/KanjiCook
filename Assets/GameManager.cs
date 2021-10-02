using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public enum StudyType {Meaning, Kunyomi, Onyomi};
public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	public static StudyType studyType = StudyType.Meaning;
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
	public CountdownController countdownClock;
	public Text timer;
	public float requestInterval = 5;
	float requestTimer = 4.5f;
	public int attempts;
	public float levelDuration;
	float levelTimer;
	bool gameStarted = false;
	string levelFileName;
	public static string levelId = "none";

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
			UpdateTimer();
			if(levelTimer > levelDuration) {
				gameStarted = false;
				timer.text = "";
				CleanUpGameplay();
				ShowResults();
			}
		}
    }

	public void StartGame() {
		if(!gameStarted) StartCoroutine(BeginCountdown());
		DismissDuJourMenu();
	}

	IEnumerator BeginCountdown() {
		yield return new WaitForSeconds(0.8f);
		for(int i = 3; i > 0; i--) {
			countdownClock.UpdateClockNumber(i);
			yield return new WaitForSeconds(0.8f);
		}
		countdownClock.UpdateClockGo();
		yield return new WaitForSeconds(0.4f);
		countdownClock.ClearCountdown();
		gameStarted = true;
		timer.text = levelDuration.ToString();
	}

	public void LevelSetup() {
		levelTimer = 0;
		attempts = 0;
		scoreTally.text = "× 0";
		//LoadKanji();
		SetUpRadicals();
		ShowDuJourMenu(0.0f);
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
	public void LoadLevel(string levelUuid) {
		Debug.Log("Load level called");
		if(levelId == "none") {
			Debug.Log(levelId);
			levelId = levelUuid;
			GetLoadedKanji();
			//LoadKanji();
			LevelSetup();
		}
	}

	/*public void LoadLevel(int lvlIndex) {
		if(levelIndex == -1) {
			levelIndex = lvlIndex;
			GetLoadedKanji();
			LevelSetup();
		}
	}*/

	public void RestartLevel() {
		Debug.Log("Restart level called");
		ShowDuJourMenu();
		gameStarted = false;
		requestQueue.ClearRequests();
		resultModal.CloseResultModal();
		levelTimer = 0;
		attempts = 0;
		requestTimer = 4.5f;
		scoreTally.text = "× 0";
	}

	public void OpenLevelSelect() {
		CleanUpGameplay();
	}
	
	void BuildDuJourLevel() {
		menu.parent.GetComponentInChildren<LevelSelectButton>().Initialize(levelId, false);
		foreach(EntreeData pairing in targetWords) {
			GameObject entreeListing = Instantiate<GameObject>(entreePrefab, menu);
			entreePrefab.GetComponent<EntreeBehavior>().Initialize(pairing);
		}
	}

	public void DuJourButtonAction() {
		if(gameStarted) {
			DismissDuJourMenu();
		} else {
			StartGame();
		}
	}

	void EnableDuJourMenu(){
		menu.parent.gameObject.SetActive(true);
		if(!gameStarted) {
			menu.parent.GetChild(2).GetComponentInChildren<Text>().text = "Begin";
		}
	}

	void DisableDujourMenu() {
		menu.parent.gameObject.SetActive(false);
		menu.parent.GetChild(2).GetComponentInChildren<Text>().text = "Close";
	}

	public void ShowDuJourMenu(float lerpTime = 0.4f) {
		EnableDuJourMenu();
		RectTransform menuRect = menu.transform.parent.GetComponent<RectTransform>();
		float rectSize = menuRect.rect.size.y;
		StartCoroutine(MenuManager.LerpInsetAnimation(menuRect, -rectSize, 0, lerpTime, RectTransform.Edge.Top));
	}

	void DismissDuJourMenu(float lerpTime = 0.4f) {
		if(!menu.parent.gameObject.activeSelf) return;
		RectTransform menuRect = menu.transform.parent.GetComponent<RectTransform>();
		float rectSize = menuRect.rect.size.y;
		StartCoroutine(MenuManager.LerpInsetAnimation(menuRect, 0, -rectSize, lerpTime, RectTransform.Edge.Top));
		Invoke("DisableDujourMenu", lerpTime);
	}

	public void ClearDuJourMenu() {
		menu.parent.GetComponentInChildren<LevelSelectButton>().Clear();
		for(int i = 1; i < menu.childCount; i++) {
			Destroy(menu.GetChild(i).gameObject);
		}
		Invoke("DismissDuJourMenu", 0.5f);
	}	

	void GetLoadedKanji() {
		targetWords = contentManager.GetLevelContent(levelId);
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

	void UpdateTimer() {
		timer.text = Mathf.Round(levelDuration - levelTimer).ToString();
	}
	
	void MakeNewRequest() {
		if(wordBag.Count == 0) {
			foreach(EntreeData pairing in targetWords) {
				wordBag.Add(pairing);
			}
		}
		int diceRoll = Mathf.FloorToInt(Random.Range(0, wordBag.Count));
		string request = ExtractTargetFromEntree(wordBag[diceRoll]);
		wordBag.RemoveAt(diceRoll);
		requestQueue.ReceiveRequest(request);
	}

	public void ClearRequest(Text answer, EntreeData result) {
		attempts++;
		if(requestQueue.SatisfiesRequest(ExtractTargetFromEntree(result))) {
			StartCoroutine(requestQueue.ClearRequest(answer, ExtractTargetFromEntree(result)));
			Debug.Log(scoreTally.text.Substring(1));
			scoreTally.text = "× " + (int.Parse(scoreTally.text.Substring(1)) + 1).ToString();
			if(foundWords.IndexOf(result.literal) == -1) foundWords.Add(result.literal);
		} 
	}

	void ShowResults() {
		resultModal.gameObject.SetActive(true);
		int score = int.Parse(scoreTally.text.Substring(1));
		resultModal.DisplayResults(score, attempts, foundWords.Count == targetWords.Length);
	}

	public void CleanUpGameplay() {
		gameStarted = false;
		DismissDuJourMenu();
		requestQueue.ClearRequests();
		cookingPot.ClearIngredients();
		foundWords.Clear();
		resultModal.CloseResultModal();
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
				List<string> recipeComponents = new List<string>(listing.components);
				List<string> cookedComponents = new List<string>(components);
				foreach(string ingredient in cookedComponents) {
					int ingredientIndex = recipeComponents.IndexOf(ingredient);
					if(ingredientIndex == -1) {
						match = false;
						break;
					}
					recipeComponents.RemoveAt(ingredientIndex);
				}
				if(match) {
					return listing;
				}
			}
		}
		return new EntreeData(new string[] {"failure"}, "駄目", new string[0], new string[0], new string[0]);
	}

	public string ExtractTargetFromEntree(EntreeData theEntree) {
		switch(studyType) {
			case StudyType.Meaning:
				return theEntree.meanings[0];
				break;
			case StudyType.Kunyomi:
				return theEntree.kunyomi[0];
				break;
			case StudyType.Onyomi:
				return theEntree.onyomi[0];
				break;
		}
		return theEntree.meanings[0];
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

