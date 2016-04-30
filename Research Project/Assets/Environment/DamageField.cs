using UnityEngine;
using System.Collections.Generic;

public class DamageField : MonoBehaviour {
    
    public float damageRate = 1.0f;
    public bool instant = false;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerStay(Collider other) {
        var damage = instant ? damageRate : (damageRate * Time.fixedDeltaTime);
        other.SendMessage("Damage", damage, SendMessageOptions.DontRequireReceiver);
    }
}
