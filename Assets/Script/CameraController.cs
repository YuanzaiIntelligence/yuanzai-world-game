using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : Singleton<CameraController>
{
    public float moveDuration;

    [Header("����")]
    public SceneData sceneData;

    [SerializeField]private float max_X = 9;
    [SerializeField] private float max_Y = 5;
    [SerializeField] private float min_X = -9;
    [SerializeField] private float min_Y = -5;


    private void Start()
    {
        UpdateMaxMin();
    }

    public bool canFreeMove;

    public float dragSpeed = 0.01f;
    private Vector3 dragOrigin;
    private bool isDragging = false;

    void Update()
    {
        if (!canFreeMove) return;

        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 currentPos = Input.mousePosition;
            Vector3 difference = dragOrigin - currentPos;

            // ������λ��
            Vector3 newPosition = transform.position + new Vector3(difference.x, difference.y, 0) * dragSpeed;

            // Ӧ����ֵ����
            newPosition.x = Mathf.Clamp(newPosition.x, min_X, max_X);
            newPosition.y = Mathf.Clamp(newPosition.y, min_Y, max_Y);

            // �ƶ�����������ƺ��λ��
            transform.position = newPosition;

            // ������ʼ�϶�λ��
            dragOrigin = currentPos;
        }
    }

    public void MoveCamera(string targetName)
    {
        var targetPos = new Vector2(0, 0);
        foreach (var sceneInfo in sceneData.sceneInfoList)
        {
            if (sceneInfo.Name == targetName)
            {
                targetPos = new Vector2(sceneInfo.X, sceneInfo.Y);
                break;
            }
            else if (targetName == string.Empty)
            {
                break;
            }
        }
        StartCoroutine(Move(new Vector3(targetPos.x, targetPos.y, -10)));
    }

    private IEnumerator Move(Vector3 targetPos)
    {

        float result = Vector3.Distance(transform.position, targetPos);
        float speed = Mathf.Abs(result) / moveDuration;

        // ������Ŀ��λ�û���һ������ʱ�����ƶ�
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            // ʹ��Vector3.MoveTowards����ƽ���ƶ�
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                speed * Time.deltaTime
            );

            yield return null;
        }

        // ȷ������λ�þ�ȷ
        transform.position = targetPos;

    }


    private void UpdateMaxMin()
    {
        foreach (var item in sceneData.sceneInfoList)
        {
            max_X = Mathf.Max(max_X, item.X);
            max_Y = Mathf.Max(max_Y, item.Y);
            min_X = Mathf.Min(min_X, item.X);
            min_Y = Mathf.Min(min_Y, item.Y);
        }
    }

    
}
