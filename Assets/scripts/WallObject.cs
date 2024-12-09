using UnityEngine;
using UnityEngine.Tilemaps;


public class WallObject : CellObject
{
    public Tile ObstacleTile;  //wall için özel set
    public int MaxHealth = 3;  //obstacle için can

    private int WO_HealthPoint;
    private Tile WO_OriginalTile;  //obstacle kýrýlýnca yerine gelecek tile


    public override void Init(Vector2Int cell)  //celler üzerinde oynama yapabilmek için
    {
        base.Init(cell);

        WO_HealthPoint = MaxHealth;

        WO_OriginalTile = GameManager.Instance.Board.GetCellTile(cell);  //tile için güncelleme
        GameManager.Instance.Board.SetCellTile(cell, ObstacleTile); //celli obstacle olarak ayarlar 
    }

    public override bool PlayerWantsToEnter() //PlayerController için referans (girmek istediði cell müsait deðil mi?)
    {
        WO_HealthPoint -= 1;  //caný 1 azalt

        //Eðer caný varsa false döndür(geçiþe izin verme)
        if (WO_HealthPoint > 0)
        {
            return false;
        }

        //yoksa yok et ve true döndür(geçiþe izin ver)
        GameManager.Instance.Board.SetCellTile(C_Cell, WO_OriginalTile);  
        Destroy(gameObject);
        return true;
    }

}
