using UnityEngine;

public class CellObject : MonoBehaviour  //inheritance i�in
{

    protected Vector2Int C_Cell; //sadece belli ��elerin eri�ebilmesi i�in

    public virtual void Init(Vector2Int cell)
    {
        C_Cell = cell;
    }

    public virtual bool PlayerWantsToEnter()  //PlayerController i�in referans (girmek istedi�i cell m�sait mi?)
    {
        return true;
    }

    //Oyuncu nesnenin bulundu�u celle(tile) girdi�inde �a�r�l�r.
    public virtual void PlayerEntered()
    {

    }
     


}
