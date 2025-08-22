using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using TMPro;

public class ShopManager : MonoBehaviour
{
    private ListOwnCharacter ListOwnCharacter;
    private GameCoin gameCoin;

    public GameObject panelPrefab;
    public Transform contentParent;

    public Button buyButton;
    public TextMeshProUGUI priceText;

    public GameObject confirmationPanel;
    public Button confirmBuyButton;
    public GameObject invoice;

    public Transform characterPreviewParent;

    private List<ItemInstance> userInventory;
    private CatalogItem selectedItem;

    void Start()
    {
        ListOwnCharacter = FindObjectOfType<ListOwnCharacter>();
        gameCoin = FindObjectOfType<GameCoin>();
        buyButton.gameObject.SetActive(false);
        confirmationPanel.SetActive(false);

        if (characterPreviewParent != null)
        {
            foreach (Transform child in characterPreviewParent)
            {
                child.gameObject.SetActive(false);
            }
        }

        GetCatalogAndInventory();
    }

    private void GetCatalogAndInventory()
    {
        PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(),
            result =>
            {
                userInventory = result.Inventory;
                GetPlayFabCatalog();
            },
            error => Debug.LogError("Gagal mendapatkan inventaris: " + error.GenerateErrorReport()));
    }

    private void GetPlayFabCatalog()
    {
        var request = new GetCatalogItemsRequest { CatalogVersion = "Character" };
        PlayFabClientAPI.GetCatalogItems(request,
            result => PopulateShopPanels(result.Catalog),
            error => Debug.LogError("Gagal mendapatkan data katalog: " + error.GenerateErrorReport()));
    }

    private void PopulateShopPanels(List<CatalogItem> catalogItems)
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in catalogItems)
        {
            GameObject newPanel = Instantiate(panelPrefab, contentParent);

            Button selectButton = newPanel.GetComponent<Button>();
            GameObject ImageCoin = newPanel.transform.Find("ImageCoin").gameObject;
            TextMeshProUGUI priceText = ImageCoin.transform.Find("TextCoin").GetComponent<TextMeshProUGUI>();
            GameObject gembokObject = newPanel.transform.Find("Gembok").gameObject;
            

            bool isOwned = userInventory.Exists(ownedItem => ownedItem.ItemId == item.ItemId);

            gembokObject.SetActive(!isOwned);

            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => SelectCharacter(item));

            string characterPath = $"Character/{item.ItemId}";
            Sprite characterSprite = Resources.Load<Sprite>(characterPath);
            newPanel.transform.Find("CH").GetComponent<Image>().sprite = characterSprite;
            priceText.text = GetItemPrice(item).ToString();
            if (isOwned)
            {
                priceText.SetText("Owned");
            }
        }
    }

    public void SelectCharacter(CatalogItem item)
    {
        selectedItem = item;
        bool isOwned = userInventory.Exists(ownedItem => ownedItem.ItemId == selectedItem.ItemId);

        if (characterPreviewParent != null)
        {
            foreach (Transform child in characterPreviewParent)
            {
                child.gameObject.SetActive(child.name == selectedItem.ItemId);
            }
        }

        buyButton.gameObject.SetActive(!isOwned);
        priceText.text = isOwned ? "Owned" : GetItemPrice(selectedItem).ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(ShowConfirmationPanel);
    }

    public void ShowConfirmationPanel()
    {
        if (selectedItem == null) return;

        confirmationPanel.SetActive(true);

        confirmBuyButton.onClick.RemoveAllListeners();
        confirmBuyButton.onClick.AddListener(BuySelectedItem);
    }

    private void BuySelectedItem()
    {
        if (selectedItem == null) return;
        string currency = "";
        uint price = 0;
        foreach (var p in selectedItem.VirtualCurrencyPrices)
        {
            currency = p.Key;
            price = p.Value;
        }

        var request = new PurchaseItemRequest
        {
            ItemId = selectedItem.ItemId,
            VirtualCurrency = currency,
            Price = (int)price,
            CatalogVersion = "Character"
        };

        RawImage invoiceResult = invoice.transform.Find("DialogPay").GetComponent<RawImage>();
        PlayFabClientAPI.PurchaseItem(request,
            result =>
            {
                Debug.Log($"Berhasil membeli item: {selectedItem.DisplayName}");
                confirmationPanel.SetActive(false);
                buyButton.gameObject.SetActive(false);

                GetCatalogAndInventory();

                if (invoiceResult != null)
                {
                    string invoicePath = $"UI/success";
                    Texture2D invoiceTexture = Resources.Load<Texture2D>(invoicePath);
                    invoiceResult.texture = invoiceTexture;
                }
                invoice.SetActive(true);
                if (ListOwnCharacter != null && gameCoin != null)
                {
                    Debug.Log("ada kok");
                    ListOwnCharacter.GetUserInventoryItems();
                    gameCoin.GetCoinBalance(); 
                }
                else
                {
                    Debug.Log("Tidak ada kok");
                }
            },
            error =>
            {
                if (invoiceResult != null)
                {
                    string invoicePath = $"UI/failed";
                    Texture2D invoiceTexture = Resources.Load<Texture2D>(invoicePath);
                    invoiceResult.texture = invoiceTexture;
                }
                invoice.SetActive(true);
            });
    }

    private uint GetItemPrice(CatalogItem item)
    {
        foreach (var price in item.VirtualCurrencyPrices)
        {
            return price.Value;
        }
        return 0;
    }
}