using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum DynamicColorAssignment {Primary, Secondary, Menu};
public class DynamicColorObject : MonoBehaviour
{
	//Image coloredObject;
	public DynamicColorAssignment colorAssignment;

    // Start is called before the first frame update
    void Start()
    {
        var coloredObject = GetComponent<Image>();
		if(colorAssignment == DynamicColorAssignment.Primary) {
			coloredObject.color = AppManager.instance.primaryColor;
		} else if(colorAssignment == DynamicColorAssignment.Secondary) {
			coloredObject.color = AppManager.instance.secondaryColor;
		} else { 
			coloredObject.color = AppManager.instance.menuColor;
		}
    }

	void OnEnable() {
        var coloredObject = GetComponent<Image>();
		/*if(coloredObject == null) {
        	coloredObject = GetComponent<Image>();
		}*/
		if(colorAssignment == DynamicColorAssignment.Primary) {
			coloredObject.color = AppManager.instance.primaryColor;
		} else if(colorAssignment == DynamicColorAssignment.Secondary) {
			coloredObject.color = AppManager.instance.secondaryColor;
		} else { 
			coloredObject.color = AppManager.instance.menuColor;
		}
	}
}
