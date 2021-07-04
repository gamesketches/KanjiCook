using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultModalController : MonoBehaviour
{
	public Image[] stars;
	public Color disabledStar;
	public Color enabledStar;
    // Start is called before the first frame update
    void Start()
    {
		foreach(Image star in stars) star.color = disabledStar;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void DisplayRating(int rating) {
		for(int i = 0; i < stars.Length; i++) {
			stars[i].color = i < rating ? enabledStar : disabledStar;
		}
	}

	public void CloseResultModal() {
		GameManager.instance.LevelSetup();
		gameObject.SetActive(false);
	}

	void OnEnable() {
		foreach(Image star in stars) star.color = disabledStar;
	}	
}
