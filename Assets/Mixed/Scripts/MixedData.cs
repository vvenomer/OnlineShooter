using UnityEngine;

public class MixedData : MonoBehaviour
{
    public int port = 1024;
    public int sendToServerInterval = 20;
    public int sendToClientInterval = 20;
    public GameObject player;

    public GameObject CreatePlayer(Vector2 vec, float angle)
    {
        return Instantiate(player, vec, Quaternion.Euler(0, 0, angle));
    }
}
