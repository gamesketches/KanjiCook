using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefController : MonoBehaviour
{
	public Transform chefArm;
	public Transform chefForearm;
	public static ChefController instance;
	public bool cooking;
	float forearmRotation;
	public float rotationInterval;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
		if(cooking) {
			float rotationAmount = Mathf.PingPong(Time.realtimeSinceStartup * 20, rotationInterval);
			chefArm.transform.rotation = Quaternion.Euler(0, 0, rotationAmount); 
			chefForearm.transform.rotation = Quaternion.Euler(0, 0, -rotationAmount);
		} else {
			chefArm.transform.rotation = Quaternion.identity;
			chefForearm.transform.rotation = Quaternion.identity;
		}
    }
}
