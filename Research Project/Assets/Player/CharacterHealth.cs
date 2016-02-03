using UnityEngine;
using System.Collections;

public class CharacterHealth : MonoBehaviour {

    public float health = 1.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (health < 0.0f) {
            Destroy(gameObject);
        }
	}

    void Damage(float amount) {
        health -= amount;
    }

    void Kill() {
        SendMessage("Damage", Mathf.Infinity);
    }
}
