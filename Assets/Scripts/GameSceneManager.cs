using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{

    [SerializeField]
    private Button restartButton;

    [SerializeField]
    private TextMeshProUGUI gameInfoText;
    // Start is called before the first frame update
    void Start()
    {
        string message = GameManager.isGameWon ? "You win" : "You lose";
        gameInfoText.text = message;
    }

    /// <summary>
    /// Loads GameScene
    /// </summary>
    public void RestartGame(){
        SceneManager.LoadScene("GameScene");
    }
}
