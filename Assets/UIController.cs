using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MyLittleStore
{
    public class UIController : MonoBehaviour
    {
        public TMP_Text MoneyLabel;
        public GameObject Hotbar;
        public ShelfInspectorPage ShelfPage;

        private static GameObject StaticHotbarReference;
        private static NumberFormatInfo MoneyFormat = new();

        private float CurrentMoneyAmount, GoalMoneyAmount;

        private void Awake()
        {
            StaticHotbarReference = Hotbar;

            MoneyFormat.CurrencySymbol = "";
        }

        private void Update()
        {
            /// Updates MoneyLabel ///
            GoalMoneyAmount = GameController.Singleton.Money;
            CurrentMoneyAmount = Mathf.Lerp(CurrentMoneyAmount, GoalMoneyAmount, .1f);
            MoneyLabel.text = CurrentMoneyAmount.ToString("C", MoneyFormat);

            /// Detects clicks on Shelves ///
            if (Input.GetMouseButtonDown(0))
            {
                Ray MouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                bool RaycastSuccess = Physics.Raycast(MouseRay, out RaycastHit Hit);
                Shelf ClickedShelf = null;

                if (RaycastSuccess) ClickedShelf = Hit.transform.GetComponent<Shelf>();
                if (!ClickedShelf) return;

                ToggleHotbar(false);
                ShelfPage.gameObject.SetActive(true);
                ShelfPage.InspectShelf(ClickedShelf);
            }

        }

        public static void ToggleHotbar(bool Visible)
        {
            StaticHotbarReference.SetActive(Visible);
        }

    }

}
