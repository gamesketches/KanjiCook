using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntreeBehavior : MonoBehaviour
{
	public Text formula;
	public Text character;
	public Text definition;
	public int literalFontSize;

    // Start is called before the first frame update
    void Start()
    {
        
    }

	public void Initialize(EntreeData entreeData) {
		character.text = entreeData.literal;
		//character.color = AppManager.instance.secondaryColor;
		//character.fontSize = literalFontSize;
		string formulaString = entreeData.components[0];
		for(int i = 1; i < entreeData.components.Length; i++) {
				formulaString += " + " + entreeData.components[i];
		}
		formula.text = formulaString;
		string definitionString = entreeData.literal;
		string reading = GenerateReadingString(entreeData.kunyomi, entreeData.onyomi);
		string definitionList = GenerateDefinitionString(entreeData.meanings);
		definitionString += reading + definitionList;
		definition.text = definitionList;
	}

	string GenerateReadingString(string[] kunyomi, string[] onyomi) {
		string hiraganaReading = "";
		if(kunyomi.Length > 0) hiraganaReading = kunyomi[0];
		else if(onyomi.Length > 0) hiraganaReading = onyomi[0];
		//return " (" + hiraganaReading + "|" + GetRomanji(hiraganaReading) + ") ";
		return " (" + hiraganaReading + "|" + GetRomanji(hiraganaReading) + ") ";
	}

	string GenerateDefinitionString(string[] meanings) {
		string returnString = meanings[0];
		for(int i = 1; i < meanings.Length; i++) {
			returnString += ", " + meanings[i];
		}
		return returnString;
	}

	string GetRomanji(string hiraganaReading) {
		return "mura";
	}
}
