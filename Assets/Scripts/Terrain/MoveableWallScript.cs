using System.Collections;
using UnityEngine;

public class MoveableWallScript : MonoBehaviour, IShooteable
{
    public GameObject wall;

    public Transform from;

    public Transform to;

    public float timeToGo;

    public void Shoot()
    {
        StartCoroutine(GoTo(to));
    }

    IEnumerator GoTo(Transform point)
    {
        float completion = 0f;
        while (completion < 1)
        {
            wall.transform.position = Vector3.Lerp(from.position, to.position, completion);
            completion += Time.deltaTime / timeToGo;
            yield return null;
        }
    }
}
