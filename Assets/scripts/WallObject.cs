using UnityEngine;
using UnityEngine.Tilemaps;


public class WallObject : CellObject
{
    public Tile ObstacleTile;  //wall i�in �zel set
    public int MaxHealth = 3;  //obstacle i�in can

    private int WO_HealthPoint;
    private Tile WO_OriginalTile;  //obstacle k�r�l�nca yerine gelecek tile


    public override void Init(Vector2Int cell)  //celler �zerinde oynama yapabilmek i�in
    {
        base.Init(cell);

        WO_HealthPoint = MaxHealth;

        WO_OriginalTile = GameManager.Instance.Board.GetCellTile(cell);  //tile i�in g�ncelleme
        GameManager.Instance.Board.SetCellTile(cell, ObstacleTile); //celli obstacle olarak ayarlar 
    }

    public override bool PlayerWantsToEnter() //PlayerController i�in referans (girmek istedi�i cell m�sait de�il mi?)
    {
        WO_HealthPoint -= 1;  //can� 1 azalt

        //E�er can� varsa false d�nd�r(ge�i�e izin verme)
        if (WO_HealthPoint > 0)
        {
            return false;
        }

        //yoksa yok et ve true d�nd�r(ge�i�e izin ver)
        GameManager.Instance.Board.SetCellTile(C_Cell, WO_OriginalTile);  
        Destroy(gameObject);
        return true;
    }

}
