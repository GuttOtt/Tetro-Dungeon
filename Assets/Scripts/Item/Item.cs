using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using System.Linq;

public class Item : ScriptableObject {
    [SerializeField]
    private string _name;

    [SerializeField]
    protected TArray<bool> _shape = new TArray<bool>(5, 5);

    [SerializeField]
    private string _description;

    public bool[,] Shape { get => _shape.GetArray<bool>(); }
    public string Name { get => _name; }
    public string Description { get => _description; }


    public bool IsSatisfiedBy(bool[,] array) {
        if (Shape.GetLength(0) != array.GetLength(0) || Shape.GetLength(1) != array.GetLength(1)) {
            Debug.LogError("비교하려는 배열의 크기가 서로 맞지 않습니다");
            return false;
        }

        for (int i = 0; i < Shape.GetLength(0); i++) { 
            for (int j = 0; j < Shape.GetLength(1); j++) {
                if (Shape[i, j] == true && array[i, j] == false)
                    return false;
            }
        }

        return true;
    }

    public virtual void OnBattleStartEffect(TurnContext turnContext) {
        //자식 클래스에서 세부 구현
    }

    public virtual void OnSatisfiedEffect(TurnContext turnContext) {
        //자식 클래스에서 세부 구현
    }

    protected List<IUnit> GetUnitsOnShape(Board board) {
        List<IUnit> units = new List<IUnit>();

        for (int i = 0; i < Shape.GetLength(0); i++) {
            for (int j = 0; j < Shape.GetLength(1); j++) {
                IUnit unit = board.GetCell(i,j).Unit;
                if (Shape[i,j] == true && unit != null) {
                    units.Add(unit);
                }
            }
        }

        return units.ToList();
    }
}
