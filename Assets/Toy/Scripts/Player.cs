using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float life, maxLife, stamina, maxStamina;
    public float hpRec, stRec;
    public float dmgReduction;

    private Image Hp, St;
    // Use this for initialization
    void Start()
    {
        Hp = GameObject.Find("HP").GetComponent<Image>();
        St = GameObject.Find("STAMINA").GetComponent<Image>();
    }

    public void Hit(float dmg)
    {
        life -= (dmg - dmgReduction);
        Debug.Log(life);
        if (life <= 0)
        {
            Debug.Log("Player Dead");
            Hp.transform.localScale = new Vector3(0, 1f, 1);
        }
        else
        {
            Hp.transform.localScale = new Vector3((life / maxLife), 1f, 1);
        }
    }

    public void Stamina(int weapon, int attack)
    {
        if (weapon == 1)
        {
            switch (attack)
            {
                case 1:
                    stamina -= 10;
                    St.transform.localScale = new Vector3((stamina / maxStamina), 0.5806693f, 1);
                    break;
                default:
                    break;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
