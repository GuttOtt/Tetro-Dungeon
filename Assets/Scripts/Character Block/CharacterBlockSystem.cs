using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBlockSystem : MonoBehaviour {
    private IGameManager _gameManager;

    private List<CharacterBlock> _characterBlocks = new List<CharacterBlock>();
    [SerializeField] private CharacterBlock _characterBlockPrefab;

    [SerializeField] private CharacterBlockConfig _testConfig;

    private bool _isInputOn = true;

    void Awake() {
       
    }

    private void Start() {
        DebugOnly();
    }

    void Update() {
        
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
}
