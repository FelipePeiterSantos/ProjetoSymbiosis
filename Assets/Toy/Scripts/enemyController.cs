using UnityEngine;
using System.Collections;

public class enemyController : MonoBehaviour {

    public Animator anim;
    public Transform player;
    public float distance;
    public SkinnedMeshRenderer skin;

	enum actions {idle, walk, atk, atk1, atk2, dashBack, dashForward, dashLeft, dashRight, hit, noAction};
    actions doAction;
    actions isDoing;

    bool hitFeed;
    int cdHitFeed;

	void Start () {
        InvokeRepeating("ReactionsUpdate",1.5f,1.5f);
	    doAction = actions.idle;
        isDoing = actions.noAction;
        hitFeed = false;
        cdHitFeed = 0;
	}
	
    void ReactionsUpdate() {
        if(Vector3.Distance(transform.position,player.position) <= distance && isDoing == actions.noAction) {
            int rnd = Random.Range(0,6);
            switch (rnd)
            {
                case 0:
                    if(isDoing != actions.atk) {
                        isDoing = actions.atk;
                        doAction = actions.atk;
                        transform.LookAt(new Vector3(player.position.x,transform.position.y,player.position.z));
                    }
                    break;
                case 1:
                    if(isDoing != actions.atk1) {
                        isDoing = actions.atk1;
                        doAction = actions.atk1;
                        transform.LookAt(new Vector3(player.position.x,transform.position.y,player.position.z));
                    }
                    break;
                case 2:
                    if(isDoing != actions.atk2) {
                        isDoing = actions.atk2;
                        doAction = actions.atk2;
                        transform.LookAt(new Vector3(player.position.x,transform.position.y,player.position.z));
                    }
                    break;
                case 3:
                    if(isDoing != actions.dashBack) {
                        isDoing = actions.dashBack;
                        doAction = actions.dashBack;
                    }
                    break;
                case 4:
                    if(isDoing != actions.dashLeft) {
                        isDoing = actions.dashLeft;
                        doAction = actions.dashLeft;
                    }
                    break;
                case 5:
                    if(isDoing != actions.dashRight) {
                        isDoing = actions.dashRight;
                        doAction = actions.dashRight;
                    }
                    break;
            }            
        }
    }

	void Update () {
        if(hitFeed) {
            cdHitFeed++;
            if(cdHitFeed >= 10f) {
                hitFeed = false;
                cdHitFeed = 0;
                Time.timeScale = 1f;
            }
        }

        if (isDoing == actions.noAction) {
            Quaternion rot = Quaternion.LookRotation(new Vector3(player.position.x,transform.position.y,player.position.z) - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation,rot,Time.deltaTime * 10f);
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
                case actions.atk:
                    if(anim.GetInteger("actions") != 3) {
                        anim.SetInteger("actions",3);
                    }
                    break;
                case actions.atk1:
                    if(anim.GetInteger("actions") != 4) {
                        anim.SetInteger("actions", 4);
                    }
                    break;
                case actions.atk2:
                    if(anim.GetInteger("actions") != 5) {
                        anim.SetInteger("actions",5);
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
            }
        }
	}

    public void EndAction() {
        doAction = actions.idle;
        isDoing = actions.noAction;
    }

	void OnTriggerEnter(Collider coll) {
        if(coll.gameObject.name == "coll") {
            StartCoroutine("Hit");
        }
    }

    public void Parred() {
        isDoing = actions.hit;
        doAction = actions.hit;
    }

    IEnumerator Hit() {
        Time.timeScale = 0.02f;
        hitFeed = true;
        skin.material.color = Color.red;
        isDoing = actions.hit;
        doAction = actions.hit;
        yield return new WaitForSeconds(0.1f);
        skin.material.color = Color.black;
    }
}
