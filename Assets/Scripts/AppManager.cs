using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppManager : MonoBehaviour
{
	public static AppManager instance;
	public GameObject GameMenuCanvas;
	public Canvas gameplayCanvas;
	public Canvas gameplayUICanvas;
	public Color primaryColor;
	public Color secondaryColor;
	
    // Start is called before the first frame update
    void Awake()
    {
		AppManager.instance = this;
		GameMenuCanvas.SetActive(true);
		DisableGameplayCanvases();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void SelectLevel(string levelId) {
		if(!LevelSelect.levelSelectLocked) return;
		EnableGameplayCanvases();
		GameMenuCanvas.GetComponent<MenuManager>().SlideOffMenus();
		GameManager.instance.LoadLevel(levelId);
	}
		
	public void OpenLevelSelect() {
		GameMenuCanvas.GetComponent<MenuManager>().SlideOnMenus();
		GameManager.instance.CleanUpGameplay();
		GameManager.instance.ClearDuJourMenu();
		GameManager.levelId = "none";
		Invoke("DisableGameplayCanvases", MenuManager.menuSlideSpeed);
	}

	void EnableGameplayCanvases() {
		gameplayCanvas.enabled = true;
		gameplayUICanvas.enabled = true;
	}
		
	void DisableGameplayCanvases() {
		gameplayCanvas.enabled = false;
		gameplayUICanvas.enabled = false;
	}

	public void OpenKanjiDicLink() {
		Application.OpenURL("https://www.edrdg.org/wiki/index.php/KANJIDIC_Project");
	}

	public void OpenKradLink() {
		Application.OpenURL("https://www.edrdg.org/krad/kradinf.html");
	}

	public void OpenEDRDGLink() {
		Application.OpenURL("http://www.edrdg.org/");
	}

	public void OpenLicenseLink() {
		Application.OpenURL("https://www.edrdg.org/edrdg/licence.html");
	}
}
