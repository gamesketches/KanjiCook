using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BundleListingController : MonoBehaviour
{
	string packName;
	string packDescription;
	string packPrice;
	public Text nameLabel;
	public Text descLabel;
	public Text priceLabel;
    // Start is called before the first frame update
    void Start()
    {
		
    }

	public void Initialize(string name, string desc, string cost) {
		packName = name;
		packDescription = desc;
		packPrice = cost;
		nameLabel.text = name;
		descLabel.text = desc;
		priceLabel.text = cost;
	}

    public void ShowContents() {
		PurchaseScreenController.instance.ShowDetails(packName);
	}
}
