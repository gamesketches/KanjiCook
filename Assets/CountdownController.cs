using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CountdownController : MonoBehaviour
{
	public AudioClip countdownNumber;
	public AudioClip countdownGo;
	AudioSource audioSource;
	Text countdownText;

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
		countdownText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void UpdateClockNumber(int newNumber) {
		if(!countdownText.enabled) countdownText.enabled = true;
		countdownText.text = newNumber.ToString();
		audioSource.PlayOneShot(countdownNumber);
	}

	public void UpdateClockGo() {
		countdownText.text = "Go!";
		audioSource.PlayOneShot(countdownGo);
	}

	public void ClearCountdown() {
		countdownText.text = "";
		countdownText.enabled = false;
	}
}
