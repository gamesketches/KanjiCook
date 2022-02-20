using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TutorialImageController : MonoBehaviour, IPointerClickHandler
{
	public Sprite[] tutorialImages;
	Image tutorialImage;
	int numImages;
	int imageIndex;

    // Start is called before the first frame update
    void Start()
    {
        numImages = tutorialImages.Length;
		imageIndex = 0;
		tutorialImage = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData pointerEventData) {
		imageIndex++;
		if(imageIndex < tutorialImages.Length) {
			tutorialImage.sprite = tutorialImages[imageIndex];
		} else {
			MenuManager.instance.BackButtonPressed();
			StartCoroutine(ResetTutorial());
		}
	}

	IEnumerator ResetTutorial() {
		yield return new WaitForSeconds(1f);
		imageIndex = 0;
		tutorialImage.sprite = tutorialImages[imageIndex];
	}
}
