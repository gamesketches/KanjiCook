using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CookingPotBehavior : MonoBehaviour
{
	public static CookingPotBehavior instance;
	public RectTransform hitRect;
	public GameObject characterPrefab;
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
		ingredients.Add(character);
		GameObject newChar = Instantiate<GameObject>(characterPrefab, transform);
		newChar.GetComponent<RectTransform>().anchoredPosition = new Vector2(Random.value * hitRect.rect.width / 2.5f, Random.value * hitRect.rect.height / 2.5f);
		newChar.transform.rotation = Quaternion.Euler(0, 0, Random.value * 360);
		newChar.GetComponent<Text>().text = character.ToString();
	}

	public void CombineIngredients() {
		if(ingredients.Count == 2) {
			if(ingredients.IndexOf('口') > -1 && ingredients.IndexOf('女') > -1) {
				resultSpot.text = "Likeness";
			}
			else if(ingredients.IndexOf('未') > -1 && ingredients.IndexOf('女') > -1) {
				resultSpot.text = "Little Sis";
			}
		}
		ingredients.Clear();
		GameManager.instance.ClearRequest(resultSpot.text);
		foreach(Transform t in transform) {
			Destroy(t.gameObject);
		}
	}
}
