using UnityEngine;
using System.Collections;

public class Key : MonoBehaviour {

    public Door door;

    bool used = false;
    Vector3 targetPos = Vector3.zero;
    Quaternion targetRot = Quaternion.identity;
    float targetTime = 0f;
    Vector3 speed = Vector3.zero;
	
    void Start () {
        targetPos = transform.localPosition;
        targetRot = transform.localRotation;
        StartCoroutine("hover");
    }

	// Update is called once per frame
	void Update () {
        if (!used) {
            transform.localRotation *= Quaternion.AngleAxis((30f * Time.deltaTime), Vector3.up);
            targetRot = transform.localRotation;
        }
        if (targetTime > 0f) {
            var distPrev = Vector3.Distance(transform.localPosition, targetPos);
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPos, ref speed, targetTime);
            if (distPrev > 0f) {
                var distNew = Vector3.Distance(transform.localPosition, targetPos);
                transform.localRotation = Quaternion.Lerp(targetRot, transform.localRotation, (distNew / distPrev));
            } else {
                transform.localRotation = targetRot;
            }
        } else {
            speed = Vector3.zero;
        }
    }

    void OnTriggerEnter(Collider col) {
        if (col.CompareTag("Player") && !used) {
            used = true;
            StartCoroutine("activate");
        }
    }

    IEnumerator hover() {
        Vector3 basePos = transform.localPosition;
        bool up = true;
        while (!used) {
            targetTime = 5f;
            targetPos = basePos;
            if (up) targetPos.y += 0.25f;
            up = !up;
            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator activate() {
        transform.parent = null;
        Transform doorPanel = door.transform.Find("Door").transform;
        float height = 0.0225f;
        float side = 1f;

        targetTime = 0.15f;
        targetPos = transform.localPosition + (1f * Vector3.up);
        yield return new WaitUntil(() => (Vector3.Distance(transform.localPosition, targetPos) < 0.1f));
        yield return new WaitForSeconds(0.25f);

        targetTime = 0.75f;
        float distance = 0.01f;
        targetPos = door.transform.TransformPoint(new Vector3(distance, 0, height));
        var altPos = door.transform.TransformPoint(new Vector3(-distance, 0, height));
        if (Vector3.Distance(transform.localPosition, altPos) < Vector3.Distance(transform.localPosition, targetPos)) {
            side = -1f;
            targetPos = altPos;
        }
        Vector3 keyPos = new Vector3((0.004f * side), 0, 0.001f);
        Quaternion keyRot = Quaternion.Euler((90 * side), 90 - (90 * side), 0);
        targetRot = (door.transform.rotation * keyRot);
        yield return new WaitUntil(() => (Vector3.Distance(transform.localPosition, targetPos) < 0.1f));
        yield return new WaitForSeconds(0.2f);

        targetTime = 0.2f;
        do {
            targetPos = doorPanel.TransformPoint(keyPos);
            targetRot = (doorPanel.rotation * keyRot);
            yield return new WaitForSeconds(0f);
        } while (Vector3.Distance(transform.localPosition, targetPos) > 0.1f);

        if (transform.parent != null) speed = transform.parent.TransformVector(speed);
        speed = doorPanel.InverseTransformVector(speed);
        transform.parent = doorPanel;

        targetPos = keyPos;
        targetRot = keyRot;
        targetTime = 1f;

        door.unlockDoor(true);
    }

}
