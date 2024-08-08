using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class CombinationPanel : MonoBehaviour, IDropHandler
{
    private List<Draggable> droppedCards = new List<Draggable>();

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent<Draggable>(out var draggable))
        {
            RectTransform panelRectTransform = transform as RectTransform;

            // 드래그된 위치가 Combination Panel 내에 있을 때만 처리
            if (RectTransformUtility.RectangleContainsScreenPoint(panelRectTransform, Input.mousePosition, Camera.main))
            {
                Debug.Log("Card dropped on Combination Panel");

                // 카드가 CombinationPanel로 드롭된 경우, 패널의 자식으로 설정
                draggable.transform.SetParent(transform, true);
                draggable.transform.localPosition = Vector3.zero;

                // 드롭된 카드를 리스트에 추가

                // 조합 시도
                TryCombine();
            }
            else
            {
                Debug.Log("Card dropped outside Combination Panel");
                draggable.ResetPosition();  // 패널 밖에 드롭된 경우 원래 위치로 되돌림
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Draggable"))
        {
            Debug.Log("Object entered Combination Panel area");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Draggable") && Input.GetMouseButtonUp(0))
        {
            Debug.Log("Object dropped on Combination Panel");

            // 드래그된 오브젝트를 패널의 자식으로 설정
            other.transform.SetParent(transform, true);
            other.transform.localPosition = Vector3.zero;

            // 드롭된 오브젝트에 대한 추가 처리 로직

        }
    }
    private void TryCombine()
    {
        BlockCard selectedBlockCard = null;
        UnitConfig selectedUnitConfig = null;

        // 드롭된 카드 리스트에서 BlockCard와 UnitConfig를 찾음
        foreach (var card in droppedCards)
        {
            BlockCard blockCard = card.GetComponentInParent<BlockCard>();
            UnitConfig unitConfig = card.GetComponentInParent<UnitDrawer>().UnitConfig;

            if (blockCard != null)
            {
                selectedBlockCard = blockCard;
            }
            else if (unitConfig != null)
            {
                selectedUnitConfig = unitConfig;
            }
        }

        // 둘 다 선택되었을 때 조합 처리
        if (selectedBlockCard != null && selectedUnitConfig != null)
        {
            Debug.Log("Combining cards");
            // 조합 로직 수행 (예: 새로운 카드 생성 등)
            CombineCards(selectedBlockCard, selectedUnitConfig);

            // 조합 후 카드들 제거
            droppedCards.Clear();
        }
    }

    private void CombineCards(BlockCard blockCard, UnitConfig unitConfig)
    {
        // 조합된 카드 생성 로직 (예: 새로운 카드 생성)
        Debug.Log($"Combined {blockCard} with {unitConfig}");
        // 여기서 새로운 카드를 생성하거나 추가 작업을 수행합니다.
    }
}
