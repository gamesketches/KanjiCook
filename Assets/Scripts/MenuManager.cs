using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

	public GameObject levelSelect;
	public GameObject titleScreen;
	public GameObject aboutScreen;

    // Start is called before the first frame update
    void Awake()
    {
        titleScreen.SetActive(true);
		levelSelect.SetActive(true);
		aboutScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OpenMainMenu() {
		DismissTitleScreen();
	}
	
	public void OpenAboutScreen() {
		aboutScreen.SetActive(true);
	}

	public void CloseAboutScreen() {
		aboutScreen.SetActive(false);
	}

	void DismissTitleScreen() {
		titleScreen.SetActive(false);
	}
}
