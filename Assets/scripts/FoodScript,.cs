using UnityEngine;

public class FoodObject : CellObject
{
    public int AmountGranted = 10;

    public override void PlayerEntered()  //player girdi�inde...
    {
        Destroy(gameObject);  //foodu haritadan sil
        Debug.Log("Yemek Artt�");

        GameManager.Instance.ChangeFood(AmountGranted);  //food miktar�n� kontrol eder(artt�r�r/azalt�r)
    }
}
