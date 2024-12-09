using UnityEngine;

public class TurnManager
{
    public event System.Action OnTick;  //callback i�in altyap�
    private int TM_TurnCount; //tur say�s�

    


    public TurnManager()
    {
        TM_TurnCount = 1;  //ba�lang�� de�eri
    }

    public void Tick()
    {
        TM_TurnCount += 1;  //tur say�s�n� 1 artt�r
        Debug.Log("Tur Say�s� : " + TM_TurnCount);
        OnTick?.Invoke();   //callback kullanmak i�in bir �e�it k�sayol
    }
}
