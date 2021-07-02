using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefController : MonoBehaviour
{
	public Transform chefArm;
	public float rotationInterval;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      chefArm.transform.rotation = Quaternion.Euler(0, 0, Mathf.PingPong(Time.realtimeSinceStartup * 10, rotationInterval)); 
    }
}
