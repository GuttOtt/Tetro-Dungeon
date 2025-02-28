using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BaseUnitMarker : MonoBehaviour {
    [SerializeField] private BoxCollider2D boxCollider2D;
    private SPUM_Prefabs spum;
    public Cell cell;
    public CharacterBlock characterBlock;

    private Vector3 originalPosition;
    private Vector3 offset;
    private bool isDragging = false;

    public void Initialize(Cell cell, CharacterBlock characterBlock) {
        this.cell = cell;
        this.characterBlock = characterBlock;
        spum = Instantiate(characterBlock.Config.SPUM_Prefabs, transform);
        spum.transform.localPosition = new Vector2(0, -0.2f);
        spum.transform.localScale = new Vector2(-1, 1);

        // UnitRoot 자식을 찾아 SortingGroup의 SortingLayer 변경
        Transform unitRoot = spum.transform.Find("UnitRoot");
        if (unitRoot != null) {
            SortingGroup sortingGroup = unitRoot.GetComponent<SortingGroup>();
            if (sortingGroup != null) {
                sortingGroup.sortingLayerName = "Dragging";
            }
        }

        transform.SetParent(cell.transform);
        transform.localPosition = Vector3.zero;
    }
    
    private void OnMouseDown() {
        // 원래 위치와 마우스 오프셋 저장
        originalPosition = transform.position;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z);
    }

    private void OnMouseDrag() {
        // 드래그 중 마우스 위치 따라 이동
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z) + offset;
    }

    private void OnMouseUp() {
        // 드랍 시 marker의 위치가 characterBlock.Cells 내의 Cell 콜라이더에 포함되는지 확인
        bool found = false;
        foreach (var candidateCell in characterBlock.Cells) {
            BoxCollider2D cellCollider = candidateCell.GetComponent<BoxCollider2D>();
            if (cellCollider != null && cellCollider.OverlapPoint(transform.position)) {
                // 해당 Cell의 위치로 스냅
                transform.position = candidateCell.transform.position;
                transform.SetParent(candidateCell.transform);
                cell = candidateCell;
                found = true;
                break;
            }
        }
        // 유효한 Cell 위가 아니라면 원래 위치로 복귀
        if (!found) {
            transform.position = originalPosition;
        }
    }

    private void Update() {
        // 마우스 버튼 왼쪽이 눌렸을 때 OnMouseDown 호출
        if (Input.GetMouseButtonDown(0) && Utils.Pick<BaseUnitMarker>() == this) {
            OnMouseDown();
            isDragging = true;
        }
        // 드래그 중일 때 OnMouseDrag 호출
        if (isDragging && Input.GetMouseButton(0)) {
            OnMouseDrag();
        }
        // 마우스 버튼을 떼면 OnMouseUp 호출
        if (isDragging && Input.GetMouseButtonUp(0)) {
            OnMouseUp();
            isDragging = false;
        }
    }
}
