using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
	public static AppManager instance;
	public GameObject GameMenuCanvas;
	
    // Start is called before the first frame update
    void Awake()
    {
		AppManager.instance = this;
		GameMenuCanvas.SetActive(true);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void SelectLevel(int levelIndex) {
		if(!LevelSelect.levelSelectLocked) return;
		GameMenuCanvas.GetComponent<MenuManager>().SlideOffMenus();
		GameManager.instance.LoadLevel(levelIndex);
	}
		
	public void OpenLevelSelect() {
		GameMenuCanvas.GetComponent<MenuManager>().SlideOnMenus();
		GameManager.instance.CleanUpGameplay();
	}
}
