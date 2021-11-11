using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour, IStoreListener
{
	public static IAPManager instance;
	
	private static IStoreController storeController;
	private static IExtensionProvider storeExtensionProvider;

	private string jlpt5 = "jlpt5";

	public void InitializePurchasing() {
		if(IsInitialized()) return;
		var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

		builder.AddProduct(jlpt5, ProductType.NonConsumable); 

		UnityPurchasing.Initialize(this, builder);
	}

	private bool IsInitialized() {
		return storeController != null && storeExtensionProvider != null;
	}

	public void BuyJlpt5() {
		BuyProductID(jlpt5);
	}

	void BuyProductID(string productId) {
		if(IsInitialized()) {
			Product product = storeController.products.WithID(productId);
			if(product != null && product.availableToPurchase) {
				Debug.Log(string.Format("Purchasing product asynchronously: '{0}'", product.definition.id));

				storeController.InitiatePurchase(product);
			}
			else {
				Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or not available");
			}
		} else {
			Debug.Log("BuyProductID FAIL, Not initialized");
		}
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {
		if(String.Equals(args.purchasedProduct.definition.id, jlpt5, StringComparison.Ordinal)) {
			Debug.Log("Purchased jlpt5");
			ContentManager.instance.AddLevelPack(jlpt5);
		}
		else {
			Debug.LogError("Purchase failed");
		}
		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
		Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
		Debug.Log("Initialized IStoreController");
		
		storeController = controller;
		storeExtensionProvider = extensions;
	}

	public void OnInitializeFailed(InitializationFailureReason error) {
		Debug.Log("IStoreController initialization failed: " + error);
	}

	
    // Start is called before the first frame update
    void Start()
    {
        if(storeController == null) {
			InitializePurchasing();
		}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
