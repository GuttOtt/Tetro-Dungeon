using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 originalPosition;
    private Vector3 offset;
    private Camera mainCamera;
    public GameObject destination;  // 패널을 참조할 변수
    [SerializeField] private float colliderScaleFactor = 0.5f;  // Collider 축소 비율
    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
        }

        // Collider 설정 및 자동 조정
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();  // 3D 오브젝트일 경우
        }

        AdjustColliderToFitRenderer(boxCollider);

        // 오브젝트의 원래 위치를 저장합니다.
        originalPosition = transform.position;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("Drag Started");
        offset = transform.position - GetMouseWorldPosition();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        transform.position = GetMouseWorldPosition() + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("Drag Ended");

        if (destination != null)
        {
            // Destination 패널의 RectTransform을 가져옵니다.
            RectTransform panelRectTransform = destination.GetComponent<RectTransform>();

            if (RectTransformUtility.RectangleContainsScreenPoint(panelRectTransform, Input.mousePosition, mainCamera))
            {
                // 드롭된 위치가 Destination 패널 내에 있을 때
                Debug.Log("Dropped on Destination Panel");

                // 패널의 중심에 오브젝트를 배치합니다.
                transform.position = panelRectTransform.position;
            }
            else
            {
                // 드롭된 위치가 패널 외부일 때, 원래 위치로 되돌립니다.
                transform.position = originalPosition;
            }
        }
        else
        {
            // Destination이 설정되지 않았을 경우에도 원래 위치로 되돌립니다.
            transform.position = originalPosition;
            Debug.LogWarning("Destination panel is not set.");
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mainCamera.WorldToScreenPoint(transform.position).z;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }

    public void SetDestination(GameObject o)
    {
        destination = o;
    }

    private void AdjustColliderToFitRenderer(BoxCollider collider)
    {
        collider.size = collider.size * colliderScaleFactor;
    }
}
