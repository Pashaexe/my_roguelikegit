using UnityEngine;

public class TurnManager
{
    public event System.Action OnTick;  //callback için altyapý
    private int TM_TurnCount; //tur sayýsý

    


    public TurnManager()
    {
        TM_TurnCount = 1;  //baþlangýç deðeri
    }

    public void Tick()
    {
        TM_TurnCount += 1;  //tur sayýsýný 1 arttýr
        Debug.Log("Tur Sayýsý : " + TM_TurnCount);
        OnTick?.Invoke();   //callback kullanmak için bir çeþit kýsayol
    }
}
