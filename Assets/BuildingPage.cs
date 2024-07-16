using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MyLittleStore
{
    public class BuildingPage : MonoBehaviour
    {
        [Header("References")]
        public Transform Container;
        public Button DoneButton;

        [Header("Settings")]
        public Color SelectedButtonColor = Color.green;
        public GameObject StructureButtonPrefab;

        private void Start()
        {
            DoneButton.onClick.AddListener(Done);
        }

        private void RefreshContent()
        {
            foreach (Transform OldButton in Container) Destroy(OldButton.gameObject);

            GameObject CreateButton(StructureAsset Asset)
            {
                GameObject Button = Instantiate(StructureButtonPrefab, Container);

                TMP_Text NameLabel = Button.transform.Find("Name").GetComponent<TMP_Text>();
                TMP_Text PriceLabel = Button.transform.Find("Price").GetComponent<TMP_Text>();
                Image Icon = Button.transform.Find("Background/Top/Icon").GetComponent<Image>();

                Button.name = Asset.Name;
                NameLabel.text = Asset.Name;
                Icon.sprite = Asset.Icon;
                PriceLabel.text = "$" + Asset.Price;

                return Button;
            }

            foreach (StructureAsset Asset in GameController.Singleton.AllStructures)
            {
                GameObject Button = CreateButton(Asset);

                Button.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Utility.UI.SetColors(Button, SelectedButtonColor);


                });
            }

        }

        public void Open()
        {
            UIController.ToggleHotbar(false);
            gameObject.SetActive(true);

            RefreshContent();
        }

        public void Done()
        {
            UIController.ToggleHotbar(true);
            gameObject.SetActive(false);
        }

    }
}