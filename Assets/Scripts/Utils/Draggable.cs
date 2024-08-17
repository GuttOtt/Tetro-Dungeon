using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 originalPosition;
    private Transform originalParent;
    private Vector3 offset;
    private Camera mainCamera;
    public GameObject destination;  // 패널을 참조할 변수

    public Action<Draggable> EndDragAction;  // Draggable 객체를 전달하는 Action
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
        originalParent = transform.parent;
        Debug.Log("Draggable Start method called");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        offset = transform.position - GetMouseWorldPosition();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = GetMouseWorldPosition() + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDragAction?.Invoke(this);
    }

    public void ResetPosition()
    {
        transform.SetParent(originalParent);
        transform.position = originalPosition;
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

    public void SetEndDragAction(Action<Draggable> action)
    {
        EndDragAction = action;
        Debug.Log("EndDragAction is set.");
    }

    private void AdjustColliderToFitRenderer(BoxCollider collider)
    {
        collider.size = collider.size * colliderScaleFactor;
    }
}
