using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Attach this class to objects that will Kill any character they touch
public class InstaKill : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Character target = collision.gameObject.GetComponent<Character>();

        if(target)
        {
            target.Kill();
        }
    }
}
