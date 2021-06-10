using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		RectTransform rt = requestTemplate.GetComponent<RectTransform>();
		requests.Add(newRequestBehavior);
		ReorganizeRequests();
	}

	public bool SatisfyRequest(string displayText) {
		Debug.Log("satisfying request");
		bool requestFound = false;
		for(int i = 0; i < requests.Count; i++) {
			if(requests[i].RequestFulfilled(displayText)) {
				Destroy(requests[i].gameObject);
				Debug.Log(requests[i]);
				requests.RemoveAt(i);
				requestFound = true;
				break;
			}
		}
		ReorganizeRequests();
		return requestFound;
	}

	void ReorganizeRequests() {
		for(int i = 0; i < requests.Count; i++) {
			requests[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, offset * i);
		}
	}
}
