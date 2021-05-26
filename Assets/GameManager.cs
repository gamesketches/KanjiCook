using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	RequestQueueManager requestQueue;
	LanguagePair[] targetWords;
	Text scoreTally;

	public float requestInterval = 5;
	float requestTimer = 4.5f;

    // Start is called before the first frame update
    void Start()
    {
		instance = this;
        targetWords = new LanguagePair[2];
		requestQueue = GameObject.Find("RequestQueue").GetComponent<RequestQueueManager>();
		targetWords[0] = new LanguagePair("Likeness", new string[] {"口", "女"});
		targetWords[1] = new LanguagePair("Little Sis", new string[] { "未", "女"});
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

class LanguagePair {
	public string target;
	public string[] components;
	
	public LanguagePair(string pairTarget, string[] pairComponents) {
		target = pairTarget;
		components = pairComponents;
	}
}
