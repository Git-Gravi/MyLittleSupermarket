using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomInspector;

namespace MyLittleStore
{
    public class Npc : MonoBehaviour
    {
        [Header("Stats")]
        public string Name;
        public string Surname;
        public int Age;

        [Header("Settings")]
        public float WalkSpeed;
        public float TurningSpeed;
        public float DistanceFromGoal = .25f;

        [Header("Info")]
        [ReadOnly] public int PlaceInQueue = 99;
        [ReadOnly] public bool HasPaid = false;
        [ReadOnly] public Transform Goal = null;
        [ReadOnly] public List<Product> WantedProducts = new();
        [ReadOnly] public List<Product> CartProducts = new();

        private Rigidbody rb;
        private Animator ar;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            ar = GetComponent<Animator>();

            GenerateWantedProducts();

            StartCoroutine(Shopping());
        }

        private void Update()
        {
            if (Goal != null)
            {
                ar.SetTrigger("StartWalking");

                Vector3 LookPosition = Goal.position - transform.position;
                LookPosition.y = 0;

                Vector3 Velocity = transform.forward * WalkSpeed;
                Velocity.y = rb.velocity.y;

                float Distance = LookPosition.magnitude;

                if (LookPosition != Vector3.zero) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(LookPosition), Time.deltaTime * TurningSpeed);

                rb.velocity = Velocity;

                if (Distance < DistanceFromGoal) Goal = null;
            }
            else
            {
                ar.SetTrigger("StopWalking");

                rb.velocity = Vector3.zero;
            }

            ar.SetFloat("Speed", rb.velocity.magnitude / WalkSpeed);
        }

        private void GenerateWantedProducts()
        {
            List<Product> AvailableProducts = GameController.Singleton.Stock;

            int Choices = Mathf.Min(Random.Range(1, 5), AvailableProducts.Count);

            for (int i = 1; i <= Choices; i++)
            {
                Product ChosenProduct = AvailableProducts[Random.Range(0, AvailableProducts.Count)];

                Utility.Products.AddToList(WantedProducts, ChosenProduct.Name, Random.Range(1, 5));
            }
        }

        private IEnumerator Shopping()
        {

            /// Looks for the desired Products ///

            foreach (Product WantedProduct in WantedProducts)
            {
                /// Heads to the TargetShelf ///

                Shelf TargetShelf = Utility.Finding.GetNearestShelfWith(WantedProduct.Name, transform);
                if (TargetShelf == null) continue;

                TargetShelf.isFree = false;

                Goal = TargetShelf.WalkToPosition;
                while (Goal != null) yield return new WaitForSeconds(.1f);


                /// Takes the WantedProducts ///

                for (int Counter = 0; Counter < WantedProduct.Amount; Counter++)
                {
                    bool ProductTaken = TargetShelf.Take(WantedProduct.Name);

                    if (!ProductTaken) break;

                    Utility.Products.AddToList(CartProducts, WantedProduct.Name);

                    yield return new WaitForSeconds(1);
                }

                TargetShelf.isFree = true;

            }


            /// Heads to the Register ///

            if (CartProducts.Count > 0)
            {
                Register TargetRegister = null;
                int PreviousPlaceInQueue = PlaceInQueue;

                while (TargetRegister == null)
                {
                    TargetRegister = Utility.Finding.GetNearestOpenRegister(transform);
                    yield return new WaitForSeconds(.25f);
                }

                while (!TargetRegister.JoinQueue(this)) yield return new WaitForSeconds(.1f);

                do
                {
                    if (PlaceInQueue != PreviousPlaceInQueue) Goal = TargetRegister.QueuePositions[PlaceInQueue];
                    PreviousPlaceInQueue = PlaceInQueue;
                    yield return new WaitForSeconds(.1f);
                }
                while (PlaceInQueue > 0);

                Goal = TargetRegister.QueuePositions[PlaceInQueue];
                while (Goal != null) yield return new WaitForSeconds(.1f);


                /// Pays ///

                StartCoroutine(TargetRegister.BeginPayment(this));

                while (!HasPaid) yield return new WaitForSeconds(.1f);

            }


            /// Leaves ///

            Goal = GameController.Singleton.Exit;
            while (Goal != null) yield return new WaitForSeconds(.1f);

            Destroy(gameObject);

        }

    }

}