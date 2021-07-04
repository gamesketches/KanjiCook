using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
	public GameObject levelButtonPrefab;
    // Start is called before the first frame update
    void Start()
    {
		int levelCount = 0;
     	foreach(TextAsset asset in Resources.LoadAll<TextAsset>("")) {
			GameObject levelButton = Instantiate(levelButtonPrefab);
			levelButton.transform.parent = transform;
			//levelButton.GetComponent<Image>().onClick.AddListener(() => LevelLoadCallback(levelCount));
			levelButton.GetComponent<LevelSelectButton>().Initialize(levelCount, levelCount.ToString());
			levelCount++;
		} 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	void LevelLoadCallback(int level) {
		Debug.Log("loading level " + level.ToString());
	}
}
