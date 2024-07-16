using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace MyLittleStore
{
    public class Utility : MonoBehaviour
    {
        public class Finding
        {
            // Get all instances of a structure type //
            public static List<Shelf> GetShelvesWith(string ProductName)
            {
                List<Shelf> Result = new();

                foreach (Shelf CurrentShelf in FindObjectsOfType<Shelf>())
                {
                    if (CurrentShelf.Contains(ProductName) && CurrentShelf.isFree) Result.Add(CurrentShelf);
                }

                return Result;
            }

            public static List<Register> GetOpenRegisters()
            {
                List<Register> Registers = new();

                foreach (Register Target in FindObjectsOfType<Register>())
                {
                    if (Target.Open) Registers.Add(Target);
                }

                return Registers;
            }


            // Get nearest specific structure //
            public static Transform GetNearest(List<Transform> Targets, Transform From)
            {
                Transform Nearest = null;
                float Distance = Mathf.Infinity;

                foreach (Transform Target in Targets)
                {
                    float TargetDistance = Vector3.Distance(From.position, Target.position);
                    if (TargetDistance < Distance)
                    {
                        Nearest = Target;
                        Distance = TargetDistance;
                    }
                }

                return Nearest;
            }

            public static Shelf GetNearestShelfWith(string ProductName, Transform From)
            {
                List<Transform> Transforms = new();
                Transform Result;

                foreach (Shelf TargetShelf in GetShelvesWith(ProductName))
                {
                    Transforms.Add(TargetShelf.transform);
                }

                Result = GetNearest(Transforms, From);

                return (Result == null) ? null : Result.GetComponent<Shelf>();
            }

            public static Register GetNearestOpenRegister(Transform From)
            {
                List<Transform> Transforms = new();
                Transform Result;

                foreach (Register TargetRegister in GetOpenRegisters())
                {
                    Transforms.Add(TargetRegister.transform);
                }

                Result = GetNearest(Transforms, From);

                return (Result == null) ? null : Result.GetComponent<Register>();
            }

        }

        public class Products
        {
            public static ProductAsset GetAsset(string ProductName)
            {
                foreach (ProductAsset Asset in GameController.Singleton.AllProducts)
                {
                    if (Asset.Name == ProductName) return Asset;
                }
                return null;
            }

            public static Product GetProductNamed(string ProductName, List<Product> ProductsList)
            {
                foreach (Product TargetProduct in ProductsList)
                {
                    if (TargetProduct.Name == ProductName) return TargetProduct;
                }
                return null;
            }

            public static int AddToList(List<Product> TargetList, string ProductName, int Amount = 1)
            {
                Product Target = GetProductNamed(ProductName, TargetList);

                if (Target == null)
                {
                    Target = new Product(ProductName);

                    TargetList.Add(Target);
                }

                Target.Amount += Amount;

                return Target.Amount;
            }

            public static int RemoveFromList(List<Product> TargetList, string ProductName, int Amount, out int RemovedAmount)
            {
                Product Target = GetProductNamed(ProductName, TargetList);

                if (Target == null)
                {
                    RemovedAmount = 0;
                    return 0;
                }

                if (Target.Amount - Amount <= 0) // Removes all of it
                {
                    RemovedAmount = Target.Amount;
                    TargetList.Remove(Target);
                    Target.Amount = 0;
                }
                else
                {
                    RemovedAmount = Amount;
                    Target.Amount -= Amount;
                }

                return Target.Amount;
            }

            public static int RemoveFromList(List<Product> TargetList, string ProductName, int Amount = 1)
            {
                return RemoveFromList(TargetList, ProductName, Amount, out int _);
            }

        }

        public class Structures
        {
            public static StructureAsset GetAsset(string StructureName)
            {
                foreach (StructureAsset Asset in GameController.Singleton.AllStructures)
                {
                    if (Asset.Name == StructureName) return Asset;
                }
                return null;
            }

        }

        public class UI
        {
            public static void SetColors(GameObject Root, Color Goal)
            {
                foreach (var Image in Root.GetComponentsInChildren<Image>())
                {
                    if (Image.gameObject.tag is "ColorLocked") continue;

                    Goal.a = Image.color.a; // Don't apply to the alpha

                    Image.color = Goal;
                }
            }

            public static void IncreaseColors(GameObject Root, Color Increment)
            {
                foreach (var Image in Root.GetComponentsInChildren<Image>())
                {
                    if (Image.gameObject.tag is "ColorLocked") continue;

                    Increment.a = Image.color.a; // Don't apply to the alpha

                    Image.color += Increment;
                }
            }
        }


    }

}