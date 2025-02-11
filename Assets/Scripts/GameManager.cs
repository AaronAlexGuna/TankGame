using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public TMP_Text scoreText;
    public TMP_Text hpText;
    public GameObject gameOverPanel;

    private int score = 0;
    private PTController player;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PTController>();
        if (gameOverPanel) gameOverPanel.SetActive(false);
        UpdateUI();
    }

    private void Update()
    {
        if (player != null && hpText != null)
            hpText.text = "HP: " + player.hitPoints;
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (scoreText) scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        if (gameOverPanel) gameOverPanel.SetActive(true);
        Time.timeScale = 0;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
