using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BundleListingController : MonoBehaviour
{
	public string packName;
	public string packDescription;
	public string packPrice;
	public string packId;
	public Text nameLabel;
	public Text descLabel;
	public Text priceLabel;
	public Text kanjiLabel;
	public Button purchaseButton;

	public void Initialize(string name, string desc, string cost, string id) {
		packName = name;
		packDescription = desc;
		packPrice = cost;
		packId = id;
		nameLabel.text = name;
		descLabel.text = desc;
		priceLabel.text = "Buy " + cost;
		kanjiLabel.text = GetLabel(id);
		if(ContentManager.instance.AlreadyOwned(id)) {
			Debug.Log(id + " is already owned");
			purchaseButton.interactable = false;
			purchaseButton.GetComponent<DynamicColorObject>().colorAssignment = DynamicColorAssignment.Menu;
		}
	}

    public void ShowContents() {
		PurchaseScreenController.instance.ShowDetails(packName);
	}

	public void PurchasePack() {
		if (!ContentManager.instance.AlreadyOwned(packId))
		{
			IAPManager.instance.BuyProductID(packId);
		}
	}

	string GetLabel(string packId) {
		switch (packId)
		{
			case "com.bluesphere.kanjicook.jlpt5":
				return "五";
			case "com.bluesphere.kanjicook.jlpt4":
				return "四";
			case "com.bluesphere.kanjicook.jlpt3":
				return "三";
			case "com.bluesphere.kanjicook.jlpt2":
				return "二";
			default:
				return "字";
		}
	}
}
