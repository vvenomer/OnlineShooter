using UnityEngine;

namespace Server
{
    public class Controller : MonoBehaviour
    {

        Vector2 pos;
        float angle;
        bool changed;
        public void UpdatePos(Vector2 pos, float angle)
        {
            this.pos = pos;
            this.angle = angle;
            changed = true;
        }

        // Use this for initialization
        void Start()
        {
            changed = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (changed)
            {
                transform.position = pos;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }
}