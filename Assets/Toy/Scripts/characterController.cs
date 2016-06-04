using UnityEngine;
using System.Collections;

public class characterController : MonoBehaviour {

    public Animator anim;
    public GameObject cameraMain;
    public Transform enemy;
    public Transform monster;
    public Transform drone;
    
    public SkinnedMeshRenderer skin;
    public GameObject hitbox;
    public GameObject parry;

	enum actions {idle, walk, walkBack, strafeRight, strafeLeft, combo, atk, dashBack, dashForward, dashLeft, dashRight, hit, noAction};
    actions doAction;
    actions isDoing;

    Transform target;
    bool atkForward;
    bool combo;
    float cdCombo;
    bool hitFeed;
    int cdHitFeed;

    void Start() {
        doAction = actions.idle;
        isDoing = actions.noAction;
        atkForward = false;
        combo = false;
        cdCombo = 0;
        hitFeed = false;
        cdHitFeed = 0;
        hitbox.SetActive(true);
        parry.SetActive(false);
        target = enemy;
    }

	void Update () {
        if(Input.GetKeyDown(KeyCode.Joystick1Button5)) {
            combo = true;
            cdCombo = 0;
        }
        if (combo) {
            cdCombo += Time.deltaTime;
            if(cdCombo >= 0.2f) {
                combo = false;
                cdCombo = 0;
            }
        }

        if(Input.GetKeyDown(KeyCode.Joystick1Button4) && isDoing != actions.hit) {
            StartCoroutine("Parry");
        }

        if(hitFeed) {
            cdHitFeed++;
            if(cdHitFeed >= 10f) {
                hitFeed = false;
                cdHitFeed = 0;
                Time.timeScale = 1f;
            }
        }
        if(Input.GetKeyDown(KeyCode.Joystick1Button0) || Input.GetKeyDown(KeyCode.Joystick1Button9)) {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            float close = 9999;
            for (int i = 0; i < enemies.Length; i++){
                if (close > Vector3.Distance(enemies[i].transform.position,transform.position)) {
                    close = Vector3.Distance(enemies[i].transform.position, transform.position);
                    if (target != enemies[i].transform) {
                        target = enemies[i].transform;
                    }
                }
            }

            RaycastHit hit;
            if(Physics.Raycast(cameraMain.transform.position, target.position - cameraMain.transform.position,out hit)) {
                if(hit.transform == target) {
                    atkForward = !atkForward;
                    cameraMain.GetComponent<cameraOrbitController>().LockCamera(!atkForward,target);
                }
            }
        }
        else if(atkForward) {
            RaycastHit hit;
            if(Physics.Raycast(cameraMain.transform.position, target.position - cameraMain.transform.position,out hit)) {
                if(hit.transform != target && hit.transform != transform) {
                    atkForward = false;
                    cameraMain.GetComponent<cameraOrbitController>().LockCamera(!atkForward,target);
                }
            }
        }

        if(isDoing != actions.noAction) {
            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
                if(!atkForward) {
                    Quaternion rot = Quaternion.LookRotation(new Vector3(cameraMain.transform.position.x,transform.position.y,cameraMain.transform.position.z) - transform.position);
                    rot *= Quaternion.Euler(0, Mathf.Atan2(-Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical")) * Mathf.Rad2Deg, 0);
                    transform.rotation = Quaternion.Slerp(transform.rotation,rot,Time.deltaTime * 10f);
                }
            }
        }

        if (isDoing == actions.noAction) {
            if(Input.GetKeyDown(KeyCode.Joystick1Button1)) {
                if(atkForward) {
                    if (Input.GetAxis("Vertical") >= 0.5f) {
                        transform.LookAt(new Vector3(target.position.x,transform.position.y,target.position.z));
                        doAction = actions.dashForward;
                        isDoing = actions.dashForward;
                    }
                    else if (Input.GetAxis("Vertical") <= -0.5f) {
                        transform.LookAt(new Vector3(target.position.x,transform.position.y,target.position.z));
                        doAction = actions.dashBack;
                        isDoing = actions.dashBack;
                    }
                    else if (Input.GetAxis("Horizontal") <= -0.5f) {
                        transform.LookAt(new Vector3(target.position.x,transform.position.y,target.position.z));
                        doAction = actions.dashLeft;
                        isDoing = actions.dashLeft;
                    }
                    else if (Input.GetAxis("Horizontal") >= 0.5f) {
                        transform.LookAt(new Vector3(target.position.x,transform.position.y,target.position.z));
                        doAction = actions.dashRight;
                        isDoing = actions.dashRight;
                    }
                    else {
                        doAction = actions.dashBack;
                        isDoing = actions.dashBack;
                    }
                }
                else {
                    doAction = actions.dashForward;
                    isDoing = actions.dashForward;
                }
            }
            else if(Input.GetKeyDown(KeyCode.Joystick1Button5)) {
                doAction = actions.combo;
                isDoing = actions.combo;
                if (atkForward) {
                    transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
                }
            }
            else if(Input.GetAxis("Fire1") <= -0.1f) {
                doAction = actions.atk;
                isDoing = actions.atk;
                if (atkForward) {
                    transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));
                }
            }
            else if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) {
                if(atkForward) {
                    if (Input.GetAxis("Vertical") >= 0.5f) {
                        doAction = actions.walk;
                        Quaternion rot = Quaternion.LookRotation(transform.position - new Vector3(target.position.x,transform.position.y,target.position.z));
                        rot *= Quaternion.Euler(0, Mathf.Atan2(-Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical")) * Mathf.Rad2Deg, 0);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
                    }
                    else if (Input.GetAxis("Vertical") <= -0.5f) {
                        doAction = actions.walkBack;
                        Quaternion rot = Quaternion.LookRotation(new Vector3(target.position.x,transform.position.y,target.position.z) - transform.position);
                        rot *= Quaternion.Euler(0, Mathf.Atan2(-Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical")) * Mathf.Rad2Deg, 0);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
                    }
                    else if (Input.GetAxis("Horizontal") <= -0.5f) {
                        doAction = actions.strafeRight;
                        Quaternion rot = Quaternion.LookRotation(transform.position - new Vector3(target.position.x,transform.position.y,target.position.z));
                        rot *= Quaternion.Euler(0, Mathf.Atan2(-Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical")) * Mathf.Rad2Deg + 90, 0);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
                    }
                    else if (Input.GetAxis("Horizontal") >= 0.5f) {
                        doAction = actions.strafeLeft;
                        Quaternion rot = Quaternion.LookRotation(transform.position - new Vector3(target.position.x,transform.position.y,target.position.z));
                        rot *= Quaternion.Euler(0, Mathf.Atan2(-Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical")) * Mathf.Rad2Deg - 90, 0);
                        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
                    }
                    //transform.LookAt(new Vector3(target.position.x,transform.position.y,target.position.z));
                }
                else {
                    doAction = actions.walk;
                    Quaternion rot = Quaternion.LookRotation(new Vector3(cameraMain.transform.position.x,transform.position.y,cameraMain.transform.position.z) - transform.position);
                    rot *= Quaternion.Euler(0, Mathf.Atan2(-Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical")) * Mathf.Rad2Deg, 0);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
                }
            }
            else {
                doAction = actions.idle;
            }
        }

        if(anim) {
            switch (doAction){
                case actions.idle:
                    if(anim.GetInteger("actions") != 0) {
                        anim.SetInteger("actions",0);
                    }
                    break;
                case actions.walk:
                    if(anim.GetInteger("actions") != 1) {
                        anim.SetInteger("actions",1);
                    }
                    break;
                case actions.combo:
                    if(anim.GetInteger("actions") != 3) {
                        anim.SetInteger("actions",3);
                    }
                    break;
                case actions.atk:
                    if(anim.GetInteger("actions") != 4) {
                        anim.SetInteger("actions",4);
                    }
                    break;
                case actions.dashForward:
                    if(anim.GetInteger("actions") != 6) {
                        anim.SetInteger("actions",6);
                    }
                    break;
                case actions.dashBack:
                    if(anim.GetInteger("actions") != 7) {
                        anim.SetInteger("actions",7);
                    }
                    break;
                case actions.dashLeft:
                    if(anim.GetInteger("actions") != 8) {
                        anim.SetInteger("actions",8);
                    }
                    break;
                case actions.dashRight:
                    if(anim.GetInteger("actions") != 9) {
                        anim.SetInteger("actions",9);
                    }
                    break;
                case actions.hit:
                    if(anim.GetInteger("actions") != 10) {
                        anim.SetInteger("actions",10);
                    }
                    break;
                case actions.walkBack:
                    if(anim.GetInteger("actions") != 11) {
                        anim.SetInteger("actions",11);
                    }
                    break;
                case actions.strafeRight:
                    if(anim.GetInteger("actions") != 12) {
                        anim.SetInteger("actions",12);
                    }
                    break;
                case actions.strafeLeft:
                    if(anim.GetInteger("actions") != 13) {
                        anim.SetInteger("actions",13);
                    }
                    break;
            }
        }
	}

    public void EndAction() {
        doAction = actions.idle;
        isDoing = actions.noAction;
    }

    public void ContinueCombo() {
        if(!combo) {
            EndAction();
        }
        else if (atkForward) {
            transform.LookAt(new Vector3(target.position.x,transform.position.y,target.position.z));
        }
    }

	void OnTriggerEnter(Collider coll) {
        if(coll.gameObject.layer == LayerMask.NameToLayer("weapon") && hitbox.activeSelf) {
            StartCoroutine("Hit");
        }
        else if(coll.gameObject.layer == LayerMask.NameToLayer("weapon") && parry.activeSelf) {
            coll.BroadcastMessage("Parred");
        }
    }

    IEnumerator Parry() {
        hitbox.SetActive(false);
        parry.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        hitbox.SetActive(true);
        parry.SetActive(false);
    }

    IEnumerator Hit() {
        Time.timeScale = 0.2f;
        skin.material.color = Color.red;
        isDoing = actions.hit;
        doAction = actions.hit;
        hitFeed = true;
        yield return new WaitForSeconds(0.1f);
        skin.material.color = Color.white;
    }
}
