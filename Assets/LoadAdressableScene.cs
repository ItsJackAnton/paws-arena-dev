using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadAdressableScene : MonoBehaviour
{
    [SerializeField] private AssetReference _scene;
    [SerializeField] private List<AssetReference> _references = new List<AssetReference>();

    private AsyncOperationHandle<SceneInstance> handle;
    public Text loadText;

    public GameObject LoadingScneePanel;

    private void Awake() 
    {
        DontDestroyOnLoad(gameObject);    
    }

    void Start()
    {
        StartCoroutine(DownloadScenes());

    }

    IEnumerator DownloadScenes()
    {
        var addresables = Addressables.InitializeAsync();
        yield return new WaitUntil(() => addresables.IsDone);
        var downloadScene = Addressables.LoadSceneAsync(_scene, LoadSceneMode.Additive);
        downloadScene.Completed += SceneDownloadComplete;
        Debug.LogError("Starting Scene Download");

        while(!downloadScene.IsDone)
        {
            var status = downloadScene.GetDownloadStatus();
            float progress = status.Percent;
            loadText.text = ""+(int)(progress * 100);
            yield return null;
        }

        Debug.LogError("Download Complete, Starting next Scene");
        loadText.text = "100";

    }

    void SceneDownloadComplete(AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance> _handle)
    {
        if(_handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.LogError(_handle.Result.Scene.name + " Successfully Loaded");
            loadText.gameObject.SetActive(false);
            LoadingScneePanel.SetActive(false);
            handle = _handle;
            //StartCoroutine(UnlocdScene());
        }
    }

    IEnumerator UnlocdScene()
    {
        yield return new WaitForSeconds(10);
        Addressables.UnloadSceneAsync(handle, true).Completed += op =>
        {
            if(op.Status == AsyncOperationStatus.Succeeded)
            {
                loadText.gameObject.SetActive(false);
                LoadingScneePanel.SetActive(false);
                Debug.LogError("Successfully Unloaded Scene");
            }
            
        };

        yield return new WaitForSeconds(5);
        StartCoroutine(DownloadScenes());
    }
}