using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Image blackPanel;
    [SerializeField] private float fadeDuration = 3f;
    private bool isTransitioning = false;

    void Awake()
    {
        Time.timeScale = 1f;
    }

    public void PlayGameButton()
    {
        if (isTransitioning) return;
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        isTransitioning = true;
        float elapsedTime = 0f;
        blackPanel.gameObject.SetActive(true);
        Color panelColor = blackPanel.color;
        panelColor.a = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            panelColor.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            blackPanel.color = panelColor;
            yield return null;
        }

        SceneManager.LoadScene("Level");
    }

    public void ExitGameButton()
    {
        if (isTransitioning) return;
        StartCoroutine(ExitGame());
    }

    private IEnumerator ExitGame()
    {
        isTransitioning = true;
        float elapsedTime = 0f;
        blackPanel.gameObject.SetActive(true);
        Color panelColor = blackPanel.color;
        panelColor.a = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            panelColor.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            blackPanel.color = panelColor;
            yield return null;
        }
        Application.Quit();
    }
}
