using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FirstScene : MonoBehaviour
{
    public static class Events
    {
        public static Action OnLoadingComplete;
    }
    public string m_NextScene;
    private AsyncOperation m_AsyncLoad;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        SceneManager.sceneLoaded += SceneManager_activeSceneChanged;
        StartCoroutine(OnLoadingNextScene(m_NextScene));
        
        Events.OnLoadingComplete += OnLoadingComplete;
    }
    private void SceneManager_activeSceneChanged(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Change Scene Success");
        
    }
    IEnumerator OnLoadingNextScene(string sceneName)
    {
        yield return new WaitForSeconds(1f);
        m_AsyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        m_AsyncLoad.allowSceneActivation = false;
        yield return new WaitForEndOfFrame();

        while (m_AsyncLoad.progress < 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }
        m_AsyncLoad.allowSceneActivation = true;

    }
    private void OnLoadingComplete()
    {
        gameObject.SetActive(false);
    }
}