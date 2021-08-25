using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

	public GameObject levelSelect;
	public GameObject titleScreen;
	public GameObject aboutScreen;
	public GameObject packStore;

    // Start is called before the first frame update
    void Awake()
    {
        titleScreen.SetActive(true);
		levelSelect.SetActive(true);
		aboutScreen.SetActive(false);
		packStore.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void OpenMainMenu() {
		DismissTitleScreen();
	}
	
	public void ToggleAboutScreen() {
		if(!aboutScreen.activeSelf) { 
			aboutScreen.SetActive(true);
		} else {
			aboutScreen.SetActive(false);
		}
	}

	public void TogglePackStore() {
		if(!packStore.activeSelf) {
			packStore.SetActive(true);
			packStore.GetComponent<PurchaseScreenController>().OpenPurchaseMenu();
		} else {
			packStore.GetComponent<PurchaseScreenController>().ClosePurchaseMenu();
		}
	}

	void DismissTitleScreen() {
		titleScreen.SetActive(false);
	}
}
