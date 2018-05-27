using UnityEngine;

public class MixedData : MonoBehaviour {
    public int port = 1024;
    public int sendToServerInterval = 20;
    public int sendToClientInterval = 20;
    public GameObject player;

    public GameObject CreatePlayer(Vector2 vec, Vector2 angle)
    {
        return Instantiate(player, vec, Quaternion.Euler(angle));
    }
}
