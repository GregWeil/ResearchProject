using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    CharacterMovement movement = null;
    CharacterHealth health = null;
    Animator anim = null;

    public GameObject damageField = null;

    float stun = 0f;
    bool attack = false;

	// Use this for initialization
	void Start () {
        movement = GetComponent<CharacterMovement>();
        health = GetComponent<CharacterHealth>();
        anim = GetComponentInChildren<Animator>();
	}

    void FixedUpdate () {

        ///Attack
        damageField.SetActive(false);
        if (attack && health.Alive() && movement.getGrounded() && !(stun > 0f)) {
            stun = 0.5f;
            anim.SetTrigger("Attack");
            damageField.SetActive(true);
        }
        attack = false;

    }
	
	// Update is called once per frame
	void Update () {
        stun -= Time.deltaTime;

        ///Update desired movement
        Vector3 move = Camera.main.transform.TransformDirection(Vector3.right);
        move.y = 0;
        move.Normalize();
        move = (Input.GetAxis("Horizontal") * move) + (Input.GetAxis("Vertical") * new Vector3(-move.z, 0, move.x));
        if (move.sqrMagnitude > 1.0f) move.Normalize();
        if (!health.Alive() || (stun > 0f)) move = Vector3.zero;
        movement.setMovement(move);

        //Try to jump
        if (health.Alive() && !(stun > 0f)) {
            movement.setJump(Input.GetButton("Jump"));
        }

        attack = Input.GetButtonDown("Fire1");

    }

    void Stun () {
        stun = 0.2f;
    }
}

public class PlayerModule : StateMachineUtilities.Modules.Module {

    [StateMachineUtilities.Modules.Method("Player/is alive")]
    public static bool isAlive() {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return false;
        return player.GetComponentInChildren<CharacterHealth>().Alive();
    }

    [StateMachineUtilities.Modules.Method("Player/get player")]
    public static GameObject getPlayerGameObject() {
        return GameObject.FindGameObjectWithTag("Player");
    }
}
