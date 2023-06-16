//--------------------------------------------
//          Agustin Ruscio & Merdeces Riego
//--------------------------------------------


using UnityEngine;

public class Chain : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponent<PlayerModel>();

        if(player != null)
        {
            player.TakeChain();
            Destroy(gameObject);
        }
    }
}