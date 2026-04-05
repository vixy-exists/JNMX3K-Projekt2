using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleEnter : MonoBehaviour
{
    [Tooltip("Name of the scene or SubScene to load (must be added to Build Settings if using SceneManager)")]
    public string sceneToLoad;

    [Tooltip("Load the target scene additively (true) or replace current scene (false)")]
    public bool loadAdditive = true;

    [Tooltip("Optional delay (seconds) before loading the scene")]
    public float delayBeforeLoad = 0f;

    [Tooltip("Only trigger when object with tag 'Player' enters the trigger")]
    public bool requirePlayerTag = true;

    // If you prefer 3D triggers use OnTriggerEnter, for 2D use OnTriggerEnter2D. Both are supported here.
    void OnTriggerEnter(Collider other)
    {
        if (!requirePlayerTag || other.CompareTag("Player"))
            StartCoroutine(LoadSceneRoutine());
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!requirePlayerTag || other.CompareTag("Player"))
            StartCoroutine(LoadSceneRoutine());
    }

    IEnumerator LoadSceneRoutine()
    {
        if (delayBeforeLoad > 0f)
            yield return new WaitForSeconds(delayBeforeLoad);

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogWarning("BattleEnter: sceneToLoad is empty, cannot load scene.");
            yield break;
        }

        var mode = loadAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneToLoad, mode);
        if (op == null)
        {
            Debug.LogWarning($"BattleEnter: Failed to start loading scene '{sceneToLoad}'. Make sure the scene name is correct and added to Build Settings.");
            yield break;
        }

        // Optionally wait until scene is loaded
        while (!op.isDone)
            yield return null;

        Debug.Log($"BattleEnter: Scene '{sceneToLoad}' loaded ({mode}).");
    }
}
