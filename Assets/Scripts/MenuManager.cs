using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
	public static MenuManager instance;
	public GameObject levelSelect;
	public GameObject titleScreen;
	public GameObject concierge;
	public GameObject aboutScreen;
	public GameObject packStore;
	public GameObject purchaseButton;
	public GameObject aboutButton;
	public GameObject backButton;
	public GameObject startButton;
	public GameObject tutorialImage;
	public AudioClip[] pageTurnSounds;
	AudioSource audioSource;
	bool aboutOpen = false;
	bool packsOpen = false;
	bool packsOnTop = false;
	bool tutorialOut = false;
	public float titleScreenSlideInTime;
	public static float menuSlideSpeed = 0.4f;

    // Start is called before the first frame update
    void Awake()
    {
		instance = this;
		audioSource = GetComponent<AudioSource>();
		//UpdateTitleBackingPosition();
        titleScreen.SetActive(true);
		packStore.transform.parent.gameObject.SetActive(true);
		LevelSelect.lerpProportion = 1;
		levelSelect.SetActive(true);
		backButton.SetActive(false);
		StartCoroutine(SlideInTitleScreen());
		DisableAboutScreen();
		DisablePackScreen();
    }

	IEnumerator SlideInTitleScreen() {
		Debug.Log("Slidin in");
		for(float t = 0; t < titleScreenSlideInTime; t += Time.deltaTime) {
			LevelSelect.lerpProportion = Mathf.SmoothStep(1, 0, t / titleScreenSlideInTime);
			yield return null;
		}
	}

	public void SlideUpTitleScreen() {
		Debug.Log("Opening levelSelect");
		RectTransform titleTransform = titleScreen.transform.GetComponent<RectTransform>();
		StartCoroutine(MenuManager.LerpInsetAnimation(titleTransform, 0, -titleTransform.rect.size.y, menuSlideSpeed, RectTransform.Edge.Top));
		startButton.GetComponent<Image>().CrossFadeAlpha(0f, menuSlideSpeed, false);
		startButton.GetComponentInChildren<Text>().CrossFadeAlpha(0f, menuSlideSpeed, false);
		Invoke("ToggleStartButton", menuSlideSpeed);
		StartCoroutine(levelSelect.GetComponent<LevelSelect>().OpenLevelSelectNoAnimation(menuSlideSpeed));
		startButton.GetComponent<Button>().interactable = false;
	}

	public void ReturnToTitleScreen() {
		RectTransform titleTransform = titleScreen.transform.GetComponent<RectTransform>();
		StartCoroutine(MenuManager.LerpInsetAnimation(titleTransform, -titleTransform.rect.size.y, 0, menuSlideSpeed, RectTransform.Edge.Top));
		startButton.GetComponent<Image>().CrossFadeAlpha(1f, menuSlideSpeed, false);
		startButton.GetComponentInChildren<Text>().CrossFadeAlpha(1f, menuSlideSpeed, false);
		startButton.GetComponent<Button>().interactable = true;
		Invoke("ToggleStartButton", menuSlideSpeed);
	}

	public void OpenMainMenu() {
		DismissTitleScreen();
	}

	public void OpenLevelSelect() {
		EnableLevelSelectScreen();
		if(!LevelSelect.levelSelectLocked) {
			StartCoroutine(levelSelect.GetComponent<LevelSelect>().FinishOpeningMenu());
		}
		backButton.SetActive(true);
	}
	
	public void ToggleAboutScreen() {
		RectTransform aboutRect = aboutScreen.GetComponent<RectTransform>();
		Canvas aboutCanvas = aboutScreen.transform.parent.GetComponent<Canvas>();
		Canvas packCanvas = packStore.transform.parent.GetComponent<Canvas>();
		float rectSize = aboutRect.rect.size.y;
		if(!aboutOpen || 
				(packsOpen && aboutOpen && packsOnTop)) { 
			aboutCanvas.enabled = true;
			packCanvas.sortingOrder = 4;
			aboutCanvas.sortingOrder = 5;
			StartCoroutine(MenuManager.LerpInsetAnimation(aboutRect, -rectSize, 0, menuSlideSpeed, RectTransform.Edge.Bottom));
			aboutOpen = true;
			packsOnTop = false;
		} else {
			StartCoroutine(MenuManager.LerpInsetAnimation(aboutRect, 0, -rectSize, menuSlideSpeed, RectTransform.Edge.Bottom));
			aboutOpen = false;
			Invoke("DisableAboutScreen", menuSlideSpeed);
			if(packsOpen) packsOnTop = true;
		}
		UpdateBackButtonActive();
	}

	public void TogglePackStore() {
		PurchaseScreenController packStoreController = packStore.GetComponent<PurchaseScreenController>();
		Canvas aboutCanvas = aboutScreen.transform.parent.GetComponent<Canvas>();
		Canvas packCanvas = packStore.transform.parent.GetComponent<Canvas>();
		if(!packsOpen || 
				(aboutOpen && packsOpen && !packsOnTop)) {
			packCanvas.enabled = true;
			aboutCanvas.sortingOrder = 4;
			packCanvas.sortingOrder = 5;
			packStore.GetComponent<PurchaseScreenController>().OpenPurchaseMenu();
			packsOpen = true;
			packsOnTop = true;
		} else {
			packStore.GetComponent<PurchaseScreenController>().ClosePurchaseMenu();
			packsOpen = false;
			packsOnTop = false;
			Invoke("DisablePackScreen", menuSlideSpeed);
		}
		PlayPageTurnSound();
		UpdateBackButtonActive();
	}

	public void ToggleTutorialImage() { 
		RectTransform tutorialTransform = tutorialImage.GetComponent<RectTransform>();
		float rectSize = tutorialTransform.rect.size.x;
		if(!tutorialOut) {
			StartCoroutine(LerpInsetAnimation(tutorialTransform, -rectSize, 0, menuSlideSpeed, RectTransform.Edge.Left));
			}
		else { 
			StartCoroutine(LerpInsetAnimation(tutorialTransform, 0, -rectSize, menuSlideSpeed, RectTransform.Edge.Left));
			}
		tutorialOut = !tutorialOut;
	}

	public void DismissTitleScreen() {
		titleScreen.transform.parent.GetComponent<Canvas>().enabled = false;
	}

	public void DisableAboutScreen() {
		aboutScreen.transform.parent.GetComponent<Canvas>().enabled = false;
	}

	public void DisablePackScreen() {
		packStore.transform.parent.GetComponent<Canvas>().enabled = false;
	}

	public void EnableLevelSelectScreen() {
		levelSelect.transform.parent.GetComponent<Canvas>().enabled = true;
	}
		
	public void DisableLevelSelectScreen() {
		levelSelect.transform.parent.GetComponent<Canvas>().enabled = false;
	}

	public void SlideOffMenus() {
		DismissTitleScreen();
		purchaseButton.SetActive(false);
		aboutButton.SetActive(false);
		backButton.SetActive(false);
		RectTransform levelSelectRect = levelSelect.GetComponent<RectTransform>();
		float levelSelectSize = levelSelectRect.rect.size.x;
		StartCoroutine(MenuManager.LerpInsetAnimation(levelSelectRect, 0, -levelSelectSize, menuSlideSpeed, RectTransform.Edge.Left));
		Invoke("DisableLevelSelectScreen", menuSlideSpeed);
		PlayPageTurnSound();
	}

	public void SlideOnMenus() {
		purchaseButton.SetActive(true);
		aboutButton.SetActive(true);
		backButton.SetActive(true);
		EnableLevelSelectScreen();
		RectTransform levelSelectRect = levelSelect.GetComponent<RectTransform>();
		float levelSelectSize = levelSelectRect.rect.size.x;
		StartCoroutine(MenuManager.LerpInsetAnimation(levelSelectRect, -levelSelectSize, 0, menuSlideSpeed, RectTransform.Edge.Left));
		PlayPageTurnSound();
		UpdateBackButtonActive();
	}

	public void UpdateBackButtonActive() {
		if(packsOpen || aboutOpen || LevelSelect.levelSelectLocked) {
			backButton.SetActive(true);
		} else {
			backButton.SetActive(false);
		}
	}

	public void BackButtonPressed() {
		if(audioSource.isPlaying) return;
		if(tutorialOut) {
			ToggleTutorialImage();
		}
		else if(packsOnTop) {
			PurchaseScreenController packsController = packStore.GetComponent<PurchaseScreenController>();
			packsController.GoBack();
			PlayPageTurnSound();
			if(!packsController.open) {
				packsOnTop = false;
				packsOpen = false;
			}
		} else if(aboutOpen) {
			ToggleAboutScreen();
		} else if(LevelSelect.levelSelectLocked) {
			Debug.Log("closingMenu");
			titleScreen.transform.parent.GetComponent<Canvas>().enabled = true;
			levelSelect.GetComponent<LevelSelect>().CloseMenuNoAnimation();
			PlayPageTurnSound();
		}
	}

	public void ToggleStartButton() {
		if(startButton.activeSelf) {
			startButton.SetActive(false);
		} else {
			startButton.SetActive(true);
		}
	}

	public void PlayPageTurnSound() {
		int diceRoll = Mathf.FloorToInt(Random.value * pageTurnSounds.Length);
		audioSource.PlayOneShot(pageTurnSounds[diceRoll]);
	}


	public static IEnumerator LerpInsetAnimation(RectTransform theRect, float startOffset, float targetOffset, float time, RectTransform.Edge parentEdge = RectTransform.Edge.Right) {
		float rectSize = theRect.rect.size.x;
		if(parentEdge == RectTransform.Edge.Top || parentEdge == RectTransform.Edge.Bottom)
			rectSize = theRect.rect.size.y;
		for(float t = 0; t < time; t += Time.deltaTime) {
			float newInset = Mathf.SmoothStep(startOffset, targetOffset, t / time);
			theRect.SetInsetAndSizeFromParentEdge(parentEdge, newInset, rectSize);
			yield return null;
		}
		theRect.SetInsetAndSizeFromParentEdge(parentEdge, targetOffset, rectSize);
	}
}
