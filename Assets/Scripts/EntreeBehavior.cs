using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntreeBehavior : MonoBehaviour
{
	public Text formula;
	public Text character;
	public Text definition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

	public void Initialize(LanguagePair entreeData) {
		character.text = entreeData.literal;
		string formulaString = "= " + entreeData.components[0];
		for(int i = 1; i < entreeData.components.Length; i++) {
				formulaString += " + " + entreeData.components[i];
		}
		formula.text = formulaString;
		string definitionString = entreeData.literal;
		string reading = "むら";
		string romanji = "mura";
		string meaning = "village";
		string definition = "\"village\"";
		definitionString += "(" + reading + "|" + romanji + ")" + definition;
	}
}
