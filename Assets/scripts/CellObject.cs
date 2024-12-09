using UnityEngine;

public class CellObject : MonoBehaviour  //inheritance için
{

    protected Vector2Int C_Cell; //sadece belli öðelerin eriþebilmesi için

    public virtual void Init(Vector2Int cell)
    {
        C_Cell = cell;
    }

    public virtual bool PlayerWantsToEnter()  //PlayerController için referans (girmek istediði cell müsait mi?)
    {
        return true;
    }

    //Oyuncu nesnenin bulunduðu celle(tile) girdiðinde çaðrýlýr.
    public virtual void PlayerEntered()
    {

    }
     


}
