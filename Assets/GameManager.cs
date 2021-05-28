using System.Collections;
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

	public float requestInterval = 5;
	float requestTimer = 4.5f;

    // Start is called before the first frame update
    void Start()
    {
		instance = this;
		requestQueue = GameObject.Find("RequestQueue").GetComponent<RequestQueueManager>();
		LoadKanji();
		SetUpRadicals();
		scoreTally = GameObject.Find("Score").GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        requestTimer += Time.deltaTime;
		if(requestTimer >= requestInterval) {
			MakeNewRequest();
			requestTimer = 0;
		}
    }

	void LoadKanji() {
		string[] kanjiLines = KanjiData.text.Split('\n');
		List<LanguagePair> deKanjis = new List<LanguagePair>();
		foreach(string kanjiListing in kanjiLines) {
			string[] components = kanjiListing.Substring(4).Split(' ');
			deKanjis.Add(new LanguagePair(kanjiListing[0].ToString(), components));
		} 
		targetWords = deKanjis.ToArray();
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
	public string[] components;
	
	public LanguagePair(string pairTarget, string[] pairComponents) {
		target = pairTarget;
		components = pairComponents;
	}
}
