using UnityEngine;
using System.Collections;

public class enemyController : MonoBehaviour
{

    public Animator anim;
    public Transform player;
    public GameObject playerBody;
    public GameObject EnemyBody;
    //public float distance;
    public SkinnedMeshRenderer skin;
    public Player playerScript;
    public float lightDmg = 10;
    public float mediumDmg = 20;
    public float heavyDmg = 30;

    public GameObject viewPoint;

    public float chaseDistance = 10f;
    public float fightDistance = 5f;


    private bool chasing, fighting = false;

    enum actions { idle, walk, atk, atk1, atk2, dashBack, dashForward, dashLeft, dashRight, hit, noAction };
    actions doAction;
    actions isDoing;

    bool hitFeed;
    int cdHitFeed;

    void Start()
    {
        EnemyBody = transform.gameObject;
        viewPoint = transform.GetChild(1).gameObject;
        //InvokeRepeating("ReactionsUpdate",1f,1f);
        InvokeRepeating("ReactionsUpdate", 1f, 1f);
        doAction = actions.idle;
        isDoing = actions.noAction;
        hitFeed = false;
        cdHitFeed = 0;
        player = GameObject.Find("PlayerPoint").transform;
        playerBody = GameObject.Find("Player");
        playerScript = GameObject.Find("Player").gameObject.GetComponent<Player>();
    }

    void ReactionsUpdate()
    {
        Debug.Log("reactionUpdate");
        if (fighting)
        {
            EnemyBody.transform.LookAt(playerBody.transform);
            int rnd = Random.Range(0, 6);
            switch (rnd)
            {
                case 0:
                    Debug.Log("ataca");
                    if (isDoing != actions.atk)
                    {
                        isDoing = actions.atk;
                        doAction = actions.atk;
                        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                    }
                    break;
                case 1:
                    if (isDoing != actions.atk1)
                    {
                        isDoing = actions.atk1;
                        doAction = actions.atk1;
                        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                    }
                    break;
                case 2:
                    if (isDoing != actions.atk2)
                    {
                        isDoing = actions.atk2;
                        doAction = actions.atk2;
                        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
                    }
                    break;
                case 3:
                    if (isDoing != actions.dashBack)
                    {
                        isDoing = actions.dashBack;
                        doAction = actions.dashBack;
                    }
                    break;
                case 4:
                    if (isDoing != actions.dashLeft)
                    {
                        isDoing = actions.dashLeft;
                        doAction = actions.dashLeft;
                    }
                    break;
                case 5:
                    if (isDoing != actions.dashRight)
                    {
                        isDoing = actions.dashRight;
                        doAction = actions.dashRight;
                    }
                    break;
            }
        }
    }

    void Update()
    {
        MoveController();

        if (hitFeed)
        {
            cdHitFeed++;
            if (cdHitFeed >= 10f)
            {
                hitFeed = false;
                cdHitFeed = 0;
                Time.timeScale = 1f;
            }
        }

        //if (isDoing == actions.noAction) {
        //    Quaternion rot = Quaternion.LookRotation(new Vector3(player.position.x,transform.position.y,player.position.z) - transform.position);
        //    transform.rotation = Quaternion.Slerp(transform.rotation,rot,Time.deltaTime * 10f);
        //}

        if (anim)
        {
            switch (doAction)
            {
                case actions.idle:
                    if (anim.GetInteger("actions") != 0)
                    {
                        anim.SetInteger("actions", 0);
                    }
                    break;
                case actions.walk:
                    if (anim.GetInteger("actions") != 1)
                    {
                        anim.SetInteger("actions", 1);
                    }
                    break;
                case actions.atk:
                    if (anim.GetInteger("actions") != 3)
                    {
                        anim.SetInteger("actions", 3);
                    }
                    break;
                case actions.atk1:
                    if (anim.GetInteger("actions") != 4)
                    {
                        anim.SetInteger("actions", 4);
                    }
                    break;
                case actions.atk2:
                    if (anim.GetInteger("actions") != 5)
                    {
                        anim.SetInteger("actions", 5);
                    }
                    break;
                case actions.dashForward:
                    if (anim.GetInteger("actions") != 6)
                    {
                        anim.SetInteger("actions", 6);
                    }
                    break;
                case actions.dashBack:
                    if (anim.GetInteger("actions") != 7)
                    {
                        anim.SetInteger("actions", 7);
                    }
                    break;
                case actions.dashLeft:
                    if (anim.GetInteger("actions") != 8)
                    {
                        anim.SetInteger("actions", 8);
                    }
                    break;
                case actions.dashRight:
                    if (anim.GetInteger("actions") != 9)
                    {
                        anim.SetInteger("actions", 9);
                    }
                    break;
                case actions.hit:
                    if (anim.GetInteger("actions") != 10)
                    {
                        anim.SetInteger("actions", 10);
                    }
                    break;
            }
        }
    }

