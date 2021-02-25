using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Aurélien
/// </summary>
public class SheepController : MonoBehaviour
{
    public float angleOrientation;

    private void Start()
    {
        angleOrientation = Vector2.Angle(new Vector2(Vector3.forward.x, Vector3.forward.z), new Vector2(Mathf.Round(transform.forward.x), Mathf.Round(transform.forward.z)));

        if(Mathf.Round(transform.forward.x) < 0)
        {
            angleOrientation = -angleOrientation;
        }
    }

    IEnumerator MoveFrontBehind()
    {

        yield break;
    }
}
