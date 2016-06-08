using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public GameObject EnemyBody;
    public Animator anim;

    public float chaseDistance = 10f;
    public float fightDistance = 5f;


    private bool chasing, fighting = false;

    enum actions { idle, walk, atk, atk1, atk2, dashBack, dashForward, dashLeft, dashRight, hit, noAction };
    actions doAction;
    actions isDoing;

    public Rigidbody rb;
    public GameObject player;
    void Start()
    {
        EnemyBody = transform.parent.gameObject;
        player = GameObject.Find("Player");
        Debug.Log(EnemyBody);
        doAction = actions.idle;
        isDoing = actions.noAction;
        //rb = GetComponent<Rigidbody>();
        //rb.drag = 0.5F;
        //rb.angularDrag = 0.5F;
        transform.LookAt(player.transform);
    }
    void FixedUpdate()
    {
        AnimationsController(); // Control all animations for any actions
        MoveController(); // Control all moves of enemy
    }


    void Update()
    {

    }

    void MoveController()
    {
        transform.LookAt(player.transform);
        RaycastHit hit;
        Ray playerRay = new Ray(transform.position, transform.forward);
        if ((Physics.Raycast(playerRay, out hit) && hit.collider.tag == "Player") || chasing)
        {
            //Debug.Log("Distância: " + hit.distance);
            if (hit.distance < chaseDistance)
            {
                Debug.Log("Chase");
                chasing = true;
                ChasePlayer();
                if (hit.distance < fightDistance)
                {
                    fighting = true;
                    var attackType = Random.Range(1, 3);
                    switch (attackType)
                    {
                        case 0:
                            doAction = actions.atk;
                            isDoing = actions.atk;
                            break;
                        case 1:
                            doAction = actions.atk1;
                            isDoing = actions.atk1;
                            break;
                        case 2:
                            doAction = actions.atk2;
                            isDoing = actions.atk2;
                            break;
                    }

                    Debug.Log("Fight");
                }
                else
                {
                    fighting = false;
                    Debug.Log("Fight Out");
                }
            }
            else
            {
                chasing = false;
                Debug.Log("back to start Position");
            }
        }
        Debug.DrawRay(transform.position, transform.forward, Color.red);
    }

    void ChasePlayer()
    {
        EnemyBody.GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
        doAction = actions.walk;
    }

    void AnimationsController()
    {
        if (anim)
        {
            switch (doAction)
            {
                case actions.idle:
                    anim.SetInteger("actions", 0);
                    break;
                case actions.walk:
                    anim.SetInteger("actions", 1);
                    break;
                case actions.atk:
                    anim.SetInteger("actions", 3);
                    break;
                case actions.atk1:
                    anim.SetInteger("actions", 4);
                    break;
                case actions.atk2:
                    anim.SetInteger("actions", 5);
                    break;
                case actions.dashForward:
                    anim.SetInteger("actions", 6);
                    break;
                case actions.dashBack:
                    anim.SetInteger("actions", 7);
                    break;
                case actions.dashLeft:
                    anim.SetInteger("actions", 8);
                    break;
                case actions.dashRight:
                    anim.SetInteger("actions", 9);
                    break;
                case actions.hit:
                    anim.SetInteger("actions", 10);
                    break;
            }
        }
    }

    public void EndAction()
    {
        doAction = actions.idle;
        isDoing = actions.noAction;
    }
    //void OnCollisionEnter(Collision collision)
    //{
    //    foreach (ContactPoint contact in collision.contacts)
    //    {
    //        Debug.DrawRay(contact.point, contact.normal, Color.green, 2, false);
    //    }
    //}
}
