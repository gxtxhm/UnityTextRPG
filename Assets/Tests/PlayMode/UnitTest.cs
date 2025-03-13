using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class UnitTest
{
    [Test]
    public void ItemGenerationTest()
    {
        GameObject gameObject = new GameObject();
        GameManager gameManager = gameObject.AddComponent<GameManager>();

        ItemManager.Instance.Inventory.Clear();
        // Act: Init is automatically called during Awake
        //yield return null;
        gameManager.Init();
         
        // Assert: Verify that exactly 5 items are generated
        Assert.AreEqual(5, ItemManager.Instance.GetItemCount(), "Expected 5 items to be generated.");
    }
}