    public void EndAction()
    {
        doAction = actions.idle;
        isDoing = actions.noAction;
    }

    void OnTriggerEnter(Collider coll)
    {
        if (coll.gameObject.name == "coll")
        {
            StartCoroutine("Hit");
            Debug.Log("teste");
        }
    }

    public void Parred()
    {
        isDoing = actions.hit;
        doAction = actions.hit;
    }


    void MoveController()
    {
        Debug.Log("teste");
        viewPoint.transform.LookAt(player.transform);
        RaycastHit hit;
        Debug.DrawRay(viewPoint.transform.position, viewPoint.transform.forward * 100, Color.green);
        Ray playerRay = new Ray(viewPoint.transform.position, viewPoint.transform.forward);
        if ((Physics.Raycast(playerRay, out hit) && hit.collider.tag == "Player") || chasing)
        {
            Debug.Log("Colidindo  " + hit.collider.tag);
            if (hit.distance < chaseDistance)
            {
                Debug.Log("Chase");
                chasing = true;
                if (hit.distance < fightDistance && hit.collider.tag == "Player")
                {
                    fighting = true;
                    //var attackType = Random.Range(1, 3);
                    //switch (attackType)
                    //{
                    //    case 0:
                    //        doAction = actions.atk;
                    //        isDoing = actions.atk;
                    //        break;
                    //    case 1:
                    //        doAction = actions.atk1;
                    //        isDoing = actions.atk1;
                    //        break;
                    //    case 2:
                    //        doAction = actions.atk2;
                    //        isDoing = actions.atk2;
                    //        break;
                    //}

                    Debug.Log("Fight");
                }
                else if (isDoing == actions.noAction)
                {
                    fighting = false;
                    Debug.Log("Fight Out");
                    ChasePlayer();
                }
                else
                {
                    Debug.Log("Nothing");
                    fighting = false;
                }
            }
            else
            {
                fighting = false;
                chasing = false;
                Debug.Log("back to start Position");
            }
        }
        Debug.DrawRay(transform.position, transform.forward, Color.red);
    }

    void ChasePlayer()
    {
        EnemyBody.GetComponent<NavMeshAgent>().SetDestination(playerBody.transform.position);
        doAction = actions.walk;
        //EnemyBody.transform.LookAt(player.transform);
    }

    IEnumerator Hit()
    {
        Time.timeScale = 0.02f;
        hitFeed = true;
        skin.material.color = Color.red;
        isDoing = actions.hit;
        doAction = actions.hit;
        yield return new WaitForSeconds(0.5f);
        int rnd = Random.Range(0, 4);
        switch (rnd)
        {
            case 0:
                if (isDoing != actions.dashBack)
                {
                    isDoing = actions.dashBack;
                    doAction = actions.dashBack;
                }
                break;
            case 1:
                if (isDoing != actions.dashLeft)
                {
                    isDoing = actions.dashLeft;
                    doAction = actions.dashLeft;
                }
                break;
            case 2:
                if (isDoing != actions.dashRight)
                {
                    isDoing = actions.dashRight;
                    doAction = actions.dashRight;
                }
                break;
            case 3:
                if (isDoing != actions.noAction)
                {
                    isDoing = actions.idle;
                    doAction = actions.noAction;
                }
                break;
        }
        skin.material.color = Color.black;
    }
}
