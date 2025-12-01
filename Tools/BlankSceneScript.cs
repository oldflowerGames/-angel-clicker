using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class BlankSceneScript : MonoBehaviour
{
    bool loadNextScene;
    public AsyncOperationHandle<SceneInstance> videoLoadHandle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(LoadScene());
    }

    public IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(1f);
        //AsyncOperation sceneLoad = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);//unity way, doesn't speed up loading by much
        //sceneLoad.completed += SceneLoadCompleted;

        videoLoadHandle = Addressables.LoadSceneAsync("Assets/Scenes/MainScene.unity", LoadSceneMode.Single);//addressable way
        //videoLoadHandle.completed += SceneLoadCompleted;

        //sceneLoad.allowSceneActivation = false;
        //while (sceneLoad.isDone == false)
        //{
        //    if (sceneLoad.progress >= 0.9f)
        //    {
        //        if (loadNextScene)
        //        {
        //            sceneLoad.allowSceneActivation = true;
        //        }
        //    }
        //    yield return null;
        //}
    }

    public void SceneLoadCompleted(AsyncOperation async)
    {
        SceneManager.UnloadSceneAsync(0);
    }
}
