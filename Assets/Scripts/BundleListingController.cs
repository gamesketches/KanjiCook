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

	public void Initialize(string name, string desc, string cost, string id) {
		packName = name;
		packDescription = desc;
		packPrice = cost;
		packId = id;
		nameLabel.text = name;
		descLabel.text = desc;
		priceLabel.text = cost;
	}

    public void ShowContents() {
		PurchaseScreenController.instance.ShowDetails(packName);
	}

	public void PurchasePack() {
		IAPManager.instance.BuyProductID(packId);
	}
}
