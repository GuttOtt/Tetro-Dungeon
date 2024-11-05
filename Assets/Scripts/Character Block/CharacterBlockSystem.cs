using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterBlockSystem : MonoBehaviour {
    private IGameManager _gameManager;

    private List<CharacterBlock> _characterBlocks = new List<CharacterBlock>();
    [SerializeField] private CharacterBlock _characterBlockPrefab;
    private CharacterBlock _selectedBlock;

    //For Debug
    [SerializeField] private CharacterBlockConfig _testConfig;

    private bool _isInputOn = true;

    void Awake() {
       
    }

    private void Start() {
        DebugOnly();
    }

    void Update() {
        SelectBlock();
        MoveSelectedBlock();
        UnSelectBlock();
        SpinBlock();
    }

    public void DebugOnly() {
        CreateCharacterBlock(_testConfig, 1);
        CreateCharacterBlock(_testConfig, 1);
    }

    public CharacterBlock CreateCharacterBlock(CharacterBlockConfig config, int currentLevel) {
        CharacterBlock newBlock = Instantiate(_characterBlockPrefab);
        newBlock.Init(config, _characterBlocks.Count, currentLevel);
        _characterBlocks.Add(newBlock);

        return newBlock;
    }

    #region Selection and Dragging Control
    private void SelectBlock() {
        if (!_isInputOn || !Input.GetMouseButton(0) || _selectedBlock != null) {
            return;
        }

        BlockPart selectedBlockPart = Utils.Pick<BlockPart>();
        if (selectedBlockPart == null) return;

        CharacterBlock selectedBlock = selectedBlockPart.CharacterBlock;
        if (selectedBlock == null) return;

        //���� ���� ���� �ϱ� ���� SortingLayer ����
        int draggingLayerID = SortingLayer.NameToID("Dragging");
        selectedBlock.ChangeSortingLayer(draggingLayerID);

        _selectedBlock = selectedBlock;
    }

    private void MoveSelectedBlock() {
        if (_selectedBlock == null) return;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = _selectedBlock.transform.position.z;
        _selectedBlock.transform.position = mousePosition;
    }

    private void UnSelectBlock() {
        if (!Input.GetMouseButtonUp(0) || _selectedBlock == null) return;

        //SortingLayer ������� �ǵ�����
        int illustLyaerID = SortingLayer.NameToID("Illust");
        _selectedBlock.ChangeSortingLayer(illustLyaerID);

        _selectedBlock = null;
    }

    private void SpinBlock() {
        if (_selectedBlock == null) return;

        bool isClockwise;
        if (Input.GetKeyDown(KeyCode.Q))
            isClockwise = false;
        else if (Input.GetKeyDown(KeyCode.E))
            isClockwise = true;
        else
            return;

        _selectedBlock.Spin(isClockwise);
    }
    #endregion

    private void TryPlace() {

    }
}
