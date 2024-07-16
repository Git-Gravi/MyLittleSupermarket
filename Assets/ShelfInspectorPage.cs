using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MyLittleStore
{
    public class ShelfInspectorPage : MonoBehaviour
    {
        [Header("References")]
        public TMP_Text Title;
        public TMP_Text Description;
        public TMP_Text ProductName;
        public TMP_Text ProductDescription;
        public TMP_Text ProductAmount;
        public Image ProductIcon;

        public StockInspectorPage StockPage;

        [Header("Buttons")]
        public Button RestockButton;
        public Button ChangeButton;
        public Button CloseButton;

        private Shelf TargetShelf;
        private StructureAsset Asset;

        private void Start()
        {
            RestockButton.onClick.AddListener(Restock);
            ChangeButton.onClick.AddListener(Change);
            CloseButton.onClick.AddListener(Close);
        }

        public void InspectShelf(Shelf Target)
        {
            TargetShelf = Target;
            Asset = Utility.Structures.GetAsset(Target.Name);

            ProductAsset ProductAsset = Utility.Products.GetAsset(Target.Content.Name);

            Title.text = Asset.Name;
            Description.text = Asset.Description;

            ProductName.text = ProductAsset.Name;
            ProductDescription.text = ProductAsset.Description;
            ProductIcon.sprite = ProductAsset.Icon;

            ProductAmount.text = Target.Content.Amount + " / " + Asset.MaxContent;

            Target.Content.AmountChanged += () => ProductAmount.text = Target.Content.Amount + " / " + Asset.MaxContent;
        }

        public void Restock()
        {
            Product ProductInStock = Utility.Products.GetProductNamed(TargetShelf.Content.Name, GameController.Singleton.Stock);

            int AmountToTake = Asset.MaxContent - TargetShelf.Content.Amount;

            if (ProductInStock == null)
            {
                Close();
                StockPage.Open();
            }

            else
            {
                Utility.Products.RemoveFromList(GameController.Singleton.Stock, TargetShelf.Content.Name, AmountToTake, out int RemovedAmount);
                TargetShelf.Content.Amount += RemovedAmount;
            }

            TargetShelf.UpdatePrefabs();

        }

        public void Change()
        {
            StockPage.Open(true);

            StockPage.ProductChosen += (string ProductName) =>
            {
                Utility.Products.AddToList(GameController.Singleton.Stock, TargetShelf.Content.Name, TargetShelf.Content.Amount);

                TargetShelf.Content.Name = ProductName;
                Restock();
                InspectShelf(TargetShelf);
            };

            gameObject.SetActive(false);
        }

        public void Close()
        {
            UIController.ToggleHotbar(true);
            gameObject.SetActive(false);
        }
    }
}