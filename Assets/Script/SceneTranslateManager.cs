using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using static UnityEditor.SceneManagement.SceneHierarchyHooks;

public class SceneTranslateManager : Singleton<SceneTranslateManager>
{
    [Header("���г�����Ϣ")]
    public SceneData SceneData;

    [Header("��ʼ����")]
    public SceneName startSceneName;

    [Header("��ǰ����")]
    public SceneInfo curSceneInfo;

    [Header("FadeCanvas")]
    public CanvasGroup fadeCanvas;
    public float fadeDuration;

    private bool isFade;


    private void Start()
    {
        CameraController.Instance.canFreeMove = true;
        Transition(string.Empty, startSceneName.ToString());
    }


    /// <summary>
    /// ������ת��
    /// </summary>
    /// <param name="to"></param>
    public void SceneTransitionScene(string to)
    {


        foreach (var sceneInfo in SceneData.sceneInfoList)
        {
            if (sceneInfo.Name == to)
            {
                curSceneInfo = sceneInfo;
            }
        }

        Transition(SceneName.CurScene.ToString(), SceneName.CurScene.ToString());
    }

    /// <summary>
    /// �������ص�ͼ
    /// </summary>
    public void SceneTransitionMap()
    {
        CameraController.Instance.canFreeMove = true;
        curSceneInfo = null;
        Transition(SceneName.CurScene.ToString(), SceneName.Map.ToString());
    }


    /// <summary>
    /// ��ͼת��������
    /// </summary>
    /// <param name="sceneName"></param>
    public void MapTransitionScene(string sceneName)
    {
        CameraController.Instance.canFreeMove = false;

        foreach (var sceneInfo in SceneData.sceneInfoList)
        {
            if (sceneInfo.Name == sceneName)
            {
                curSceneInfo = sceneInfo;
            }
        }
        
        Transition(SceneName.Map.ToString(), SceneName.CurScene.ToString());
    }

    /// <summary>
    /// ��fromת����to����
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
    /// �첽ж�ؼ��س���
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
