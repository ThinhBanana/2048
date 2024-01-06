using TMPro;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public TileBoard tileBoard;
    public CanvasGroup gameOverCanvasGroup;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI highScoreText;

    private int currentScore;


    private void Start()
    {
        NewGame();
    }

    public void NewGame()
    {
        SetScore(0);
        highScoreText.text = LoadHighScore().ToString();
        gameOverCanvasGroup.alpha = 0f;
        gameOverCanvasGroup.interactable = false;

        tileBoard.ClearBoard();
        tileBoard.CreateTile();
        tileBoard.CreateTile();
        tileBoard.enabled = true;
    }

    public void GameOver()
    {
        tileBoard.enabled = false;
        StartCoroutine(IFade(gameOverCanvasGroup, 1f));
    }

    private IEnumerator IFade(CanvasGroup canvasGroup, float to)
    {
        yield return new WaitForSecondsRealtime(Constants.FADE_GAME_OVER_DELAY);

        float elapsed = 0f;

        float from = canvasGroup.alpha;

        while (elapsed < Constants.FADE_GAME_OVER_DURATION)
        {
            canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / Constants.FADE_GAME_OVER_DURATION);
            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = to;
        canvasGroup.interactable = true;
    }

    public void IncreaseScore(int points)
    {
        SetScore(currentScore + points);
    }

    private void SetScore(int score)
    {
        this.currentScore = score;

        currentScoreText.text = currentScore.ToString();

        SaveHighScore();
    }

    private void SaveHighScore()
    {
        int highScore = LoadHighScore();

        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt("highScore", currentScore);
            highScoreText.text = currentScore.ToString();
        }
    }

    private int LoadHighScore()
    {
        return PlayerPrefs.GetInt("highScore", 0);
    }


}
