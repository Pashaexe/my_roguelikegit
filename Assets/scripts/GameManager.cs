using UnityEngine;
using UnityEngine.UIElements;


public class GameManager : MonoBehaviour //oyun buradan y�netilecek
{
    public static GameManager Instance { get; private set; } //Bu �zellik, GameManager s�n�f�n�n sadece bir �rne�inin var olmas�n� sa�lar.(gpt'den kopya �ektim)

    public TurnManager GM_turnManager;

    public BoardManager Board;
    public PlayerController Player;
    public UIDocument UIDoc;

    //UI i�in referanslar
    private int GM_FoodAmount = 100;
    private Label GM_FoodLabel;
    private VisualElement GM_GameOverPanel;
    private Label GM_GameOverMessage;

    private int GM_CurrentLevel = 1; //�uanki level say�s�

    //gamemanager ��esinin instance'�n� kontrol eder
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GM_turnManager = new TurnManager();
        GM_turnManager.OnTick += OnTurnHappen;

        GM_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel"); //UI da yaz� yazabilmek i�in

        GM_GameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");  //Game Over panelini bulur ve bir de�i�kene atar.
        GM_GameOverMessage = GM_GameOverPanel.Q<Label>("GameOverMessage");             //Panel i�indeki mesaj eleman�na eri�ir.
         
        GM_GameOverPanel.style.visibility = Visibility.Hidden;                         //Paneli ba�lang��ta g�r�nmez yapar.
 
        GM_FoodLabel.text = "Food : " + GM_FoodAmount;  //UI daki yaz� GM_FoodLabel.text = "Food : " + GM_FoodAmount;  //UI daki yaz�

        NewLevel();
    }

    
    public void NewLevel()
    {
        GM_GameOverPanel.style.visibility = Visibility.Hidden;  //Paneli ba�lang��ta g�r�nmez yapar.

        GM_CurrentLevel = 1;
        GM_FoodAmount = 30;
        GM_GameOverPanel.style.visibility = Visibility.Hidden;  //Paneli ba�lang��ta g�r�nmez yapar.

        Board.Clean();  //sil
        Board.Init();   //yeniden kur
        Player.Init();
        Player.Spawn(Board, new Vector2Int(1, 1));  //playeri konumland�r
    }

    void OnTurnHappen()  //her tur ba��nda
    {
        ChangeFood(-1);  //foodu 1 azalt
    }


    public void ChangeFood(int amount)  //food de�erini g�ncelle
    {
        GM_FoodAmount += amount;
        //UI ve Logu g�ncelle
        GM_FoodLabel.text = "Food : " + GM_FoodAmount;
        Debug.Log("Current amount of food : " + GM_FoodAmount);

        //e�er food 0 alt�na d��erse gamover yaz�s�n� g�r�n�r yap
        if (GM_FoodAmount <= 0)
        {
            Player.GameOver();
            GM_GameOverPanel.style.visibility = Visibility.Visible;
            GM_GameOverMessage.text = "Game Over!\n\nYou traveled through\n\npress enter " + GM_CurrentLevel + " levels";  //n/n sat�r atlamak i�in 

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
