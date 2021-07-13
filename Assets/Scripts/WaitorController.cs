using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitorController : MonoBehaviour
{
	public float waitorInterval;
	bool animating;
	float offscreenTimer;
	Vector3 waitorPosition;
	public float waitorTravelTime;
	SpriteRenderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        offscreenTimer = waitorInterval;
		waitorPosition = transform.position;
		transform.position += new Vector3(10, 0, 0);
		renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
		if(!animating) {
			offscreenTimer -= Time.deltaTime;
			if(offscreenTimer <= 0) {
				animating = true;
				StartCoroutine(WaitorWalksIn());
			}
		}
    }

	IEnumerator WaitorWalksIn() {
		Vector3 offscreenPos = transform.position;
		transform.localScale = Vector3.one;
		//renderer.flipX = false;
		for(float t = 0; t < waitorTravelTime; t += Time.deltaTime) {
			transform.position = Vector3.Lerp(offscreenPos, waitorPosition, t / waitorTravelTime);
			yield return null;
		}
		yield return new WaitForSeconds(1);
		//renderer.flipX = true;
		transform.localScale = new Vector3(-1, 1, 1);
		for(float t = 0; t < waitorTravelTime; t += Time.deltaTime) {
			transform.position = Vector3.Lerp(waitorPosition, offscreenPos, t / waitorTravelTime);
			yield return null;
		}
	}
}
