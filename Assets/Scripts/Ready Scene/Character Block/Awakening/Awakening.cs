using AYellowpaper.SerializedCollections;
using EnumTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class Awakening: ScriptableObject {
    [SerializeField]
    public AwakeningCondition _condition;

    public abstract void UpdateActivation(CharacterBlock characterBlock);
}

[Serializable]
public class AwakeningCondition {
    [SerializeField]
    private SerializedDictionary<SynergyTypes, int> _synergyDic;

    public bool IsSatisfied(CharacterBlock characterBlock) {
        List<Equipment> equipments = characterBlock.Equipments;

        Dictionary<SynergyTypes, int> equipmentSynergyCount = new Dictionary<SynergyTypes, int>();

        foreach (Equipment equipment in equipments) {
            List<SynergyTypes> synergies = equipment.Synergies;

            foreach (SynergyTypes synergy in synergies) {
                if (equipmentSynergyCount.ContainsKey(synergy))
                    equipmentSynergyCount[synergy]++;
                else
                    equipmentSynergyCount.Add(synergy, 1);
            }
        }
        
        foreach(SynergyTypes synergyType in _synergyDic.Keys) {
            if (!equipmentSynergyCount.ContainsKey(synergyType) ||
                equipmentSynergyCount[synergyType] < _synergyDic[synergyType])
                return false;
        }

        return true;
    }
}