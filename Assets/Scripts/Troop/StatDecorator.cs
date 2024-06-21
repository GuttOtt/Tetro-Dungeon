using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatDecorator", menuName = "StatDecorator")]
public class StatDecorator : ScriptableObject{
    #region private members
    [SerializeField]
    private int _attack, _hp, _range;
    #endregion

    #region Properties
    public int Attack { get => _attack; }
    public int HP { get => _hp; }
    public int Range { get => _range; }
    #endregion

    public bool IsSatisfied(UnitConfig unitConfig) {
        return true;
    }

    public void Decorate(BaseUnit unit) {
        unit.ChangeAttack(_attack);
        unit.ChangeMaxHP(_hp);
    }
}
