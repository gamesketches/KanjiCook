using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RequestQueueManager : MonoBehaviour
{
	public GameObject requestTemplate;
	List<RequestBehavior> requests;
	Rect referenceRect;
	float offset = 110;

    // Start is called before the first frame update
    void Start()
    {
        referenceRect = requestTemplate.GetComponent<RectTransform>().rect;
		requests = new List<RequestBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void ReceiveRequest(string displayText) {
		GameObject newRequest = Instantiate<GameObject>(requestTemplate, transform);
		RequestBehavior newRequestBehavior = newRequest.GetComponent<RequestBehavior>();
		newRequestBehavior.Initialize(displayText);
		requestTemplate.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.Range(0, offset * 2), Random.Range(0, offset));
		requests.Add(newRequestBehavior);
	}

	public bool SatisfiesRequest(string displayText) {
		Debug.Log("satisfying request");
		for(int i = 0; i < requests.Count; i++) {
			if(requests[i].RequestFulfilled(displayText)) {
				return true;
				break;
			}
		}
		//ReorganizeRequests();
		return false;
	}

	public IEnumerator ClearRequest(Text kanjiOrder) {
		for(int i = 0; i < requests.Count; i++) {
			if(requests[i].RequestFulfilled(kanjiOrder.text)) {
				Vector3 targetPos = requests[i].transform.position;
				Vector3 startPos = kanjiOrder.transform.position;
				GameObject servedKanji = Instantiate(kanjiOrder.gameObject, transform.parent);
				kanjiOrder.text = "";
				for(float t = 0; t < 1; t += Time.deltaTime) {
					servedKanji.transform.position = Vector3.Lerp(startPos, targetPos, t/1);
					yield return null;
				}
				Destroy(servedKanji);
				Destroy(requests[i].gameObject);
				Debug.Log(requests[i]);
				requests.RemoveAt(i);
				break;
			}
		}
		yield return null;
	}
	void ReorganizeRequests() {
		for(int i = 0; i < requests.Count; i++) {
			requests[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, offset * i);
		}
	}
}
