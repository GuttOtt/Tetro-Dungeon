using UnityEngine;
[SerializeField]
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UnitConfig", order = 0)]
public class UnitConfig : ScriptableObject
{
    [SerializeField]
    private Sprite _sprite;

    [SerializeField]
    private int _maxHp, _maxMP, _attack, _range;

    [SerializeField]
    private string _unitTypeString;

    public Sprite Sprite { get => _sprite; }
    public int MaxHP { get => _maxHp; }
    public int MaxMP { get => _maxMP; }
    public int Attack { get => _attack; }
    public int Range { get => _range; }
    public string UnitTypeString { get => _unitTypeString; }
}