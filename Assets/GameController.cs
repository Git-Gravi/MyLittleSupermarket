using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyLittleStore
{
    public class GameController : MonoBehaviour
    {
        public static GameController Singleton;

        [Header("Stats")]
        public float Money = 0;
        public bool Open = true;
        public List<Product> Stock = new();


        [Header("Settings")]
        public List<ProductAsset> AllProducts = new();
        public List<StructureAsset> AllStructures = new();


        [Header("Assets")]
        public GameObject CustomerPrefab;


        [HideInInspector] public Transform Entry, Exit;


        private Transform NpcParent;


        private void Awake()
        {
            Singleton = this;

            Entry = GameObject.FindGameObjectWithTag("Entry").transform;
            Exit = GameObject.FindGameObjectWithTag("Exit").transform;
            NpcParent = GameObject.FindGameObjectWithTag("NPC Parent").transform;

            StartCoroutine(SpawnCustomers());
        }

        IEnumerator SpawnCustomers()
        {
            while (Open)
            {
                yield return new WaitForSeconds(Random.Range(1, 5));

                Instantiate(CustomerPrefab, Entry.position, Quaternion.identity, NpcParent);
            }
        }

    }

    #region Classes

    [System.Serializable]
    public class StructureAsset
    {
        [Header("Info")]
        public string Name;
        public string Description;
        public ShelfType Type;
        public float Price;
        public int MaxContent;

        [Header("Assets")]
        public GameObject Prefab;
        public Sprite Icon;
    }

    [System.Serializable]
    public class ProductAsset
    {
        [Header("Info")]
        public string Name;
        public string Description;
        public float Price;
        public float SellingPrice;

        [Header("Settings")]
        public float MinSellingPrice;
        public float MaxSellingPrice;
        public List<ShelfType> Shelves;

        [Header("Assets")]
        public GameObject Prefab;
        public Sprite Icon;

        public Product Get(int Amount)
        {
            return new(Name, Amount);
        }
    }

    [System.Serializable]
    public class Product
    {
        [SerializeField] public string Name;
        [SerializeField] private int amount;

        public int Amount
        {
            get => amount;
            set
            {
                if (amount != value)
                {
                    amount = value;
                    AmountChanged?.Invoke();
                }
            }
        }


        public delegate void Changed();

        public event Changed AmountChanged = delegate { };


        public Product(string ProductName, int ProductAmount = 0)
        {
            Name = ProductName;
            Amount = ProductAmount;
        }
    }

    #endregion
}