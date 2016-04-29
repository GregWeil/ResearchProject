using UnityEngine;
using System.Collections;

public class CharacterHealth : MonoBehaviour {

    public float health = 1.0f;
    bool dead = false;

    Animator anim = null;
    float stunCooldown = 0f;

	// Use this for initialization
	void Start () {
        anim = GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        stunCooldown -= Time.deltaTime;
	    if ((health < 0.0f) && !dead) {
            dead = true;
            anim.SetTrigger("Die");
        }
	}

    public void Damage(float amount) {
        if (!dead) {
            health -= amount;
            if (stunCooldown < 0f) {
                SendMessage("Stun");
                anim.SetTrigger("Hurt");
                stunCooldown = 0.5f;
            }
        }
    }

    public void Kill() {
        if (!dead) {
            SendMessage("Damage", Mathf.Infinity);
        }
    }

    public bool Alive() {
        return !dead;
    }
}
