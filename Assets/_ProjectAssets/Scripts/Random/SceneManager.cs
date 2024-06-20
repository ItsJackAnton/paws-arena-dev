using UnityEngine;

public class SceneManager : MonoBehaviour
{
    private const string NFT_SELECTION = "NftSelection";
    private const string MAIN_MENU = "MainMenu";
    private const string LEADERBOARD = "Leaderboard";
    private const string TUTORIAL = "Tutorial";
    private const string AFTER_GAME = "AfterGame";
    private const string LOGIN_SCENE = "Login";
    public const string GAME_ROOM = "GameRoom";
    public const string GAME_ROOM_SPECTATOR = "GameRoomSpectator";
    public const string SINGLE_PLAYER = "SinglePlayerGameRoom";
    public const string GAME_SCENE = "GameScene";
    public const string SINGLE_PLAYER_GAME = "SinglePlayerGame";
    public const string GUILDS = "Guilds";
    public const string GAME_SPECTATOR = "GameSpectator";
    public static SceneManager Instance;

    private void Awake()
    {
        if (Instance==null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadNftSelection()
    {
        LoadScene(NFT_SELECTION);
    }

    public void LoadGuilds()
    {
        LoadScene(GUILDS);
    }

    public void LoadLoginScene()
    {
        LoadScene(LOGIN_SCENE);
    }

    public void LoadLeaderboard()
    {
        LoadScene(LEADERBOARD);
    }

    public void LoadMainMenu()
    {
        LoadScene(MAIN_MENU);
    }

    public void LoadTutorial()
    {
        LoadScene(TUTORIAL);
    }

    public void LoadAfterGame()
    {
        LoadScene(AFTER_GAME);
    }

    public void Reload()
    {
        LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void LoadScene(string _key)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(_key);
    }
}
