using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPhase {
    public void EnterPhase(PhaseSystem phaseSystem);
    public void ExitPhase(PhaseSystem phaseSystem);
}
