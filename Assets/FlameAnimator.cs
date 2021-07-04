using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameAnimator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {	
        transform.localScale = new Vector3(1 + Random.Range(0f, 0.2f), 1 + Random.Range(0f, 0.2f), 1);
    }

    // Update is called once per frame
    void Update()
    {
    	transform.localScale = new Vector3(1 + Random.Range(-0.2f, 0.2f), 1 + Random.Range(-0.2f, 0.2f), 1);
			
    }
}
