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

	private string jlpt5 = "com.bluesphere.kanjicook.jlpt5";
	private string jlpt4 = "com.bluesphere.kanjicook.jlpt4";
	private string jlpt3 = "com.bluesphere.kanjicook.jlpt3";
	private string jlpt2 = "com.bluesphere.kanjicook.jlpt2";

    // Start is called before the first frame update
    void Start()
    {
        if(storeController == null) {
			InitializePurchasing();
		}
		instance = this;
    }

	public void InitializePurchasing() {
		if(IsInitialized()) return;
		var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

		builder.AddProduct(jlpt5, ProductType.NonConsumable); 
		builder.AddProduct(jlpt4, ProductType.NonConsumable);
		builder.AddProduct(jlpt3, ProductType.NonConsumable); 
		builder.AddProduct(jlpt2, ProductType.NonConsumable);

		UnityPurchasing.Initialize(this, builder);
	}

	private bool IsInitialized() {
		return storeController != null && storeExtensionProvider != null;
	}

	public void BuyJlpt5() {
		BuyProductID(jlpt5);
	}

	public void BuyProductID(string productId) {
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
			Debug.Log("Adding code " + jlpt5);
			ContentManager.instance.AddLevelPack(jlpt5);
		}
		else if(String.Equals(args.purchasedProduct.definition.id, jlpt4, StringComparison.Ordinal)) {
			Debug.Log("Purchased jlpt4");
			Debug.Log("Adding code " + jlpt4);
			ContentManager.instance.AddLevelPack(jlpt4);
		}
		else if(String.Equals(args.purchasedProduct.definition.id, jlpt3, StringComparison.Ordinal)) {
			Debug.Log("Purchased jlpt3");
			Debug.Log("Adding code " + jlpt3);
			ContentManager.instance.AddLevelPack(jlpt3);
		}
		else if(String.Equals(args.purchasedProduct.definition.id, jlpt2, StringComparison.Ordinal)) {
			Debug.Log("Purchased jlpt2");
			Debug.Log("Adding code " + jlpt2);
			ContentManager.instance.AddLevelPack(jlpt2);
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

	public void RestorePurchases() {
		if(!IsInitialized()) {
			Debug.Log("Restore Purchases Failed. Purchaser not initialized");
		}

		if(Application.platform == RuntimePlatform.IPhonePlayer) {
			Debug.Log("Restore Purchases Started");

			var apple = storeExtensionProvider.GetExtension<IAppleExtensions>();

			apple.RestoreTransactions((result) => {
				Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
			});
		}
	}

	public void OnInitializeFailed(InitializationFailureReason error) {
		Debug.Log("IStoreController initialization failed: " + error);
	}
}
