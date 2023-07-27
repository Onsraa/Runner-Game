using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolScript : MonoBehaviour
{
    public Transform pathParent;
    public float speed;
    
    private void Start()
    {
        Transform[] pathPoints = new Transform[pathParent.childCount];
        for (int i = 0; i < pathPoints.Length; ++i)
        {
            pathPoints[i] = pathParent.GetChild(i).transform;
        }

        StartCoroutine(patrol(pathPoints));
    }
    
    IEnumerator patrol(Transform[] positions)
    {
        transform.position = positions[0].position;
        transform.rotation = positions[0].rotation;

        int index = 0;
        Vector3 targetPos = positions[index].position;

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, positions[index].position,
                speed * Time.deltaTime);

            if (transform.position == positions[index].position)
            {
                index = (index + 1) % positions.Length;
                transform.eulerAngles = positions[index].eulerAngles;
            }

            yield return null;
        }
    }
}
