using UnityEngine;
using UnityEngine.Tilemaps;

public class ExitCellObject : CellObject
{
    public Tile EndTile;

    public override void Init(Vector2Int coord)  //celler üzerinde oynama yapabilmek için
    {
        base.Init(coord);
        GameManager.Instance.Board.SetCellTile(coord, EndTile);  //celli exit olarak ayarlar
    }

    public override void PlayerEntered()  //player celle girerse
    {
        GameManager.Instance.NewLevel();  //yeni level oluþtur
        Debug.Log("çýkýþa ulaþýldý");
    }
}
