using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour, IPointerClickHandler
{
	string levelFile;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void Initialize(string id, string levelName) {
		levelFile = id;
		GetComponentInChildren<Text>().text = levelName;
	}

	public void OnPointerClick(PointerEventData pointerEventData) {
		GameManager.instance.LoadLevel(levelFile);
	}
}
