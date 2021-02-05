using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aaaa : MonoBehaviour
{
    List<GameObject> o = new List<GameObject>();
    [SerializeField] GameObject a;
    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            var go = Instantiate(a);
            go.name = i.ToString();
            o.Add(go);
        }
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            for (int i = o.Count - 1; i > -1; i--)
            {
                var go = o[i];
                Destroy(go);
                o.Remove(go);
            }
        }
    }

}
