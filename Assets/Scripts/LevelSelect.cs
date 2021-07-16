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
		int levelCount = 1;
     	foreach(TextAsset asset in Resources.LoadAll("", typeof(TextAsset))) {
			GameObject levelButton = Instantiate(levelButtonPrefab);
			levelButton.transform.parent = transform;
			levelButton.GetComponentInChildren<Text>().text = asset.name;
			levelButton.GetComponentInChildren<LevelSelectButton>().Initialize(asset.name, levelCount.ToString());
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
