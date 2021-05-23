using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CookingPotBehavior : MonoBehaviour
{
	public static CookingPotBehavior instance;
	public RectTransform hitRect;
	List<char> ingredients;
	public Text resultSpot;
	Image curImage;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
		curImage = GetComponent<Image>();
		ingredients = new List<char>();
		hitRect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void AddIngredient(char character) {
		Debug.Log("adding " + character.ToString());
		ingredients.Add(character);
	}

	public void CombineIngredients() {
		if(ingredients.Count == 2) {
			if(ingredients.IndexOf('口') > -1 && ingredients.IndexOf('女') > -1) {
				resultSpot.text = "Likeness";
			}
			else if(ingredients.IndexOf('未') > -1 && ingredients.IndexOf('女') > -1) {
				resultSpot.text = "little Sis";
			}
		}
		ingredients.Clear();
	}
}
