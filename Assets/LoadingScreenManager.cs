using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadingScreenManager : MonoBehaviour
{
    

    [Header("Loading Visuals")]
   // public GameObject loadingIcon;
   // public GameObject loadingDoneIcon;
    public UILabel loadingText;
    public UISlider progressBar;
    public TweenAlpha fadeOverlayTween;


    [Header("Timing Settings")]
    public float waitOnLoadEnd = 0.25f;
    public float fadeDuration = 0.25f;

    [Header("Loading Settings")]
    public LoadSceneMode loadSceneMode = LoadSceneMode.Single;
    public ThreadPriority loadThreadPriority;

    [Header("Other")]
    // If loading additive, link to the cameras audio listener, to avoid multiple active audio listeners
    public AudioListener audioListener;

    AsyncOperation operation;
    Scene currentScene;

    public static bool forceUnload = false;
    public static int forceUnloadIndex = 0;
    public static int sceneToLoad = -1;
    // IMPORTANT! This is the build index of your loading scene. You need to change this to match your actual scene index
    static int loadingSceneIndex = 2;

    public static void LoadScene(int levelNum, bool ForceUnload = false, int ForceUnloadIndex = 0)
    {
        forceUnload = ForceUnload;
        forceUnloadIndex = ForceUnloadIndex;
        Application.backgroundLoadingPriority = ThreadPriority.High;
        sceneToLoad = levelNum;
        SceneManager.LoadScene(loadingSceneIndex);
    }

    void Start()
    {
        if (sceneToLoad < 0)
            return;

        //fadeOverlay.gameObject.SetActive(true); // Making sure it's on so that we can crossfade Alpha
        currentScene = SceneManager.GetActiveScene();
        StartCoroutine(LoadAsync(sceneToLoad));
    }

    private IEnumerator LoadAsync(int levelNum)
    {
        if (forceUnload)
        {
            SceneManager.UnloadScene(forceUnloadIndex);
        }

        ShowLoadingVisuals();
        FadeIn();

        yield return null;

        
        StartOperation(levelNum);

        //float lastProgress = 0f;

        // operation does not auto-activate scene, so it's stuck at 0.9
        while (DoneLoading() == false)
        {
            yield return null;
            
            //if (Mathf.Approximately(operation.progress, lastProgress) == false)
            //{
                progressBar.value = operation.progress;
                Debug.Log("Loading " + (operation.progress * 100).ToString()+"%");
                //lastProgress = operation.progress;
            //}
        }

        if (loadSceneMode == LoadSceneMode.Additive)
            audioListener.enabled = false;

        ShowCompletionVisuals();

//        yield return new WaitForSeconds(waitOnLoadEnd);

//        FadeOut();

//        yield return new WaitForSeconds(fadeDuration);

        if (loadSceneMode == LoadSceneMode.Additive)
            SceneManager.UnloadScene(currentScene.name);
        else
            operation.allowSceneActivation = true;
    }

    private void StartOperation(int levelNum)
    {
        Application.backgroundLoadingPriority = loadThreadPriority;
        operation = SceneManager.LoadSceneAsync(levelNum, loadSceneMode);


        if (loadSceneMode == LoadSceneMode.Single)
            operation.allowSceneActivation = false;
    }

    private bool DoneLoading()
    {
        return (loadSceneMode == LoadSceneMode.Additive && operation.isDone) || (loadSceneMode == LoadSceneMode.Single && operation.progress >= 0.9f);
    }

    void FadeIn()
    {
        fadeOverlayTween.duration = fadeDuration;
        fadeOverlayTween.PlayForward();
        
        
        //fadeOverlay.CrossFadeAlpha(0, fadeDuration, true);
    }

    void FadeOut()
    {
        fadeOverlayTween.duration = fadeDuration;
        fadeOverlayTween.PlayReverse();
    }

    void ShowLoadingVisuals()
    {
        //loadingIcon.gameObject.SetActive(true);
        //loadingDoneIcon.gameObject.SetActive(false);

        progressBar.value = 0f;
        loadingText.text = Localization.Get("Loading");
    }

    void ShowCompletionVisuals()
    {
        //loadingIcon.gameObject.SetActive(false);
        //loadingDoneIcon.gameObject.SetActive(true);

        progressBar.value = 1f;
        loadingText.text = Localization.Get("LoadingComplete");
    }

}