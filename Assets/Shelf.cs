using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInspector;

namespace MyLittleStore
{
    public class Shelf : MonoBehaviour
    {
        [Header("Info")]
        public string Name = "Basic shelf";

        [Header("Settings")]
        public Transform WalkToPosition;
        public Transform ProductPositions;
        public Transform ProductParent;

        [Header("Assets")]
        public AudioClip GrabbingSound;

        [ReadOnly] public Product Content;
        [HideInInspector] public bool isFree = true;

        private AudioSource aux;

        private void Start()
        {
            aux = GetComponent<AudioSource>();

            Content = GameController.Singleton.AllProducts[0].Get(10); // Temp

            UpdatePrefabs();
        }

        public void UpdatePrefabs()
        {
            ProductAsset Asset = Utility.Products.GetAsset(Content.Name);
            List<Transform> Positions = new();

            foreach (Transform Position in ProductPositions) Positions.Add(Position);
            foreach (Transform OldPrefab in ProductParent) Destroy(OldPrefab.gameObject);

            for (int Position = 0; Position < Mathf.Min(Content.Amount, Positions.Count); Position++)
            {
                Instantiate(Asset.Prefab, Positions[Position].position, Quaternion.identity, ProductParent);
            }
        }

        public bool Contains(string ProductName)
        {
            return ProductName == Content.Name && Content.Amount > 0;
        }

        public bool Take(string ProductName)
        {
            if (ProductName != Content.Name) return false;
            if (Content.Amount <= 0) return false;

            Content.Amount--;

            UpdatePrefabs();

            aux.PlayOneShot(GrabbingSound);

            return true;
        }

    }

    public enum ShelfType
    {
        Basic
    }
}
