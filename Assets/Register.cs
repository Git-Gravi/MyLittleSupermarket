using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyLittleStore
{
    public class Register : MonoBehaviour
    {
        [Header("Stats")]
        public bool Open = false;

        [Header("Settings")]
        public List<Transform> QueuePositions = new();

        [Header("Assets")]
        public AudioClip ScanSound;
        public AudioClip PaymentCompletedSound;

        [HideInInspector] public bool Full = false;

        [SerializeField]
        private List<Npc> Queue;

        AudioSource aux;

        private void Start()
        {
            aux = GetComponent<AudioSource>();
        }

        private void Update()
        {
            Full = Queue.Count >= QueuePositions.Count;
        }

        public bool JoinQueue(Npc Customer)
        {
            if (Full) return false;

            Queue.Add(Customer);
            Customer.PlaceInQueue = Mathf.Clamp(Queue.Count - 1, 0, QueuePositions.Count - 1);

            return true; // Tells the Npc that a place has been found.

        }


        public IEnumerator BeginPayment(Npc Customer)
        {
            float Check = 0;

            foreach (Product CartProduct in Customer.CartProducts)
            {
                for (int Piece = 1; Piece <= CartProduct.Amount; Piece++)
                {
                    ProductAsset Asset = Utility.Products.GetAsset(CartProduct.Name);
                    Check += Asset.SellingPrice;

                    aux.PlayOneShot(ScanSound);

                    yield return new WaitForSeconds(Random.Range(.25f, 1.5f));
                }
            }

            GameController.Singleton.Money += Check;

            aux.PlayOneShot(PaymentCompletedSound);

            Customer.HasPaid = true;

            Queue.Remove(Customer);
            foreach (Npc InQueueCustomer in Queue) InQueueCustomer.PlaceInQueue--;

        }

    }
}
