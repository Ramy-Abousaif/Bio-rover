using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEvent : MonoBehaviour
{
    void Death()
    {
        Destroy(gameObject);
    }
}
