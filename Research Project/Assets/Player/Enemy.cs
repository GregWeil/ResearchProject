using UnityEngine;
using StateMachineUtilities;

public class EnemyModule : Modules.Module {

    [Modules.Method("Enemies/movement towards")]
    public static Vector3 movementTowards(GameObject self, Vector3 target) {
        return Vector3.Scale((target - self.transform.position), new Vector3(1, 0, 1)).normalized;
    }

}