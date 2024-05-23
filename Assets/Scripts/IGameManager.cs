using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameManager {
    public T GetSystem<T>() where T : class;
}
