using UnityEngine;
using System.Collections;

public class monsterController : MonoBehaviour {

    public Animator anim;
    public Transform player;
    public float distance;
    public SkinnedMeshRenderer skin;

	enum actions {atk, atk1, death, idle, idle1, idleTaunt, jumpAtk, run, walk, noAction};
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
            int rnd = Random.Range(0,3);
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
                    if(isDoing != actions.jumpAtk) {
                        isDoing = actions.jumpAtk;
                        doAction = actions.jumpAtk;
                        transform.LookAt(new Vector3(player.position.x,transform.position.y,player.position.z));
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
                case actions.atk:
                    if(anim.GetInteger("actions") != 0) {
                        anim.SetInteger("actions",0);
                    }
                    break;
                case actions.atk1:
                    if(anim.GetInteger("actions") != 1) {
                        anim.SetInteger("actions",1);
                    }
                    break;
                case actions.death:
                    if(anim.GetInteger("actions") != 2) {
                        anim.SetInteger("actions",2);
                    }
                    break;
                case actions.idle:
                    if(anim.GetInteger("actions") != 3) {
                        anim.SetInteger("actions", 3);
                    }
                    break;
                case actions.idleTaunt:
                    if(anim.GetInteger("actions") != 4) {
                        anim.SetInteger("actions",4);
                    }
                    break;
                case actions.jumpAtk:
                    if(anim.GetInteger("actions") != 6) {
                        anim.SetInteger("actions",6);
                    }
                    break;
                case actions.run:
                    if(anim.GetInteger("actions") != 7) {
                        anim.SetInteger("actions",7);
                    }
                    break;
                case actions.walk:
                    if(anim.GetInteger("actions") != 8) {
                        anim.SetInteger("actions",8);
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
        isDoing = actions.idle;
        doAction = actions.noAction;
    }

    IEnumerator Hit() {
        Time.timeScale = 0.02f;
        hitFeed = true;
        skin.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        skin.material.color = Color.white;
    }
}
