using UnityEngine;
using System.Collections;

public class DamageField : MonoBehaviour {

    public float damageRate = 1.0f;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerStay(Collider other) {
        other.SendMessage("Damage", (damageRate * Time.fixedDeltaTime), SendMessageOptions.DontRequireReceiver);
    }
}
