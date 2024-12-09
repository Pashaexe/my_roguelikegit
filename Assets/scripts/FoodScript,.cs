using UnityEngine;

public class FoodObject : CellObject
{
    public int AmountGranted = 10;

    public override void PlayerEntered()  //player girdiðinde...
    {
        Destroy(gameObject);  //foodu haritadan sil
        Debug.Log("Yemek Arttý");

        GameManager.Instance.ChangeFood(AmountGranted);  //food miktarýný kontrol eder(arttýrýr/azaltýr)
    }
}
