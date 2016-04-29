using UnityEngine;
using System.Collections;

public class CharacterHealth : MonoBehaviour {

    public float health = 1.0f;
    bool dead = false;

    Animator anim = null;
    float animCooldown = 0f;

	// Use this for initialization
	void Start () {
        anim = GetComponentInChildren<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        animCooldown -= Time.deltaTime;
	    if ((health < 0.0f) && !dead) {
            dead = true;
            anim.SetTrigger("Die");
            Destroy(gameObject, 1f);
        }
	}

    public void Damage(float amount) {
        if (!dead) {
            health -= amount;
            if (animCooldown < 0f) {
                anim.SetTrigger("Hurt");
                animCooldown = 0.4f;
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
