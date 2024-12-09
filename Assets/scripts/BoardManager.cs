using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using NUnit.Framework;

public class BoardManager : MonoBehaviour
{

    public class CellData
    {
        public bool Passable; //geçilebilir mi ?
        public CellObject ContainedObject;  //üstünde bir birim var mý ? (kapsülleme) (*Ýçerebilir*)
    }

    private CellData[,] BM_BoardData; //boarddaki tüm tilelarýn durumunu saklar (gpt saðolsun)
    private Tilemap BM_Tilemap; //tilemap için deðiþken
    private Grid BM_Grid;

    public int Width;          //x eksenindeki tile sayýsý (map geniþliði)
    public int Height;         //y eksenindeki tile sayýsý (map yüksekliði)
    public Tile[] GroundTiles; //ground tile arrayi
    public Tile[] WallTiles;   //duvar tile arrayi
    public List<Vector2Int> BM_EmptyCellsList; //boþ celler(tile) için liste
    public PlayerController Player; //player için referans
    public FoodObject FoodPrefab;   //food için referans
    public WallObject WallPrefab;   //wall için referans
    public ExitCellObject ExitCellPrefab;  //exit için referans
    public EnemyObject EnemyPrefab;   //wall için referans


    public void Init()
    {
        //random map oluþturmak için yöntem
        BM_Tilemap = GetComponentInChildren<Tilemap>(); //tilemap öðesine eriþim

        BM_Grid = GetComponentInChildren<Grid>();       //grid öðesine eriþim

        BM_EmptyCellsList = new List<Vector2Int>();     //listeyi oluþtur/baþlat

        BM_BoardData = new CellData[Width, Height];     //celldata toplamak için yöntem

        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                
                Tile tile;
                BM_BoardData[x, y] = new CellData();  //boarddatayý güncelleme

                //belirtilen konum kenarlarsa duvar, deðilse ground oluþtur
                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                {
                    tile = WallTiles[Random.Range(0, WallTiles.Length)];
                    BM_BoardData[x, y].Passable = false;  //oluþturulan cell'in(tile) geçilebilir olma durumunu kontrol etmek için
                }
                else
                {
                    tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                    BM_BoardData[x, y].Passable = true;  //oluþturulan cell'in(tile) geçilebilir olma durumunu kontrol etmek için
                    BM_EmptyCellsList.Add(new Vector2Int(x, y));  //bu konumu listeye ekle
                }

                BM_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
        BM_EmptyCellsList.Remove(new Vector2Int(1, 1));  //bu konumu(player konumu) listeden sil

        Vector2Int endCoord = new Vector2Int(Width - 2, Height - 2);  //exitin pozisyonu
        AddObject(Instantiate(ExitCellPrefab), endCoord);  //objenin klonlamasý ve ayarlanmasý
        BM_EmptyCellsList.Remove(endCoord);  //bu konumu listeden sil

        GenerateWall();
        GenerateFood();
        GenerateEnemy();
    }

    public void SetCellTile(Vector2Int cellIndex, Tile tile)  //dikat
    {
        BM_Tilemap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);  //initte tilelarý deðiþtirebilmek için
    }


    //cellerdeki bilgileri kullanarak haritadaki konumunu saptamak için 
    public Vector3 CellToWorld(Vector2Int cellIndex)               //vector2Int cinsinden cell bilgilerini world positiona çevirir (ilk defa kullandým)
    {
        return BM_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    //cellerin geçilebilir olup olmadýðýný kontor etmek için(sahnenin kenarý(duvar) olma durumu)
    public CellData GetCellData(Vector2Int cellIndex)
    {
        if (cellIndex.x < 0 || cellIndex.x >= Width
            || cellIndex.y < 0 || cellIndex.y >= Height)
        {
            return null;
        }

        return BM_BoardData[cellIndex.x, cellIndex.y];  //data al
    }

    //ayný foodtaki gibi random konumda wall yaratmak için
    void GenerateWall()
    {
        int wallCount = Random.Range(6, 10);
        for (int i = 0; i < wallCount; ++i)
        {
            // random konum belirleme (listede olan konumlar içinden seçer)
            int randomIndex = Random.Range(0, BM_EmptyCellsList.Count);
            Vector2Int coord = BM_EmptyCellsList[randomIndex];

            BM_EmptyCellsList.RemoveAt(randomIndex);  //bu konumu listeden sil
            CellData data = BM_BoardData[coord.x, coord.y];  //data al

            //Wall yaratmak için onu prefabden klonlama, pozisyonunu belirleme ve datayý güncelleme
            WallObject newWall = Instantiate(WallPrefab);
            AddObject(newWall, coord);
        }
    }

    //random konumda food yaratmak için
    void GenerateFood()
    {
        //Food birimini ayarlama
        int foodCount = 5;
        for (int i = 0; i < foodCount; ++i)
        {
            // random konum belirleme (listede olan konumlar içinden seçer)
            int randomIndex = Random.Range(0, BM_EmptyCellsList.Count);
            Vector2Int coord = BM_EmptyCellsList[randomIndex];

            BM_EmptyCellsList.RemoveAt(randomIndex);  //bu konumu listeden sil
            CellData data = BM_BoardData[coord.x, coord.y];  //data al
            if (data.Passable && data.ContainedObject == null)  //belirtilen konumdaki cell(tile) geçilebilir ve içerebilir ise çalýþýr
            {
                //Food yaratmak için onu prefabden klonlama, pozisyonunu belirleme ve datayý güncelleme
                FoodObject newFood = Instantiate(FoodPrefab);
                AddObject(newFood, coord);
            }
        }
    }

    void GenerateEnemy()
    {
        int enemyCount = Random.Range(1, 4);
        for (int i = 0; i < enemyCount; ++i)
        {
            // random konum belirleme (listede olan konumlar içinden seçer)
            int randomIndex = Random.Range(0, BM_EmptyCellsList.Count);
            Vector2Int coord = BM_EmptyCellsList[randomIndex];

            BM_EmptyCellsList.RemoveAt(randomIndex);  //bu konumu listeden sil
            CellData data = BM_BoardData[coord.x, coord.y];  //data al

            //enemy yaratmak için onu prefabden klonlama, pozisyonunu belirleme ve datayý güncelleme
            EnemyObject newEnemy = Instantiate(EnemyPrefab);
            AddObject(newEnemy, coord);
        }
    }


    //hatalarý engellemek ve kontrolü için düzenleme
    void AddObject(CellObject obj, Vector2Int coord)  
    {
        CellData data = BM_BoardData[coord.x, coord.y];  //datayý al
        obj.transform.position = CellToWorld(coord);     //pozisyonu belirleme
        data.ContainedObject = obj;  //cell üzerindeki obj datasý
        obj.Init(coord);  //init ayarý
    }

    public Tile GetCellTile(Vector2Int cellIndex)
    {
        return BM_Tilemap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
    }

    //Sil baþtan baþlamak gerek bazen, GameBoardý sýfýrlamak...
    public void Clean()
    {
        
        if (BM_BoardData == null)  //gb'ýn oluþup oluþmadýðýný kontrol eder
            return;

        //bütün hücreleri tarar ve hepsini siler sonra da boþ deðer verir
        for (int y = 0; y < Height; ++y)
        {
            for (int x = 0; x < Width; ++x)
            {
                var cellData = BM_BoardData[x, y];

                if (cellData.ContainedObject != null)
                {
                    Destroy(cellData.ContainedObject.gameObject);
                }

                SetCellTile(new Vector2Int(x, y), null);
            }
        }
    }

}