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
		requests = new List<RequestBehavior>(gameObject.GetComponentsInChildren<RequestBehavior>());
		foreach(RequestBehavior request in requests) { request.gameObject.SetActive(false);}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void ReceiveRequest(string displayText) {
		foreach(RequestBehavior poolRequest in requests) {
			if(!poolRequest.gameObject.activeInHierarchy) {
				poolRequest.gameObject.SetActive(true);
				poolRequest.Initialize(displayText);
				break;
			} 
		}
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
				requests[i].gameObject.SetActive(false);
				WaitorController.instance.PickUpOrder();
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
