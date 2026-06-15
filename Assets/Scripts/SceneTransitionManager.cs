using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [SerializeField] private string mapSceneName = "MapScene";
    [SerializeField] private string battleSceneName = "BattleScene";
    [SerializeField] private GameObject transitionCanvas;

    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void LoadBattleScene()
    {
        if (isTransitioning == true)
        {
            return;
        }

        StartCoroutine(LoadBattleSceneRoutine());
    }

    public void ReturnToMapScene()
    {
        if (isTransitioning == true)
        {
            return;
        }

        StartCoroutine(ReturnToMapSceneRoutine());
    }

    private IEnumerator LoadBattleSceneRoutine()
    {
        isTransitioning = true;

        ShowTransitionCanvas();
        yield return null;

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(battleSceneName, LoadSceneMode.Additive);

        while (loadOperation.isDone == false)
        {
            yield return null;
        }

        SetSceneRootsActive(mapSceneName, false);
        HideTransitionCanvas();

        isTransitioning = false;
    }

    private IEnumerator ReturnToMapSceneRoutine()
    {
        isTransitioning = true;

        ShowTransitionCanvas();
        yield return null;

        SetSceneRootsActive(mapSceneName, true);

        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(battleSceneName);

        while (unloadOperation != null && unloadOperation.isDone == false)
        {
            yield return null;
        }

        HideTransitionCanvas();

        isTransitioning = false;
    }

    private void ShowTransitionCanvas()
    {
        if (transitionCanvas != null)
        {
            transitionCanvas.SetActive(true);
        }
    }

    private void HideTransitionCanvas()
    {
        if (transitionCanvas != null)
        {
            transitionCanvas.SetActive(false);
        }
    }

    private void SetSceneRootsActive(string sceneName, bool isActive)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);

        if (scene.IsValid() == false)
        {
            return;
        }

        GameObject managerRoot = transform.root.gameObject;
        GameObject transitionRoot = transitionCanvas != null ? transitionCanvas.transform.root.gameObject : null;

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            if (root == managerRoot || root == transitionRoot)
            {
                continue;
            }

            root.SetActive(isActive);
        }
    }
}
