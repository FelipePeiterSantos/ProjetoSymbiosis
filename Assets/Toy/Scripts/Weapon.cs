using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour
{
    public float Damage, Stamina, Resistance, Weight;
    public bool Active;
    // Use this for initialization
    void Start()
    {
        if (!Active)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
