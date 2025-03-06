using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using static UnityEditor.SceneManagement.SceneHierarchyHooks;

public class SceneTranslateManager : Singleton<SceneTranslateManager>
{
    [Header("所有场景信息")]
    public SceneData sceneData;

    [Header("开始场景")]
    public SceneName startSceneName;

    [Header("当前场景")]
    public SceneInfo curSceneInfo;

    [Header("FadeCanvas")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration;

    private bool isFade;

    public string lookScene;//暂时，摄像机对焦的场景名

    private void Start()
    {
        
        if(sceneData.sceneInfoList.Count != 0)
        {
            CameraController.Instance.canFreeMove = true;
            Transition(string.Empty, startSceneName.ToString());
        }

    }


    /// <summary>
    /// 场景间转换
    /// </summary>
    /// <param name="to"></param>
    public void SceneTransitionScene(string to)
    {


        foreach (var sceneInfo in sceneData.sceneInfoList)
        {
            if (sceneInfo.Name == to)
            {
                curSceneInfo = sceneInfo;
                lookScene = sceneInfo.Name;
                break;
            }
        }

        Transition(SceneName.CurScene.ToString(), SceneName.CurScene.ToString());
    }

    /// <summary>
    /// 场景返回地图
    /// </summary>
    public void SceneTransitionMap()
    {
        CameraController.Instance.canFreeMove = true;
        curSceneInfo = null;
        Transition(SceneName.CurScene.ToString(), SceneName.Map.ToString());
    }


    /// <summary>
    /// 地图转换到场景
    /// </summary>
    /// <param name="sceneName"></param>
    public void MapTransitionScene(string sceneName)
    {
        CameraController.Instance.canFreeMove = false;

        foreach (var sceneInfo in sceneData.sceneInfoList)
        {
            if (sceneInfo.Name == sceneName)
            {
                curSceneInfo = sceneInfo;
                lookScene = sceneInfo.Name;
                break;
            }
        }
        
        Transition(SceneName.Map.ToString(), SceneName.CurScene.ToString());
    }

    /// <summary>
    /// 从from转换到to场景
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void Transition(string from, string to)
    {

        if (!isFade)
        {
            StartCoroutine(TransitionToScene(from, to));
        }
    }

    /// <summary>
    /// 异步卸载加载场景
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    private IEnumerator TransitionToScene(string from, string to)
    {
        yield return Fade(1);

        if (from != string.Empty)
        {
            yield return SceneManager.UnloadSceneAsync(from);
        }

        yield return SceneManager.LoadSceneAsync(to, LoadSceneMode.Additive);

        Scene newscene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(newscene);

        if(to == SceneName.Map.ToString())
        {
            CameraController.Instance.MoveCamera(lookScene);
        }
        else
        {
            CameraController.Instance.MoveCamera(string.Empty);
        }
        

        yield return Fade(0);
    }


    private IEnumerator Fade(float targetAlpha)
    {
        isFade = true;

        fadeCanvas.blocksRaycasts = true;
        float speed = Mathf.Abs(fadeCanvas.alpha - targetAlpha) / fadeDuration;

        while (!Mathf.Approximately(fadeCanvas.alpha, targetAlpha))
        {
            fadeCanvas.alpha = Mathf.MoveTowards(fadeCanvas.alpha, targetAlpha, speed * Time.deltaTime);
            yield return null;
        }

        fadeCanvas.blocksRaycasts = false;

        isFade = false;
    }


}
