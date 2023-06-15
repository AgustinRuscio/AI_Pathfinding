using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponent<PlayerModel>();

        if (player != null)
            player.TakeDamage();
    }
}
