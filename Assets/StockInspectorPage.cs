using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace MyLittleStore
{
    public class StockInspectorPage : MonoBehaviour
    {
        [Header("References")]
        public Transform Container;
        public GameObject Toolbar;

        [Header("Buttons")]
        public Button CloseButton;
        public Button BuyButton;
        public Button DoneButton;
        public Button IncreaseModeButton;
        public Button DecreaseModeButton;
        public Button MultiplierButton;

        [Header("Settings")]
        public Color SelectedButtonColor = Color.green;
        public GameObject ProductButtonPrefab;

        private bool ChoiceMode = false;
        private bool BuyMode = false;
        private bool IncreaseMode = true;
        private int Multiplier = 1;

        private List<Product> Cart = new();

        public delegate void ProductClicked(string ProductName);
        public ProductClicked ProductChosen;


        private void Start()
        {
            CloseButton.onClick.AddListener(Close);

            BuyButton.onClick.AddListener(() => ToggleBuyMode(true));
            DoneButton.onClick.AddListener(() => ToggleBuyMode(false));

            IncreaseModeButton.onClick.AddListener(() => ToggleIncreaseMode(true));
            DecreaseModeButton.onClick.AddListener(() => ToggleIncreaseMode(false));

            MultiplierButton.onClick.AddListener(ToggleMultiplier);
        }

        private void RefreshContent()
        {
            foreach (Transform Child in Container) Destroy(Child.gameObject);

            GameObject CreateButton(ProductAsset Asset, out TMP_Text AmountLabel)
            {
                GameObject Button = Instantiate(ProductButtonPrefab, Container);

                TMP_Text NameLabel = Button.transform.Find("Name").GetComponent<TMP_Text>();
                Image Icon = Button.transform.Find("Background/Top/Icon").GetComponent<Image>();

                AmountLabel = Button.transform.Find("Amount").GetComponent<TMP_Text>();

                Button.name = Asset.Name;
                NameLabel.text = Asset.Name;
                Icon.sprite = Asset.Icon;

                return Button;
            }

            if (BuyMode)
            {
                foreach (ProductAsset Asset in GameController.Singleton.AllProducts)
                {
                    GameObject Button = CreateButton(Asset, out TMP_Text AmountLabel);

                    Utility.UI.IncreaseColors(Button, Color.white / 10);

                    AmountLabel.text = "+";

                    Button.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        Utility.UI.SetColors(Button, SelectedButtonColor);

                        int AmountInCart = IncreaseMode ? Utility.Products.AddToList(Cart, Asset.Name, Multiplier) : Utility.Products.RemoveFromList(Cart, Asset.Name, Multiplier);

                        AmountLabel.text = AmountInCart == 0 ? "+" : AmountInCart.ToString() + "x";
                    });
                }
            }
            else
            {
                foreach (Product InStockProduct in GameController.Singleton.Stock)
                {
                    ProductAsset Asset = Utility.Products.GetAsset(InStockProduct.Name);
                    GameObject Button = CreateButton(Asset, out TMP_Text AmountLabel);

                    AmountLabel.text = "x" + InStockProduct.Amount;
                    InStockProduct.AmountChanged += () => AmountLabel.text = "x" + InStockProduct.Amount;

                    if (ChoiceMode)
                    {
                        Button.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            Utility.UI.SetColors(Button, SelectedButtonColor);

                            ProductChosen(InStockProduct.Name);

                            Close();
                        });
                    }
                }
            }
        }

        public void Open(bool WithChoiceMode = false)
        {
            UIController.ToggleHotbar(false);
            gameObject.SetActive(true);

            ChoiceMode = WithChoiceMode;
            Toolbar.SetActive(!WithChoiceMode);

            RefreshContent();
        }

        public void Close()
        {
            gameObject.SetActive(false);
            UIController.ToggleHotbar(true);
        }



        public void ToggleBuyMode(bool State)
        {
            if (BuyMode && !State) // if the Player clicks on Done
            {
                float Check = 0;

                foreach (var Product in Cart)
                {
                    ProductAsset Asset = Utility.Products.GetAsset(Product.Name);
                    Check += Asset.Price * Product.Amount;
                }

                if (GameController.Singleton.Money > Check)
                {
                    foreach (var BoughtProduct in Cart) Utility.Products.AddToList(GameController.Singleton.Stock, BoughtProduct.Name, BoughtProduct.Amount);

                    GameController.Singleton.Money -= Check;
                }

                Cart.Clear();
            }

            BuyMode = State;
            IncreaseMode = true;

            BuyButton.gameObject.SetActive(!State);
            DoneButton.gameObject.SetActive(State);

            IncreaseModeButton.gameObject.SetActive(false);
            DecreaseModeButton.gameObject.SetActive(State);

            MultiplierButton.gameObject.SetActive(State);

            RefreshContent();
        }

        public void ToggleIncreaseMode(bool Increase)
        {
            if (!BuyMode) return;

            print(Increase);

            IncreaseModeButton.gameObject.SetActive(!Increase);
            DecreaseModeButton.gameObject.SetActive(Increase);

            IncreaseMode = Increase;
        }

        public void ToggleMultiplier()
        {
            List<int> Multipliers = new() { 1, 10, 25, 50, 100, 1000 };
            int Current = Multipliers.IndexOf(Multiplier);

            Current++;

            if (Current >= Multipliers.Count) Current = 0;

            Multiplier = Multipliers[Current];

            MultiplierButton.GetComponentInChildren<TMP_Text>().text = "x" + Multiplier.ToString();
        }
    }
}
