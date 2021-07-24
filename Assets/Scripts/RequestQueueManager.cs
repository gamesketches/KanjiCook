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

	public IEnumerator ClearRequest(Text kanjiOrder, string targetText) {
		for(int i = 0; i < requests.Count; i++) {
			if(requests[i].RequestFulfilled(targetText)) {
				float travelTime = 0.7f;
				Vector3 targetPos = requests[i].transform.position;
				Vector3 startPos = kanjiOrder.transform.position;
				GameObject servedKanji = Instantiate(kanjiOrder.gameObject, transform.parent);
				servedKanji.transform.position = kanjiOrder.transform.position;
				float stretchAnimTime = 0.3f;
				for(float t = 0; t < stretchAnimTime; t+= Time.deltaTime) {
					servedKanji.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(1.7f, 1.7f, 1.7f), Mathf.PingPong(t, stretchAnimTime / 2) / stretchAnimTime / 2);
					yield return null;
				}
				kanjiOrder.text = "";
				for(float t = 0; t < travelTime; t += Time.deltaTime) {
					servedKanji.transform.position = Vector3.Lerp(startPos, targetPos, t / travelTime);
					yield return null;
				}
				Destroy(servedKanji);
				requests[i].PlayFulfilledAnimation();
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

	public void ClearRequests() {
		foreach(RequestBehavior poolRequest in requests) {
			poolRequest.gameObject.SetActive(false);
		}
	}
}
