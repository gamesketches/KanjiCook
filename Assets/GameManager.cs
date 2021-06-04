﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	RequestQueueManager requestQueue;
	public static LanguagePair[] targetWords;
	Text scoreTally;
	public TextAsset KanjiData;
	public ContentManager contentManager;
	public Text menu;
	public float requestInterval = 5;
	float requestTimer = 4.5f;
	bool gameStarted = false;

    // Start is called before the first frame update
    void Start()
    {
		instance = this;
		requestQueue = GameObject.Find("RequestQueue").GetComponent<RequestQueueManager>();
		scoreTally = GameObject.Find("Score").GetComponent<Text>();
		LoadKanji();
		SetUpRadicals();
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
		}
    }

	public void StartGame() {
		gameStarted = true;
		menu.transform.parent.gameObject.SetActive(false);
	}

	void LoadKanji() {
		targetWords = contentManager.CreateGameContent();
		string menuDisplay = "";
		foreach(LanguagePair pairing in targetWords) {
			menuDisplay += pairing.target + " = " + pairing.literal + ", made from: " + pairing.components[0];
			for(int i = 1; i < pairing.components.Length; i++) {
				menuDisplay += " + " + pairing.components[i];
			}
			menuDisplay += "\n";
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
			for(int i = 0; i < radicals.Count; i++) {
				radicalButtons[i].GetComponent<DraggableKanji>().SetRadical(radicals[i]);
			}
		}
	}

	void MakeNewRequest() {
		int diceRoll = Mathf.FloorToInt(Random.Range(0, targetWords.Length));
		string request = targetWords[diceRoll].target;
		requestQueue.ReceiveRequest(request);
	}

	public void ClearRequest(string answer) {
		if(requestQueue.SatisfyRequest(answer)) {
			scoreTally.text = (int.Parse(scoreTally.text) + 100).ToString();
		}
	}
}

public class LanguagePair {
	public string target;
	public string literal;
	public string[] components;
	
	public LanguagePair(string pairTarget, string literal, string[] pairComponents) {
		target = pairTarget;
		components = pairComponents;
	}
}

