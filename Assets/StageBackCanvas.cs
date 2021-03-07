using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIconRoot : MonoBehaviour
{
    // Start is called before the first frame update
    public List<WayStruct> Waypoint;

    public struct WayStruct
    {
        public Vector3 Waypoint;

        public bool Brake;
        public bool NotAccele;
    }




    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos()
    {
        for (int n = 0; n < Waypoint.Count; n++)
        {
            Gizmos.color = new Color(1f, 0, 0, 1);

            Gizmos.DrawSphere(Waypoint[n].Waypoint, 0.5f);


        }
        for (int n = 0; n < Waypoint.Count; n++)
        {



        }
        for (int n = 1; n < Waypoint.Count; n++)
        {
            Gizmos.DrawLine(Waypoint[n - 1].Waypoint, Waypoint[n].Waypoint);
        }
    }
}