using UnityEngine;
using System.Collections;

public class droneController : MonoBehaviour {

    public Transform player;
    public MeshRenderer skin;
    public Rigidbody rig;
    public Animator anim;
    public GameObject bulletPf;
    public Transform positionInst;

    enum actions {idle, chargingAtk, dashBack, dashLeft, dashRight, noAction};
    actions doAction;
    actions isDoing;

    bool hitFeed;
    int cdHitFeed;

	void Start () {
        InvokeRepeating("ReactionsUpdate",2f,2f);
        doAction = actions.idle;
        isDoing = actions.noAction;
	    hitFeed = false;
        cdHitFeed = 0;
	}
	
    void ReactionsUpdate() {
        if(Vector3.Distance(transform.position,player.position) <= 6 && isDoing == actions.noAction) {
            int rnd = Random.Range(0,3);
            switch (rnd)
            {
                case 0:
                    if(isDoing != actions.idle) {
                        isDoing = actions.noAction;
                        doAction = actions.idle;
                    }
                    break;
                case 1:
                    if(isDoing != actions.dashLeft) {
                        isDoing = actions.noAction;
                        doAction = actions.dashLeft;
                    }
                    break;
                case 2:
                    if(isDoing != actions.dashRight) {
                        isDoing = actions.noAction;
                        doAction = actions.dashRight;
                    }
                    break;
            }
        }
        else if (Vector3.Distance(transform.position,player.position) <= 20 && isDoing == actions.noAction) {
            if(isDoing != actions.chargingAtk) {
                isDoing = actions.chargingAtk;
                doAction = actions.chargingAtk;
            }
        }
    }

	void Update () {
        Quaternion rot = Quaternion.LookRotation(new Vector3(player.position.x,transform.position.y,player.position.z) - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation,rot,Time.deltaTime * 10f);
        switch (doAction){
            case actions.idle:
                break;
            case actions.chargingAtk:
                anim.SetBool("charge", true);          
                break;
            case actions.dashBack:
                rig.velocity = transform.forward * -10;
                EndAction();
                break;
            case actions.dashLeft:
                rig.velocity = transform.right * -10;
                EndAction();
                break;
            case actions.dashRight:
                rig.velocity = transform.right * 10;
                EndAction();
                break;
        }

        if(hitFeed) {
            cdHitFeed++;
            if(cdHitFeed >= 10f) {
                hitFeed = false;
                cdHitFeed = 0;
                Time.timeScale = 1f;
            }
        }
	}

    void OnTriggerEnter(Collider coll) {
        if(coll.gameObject.name == "coll") {
            StartCoroutine("Hit");
            rig.velocity = transform.forward * -4;
        }
    }

    public void EndAction() {
        doAction = actions.idle;
        isDoing = actions.noAction;
    }

    public void Attack() {
        anim.SetBool("charge", false);
        Instantiate(bulletPf,positionInst.position + transform.forward,Quaternion.identity);
        EndAction();
    }

    IEnumerator Hit() {
        Time.timeScale = 0.02f;
        hitFeed = true;
        skin.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        skin.material.color = Color.white;
    }
}
