using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour, IPointerClickHandler
{
	int levelId;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void Initialize(int id, string levelName) {
		levelId = id;
		GetComponentInChildren<Text>().text = levelName;
	}

	public void OnPointerClick(PointerEventData pointerEventData) {
		Debug.Log("clicked " + levelId.ToString());
	}
}
