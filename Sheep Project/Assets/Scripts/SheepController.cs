using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Aurélien
/// </summary>
public class SheepController : MonoBehaviour
{
    [HideInInspector] public float angleOrientation;
    public float walkTime;

    private void Start()
    {
        angleOrientation = Vector2.Angle(new Vector2(Vector3.forward.x, Vector3.forward.z), new Vector2(Mathf.Round(transform.forward.x), Mathf.Round(transform.forward.z)));

        if (Mathf.Round(transform.forward.x) < 0)
        {
            angleOrientation = -angleOrientation;
        }
    }

    public void ChooseMovement(Vector3 direction)
    {
        if (direction == transform.forward || direction == -transform.forward)
        {
            MoveFrontBehindUntilSomething(direction);
        }
        else
        {
            MoveSide(direction);
        }
    }

    #region FrontBehind Moves

    public void MoveFrontBehindUntilSomething(Vector3 direction)
    {
        if (Physics.Raycast(transform.position, direction, 1f, LayerRefs.lR.blocMask))
        {
            StartCoroutine(BonkAgainstWall(direction));
        }
        else if (Physics.Raycast(transform.position, direction, 1f, LayerRefs.lR.sheepMask))
        {
            //Stack
        }
        else if (Physics.Raycast(transform.position + direction, Vector3.down, 1f, LayerRefs.lR.blocMask))
        {
            StartCoroutine(MoveFBThenMove(direction));
        }
        else
        {
            StartCoroutine(MoveFBThenFall(direction));
        }

    }

    IEnumerator MoveFBThenMove(Vector3 direction)
    {
        Vector3 originalPosition = transform.position;

        for (float i = 0; i < walkTime / 2f; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / walkTime;
            transform.position += Vector3.up * Time.fixedDeltaTime / (walkTime * 4f);

            yield return new WaitForFixedUpdate();
        }

        for (float i = 0; i < walkTime / 2f; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / walkTime;
            transform.position -= Vector3.up * Time.fixedDeltaTime / (walkTime * 4f);

            yield return new WaitForFixedUpdate();
        }

        transform.position = originalPosition + direction;

        MoveFrontBehindUntilSomething(direction);
    }

    IEnumerator MoveFBThenFall(Vector3 direction)
    {
        Vector3 originalPosition = transform.position;

        for (float i = 0; i < walkTime / 2f; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / walkTime;
            transform.position += Vector3.up * Time.fixedDeltaTime / (walkTime * 4f);

            yield return new WaitForFixedUpdate();
        }

        for (float i = 0; i < walkTime / 2f; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / walkTime;
            transform.position -= Vector3.up * Time.fixedDeltaTime / (walkTime * 4f);

            yield return new WaitForFixedUpdate();
        }

        transform.position = originalPosition + direction;

        StartCoroutine(FBFallUntilSomething(direction));
    }

    IEnumerator FBFallUntilSomething(Vector3 direction)
    {
        Vector3 originalPosition = transform.position;

        for (float i = 0; i < walkTime / 2f; i += Time.fixedDeltaTime)
        {
            transform.position += Vector3.down * Time.fixedDeltaTime * 2f / (walkTime);

            yield return new WaitForFixedUpdate();
        }

        transform.position = originalPosition + Vector3.down;

        if (Physics.Raycast(transform.position, Vector3.down, 1f, LayerRefs.lR.blocMask))
        {
            MoveFrontBehindUntilSomething(direction);
        }
        else
        {
            StartCoroutine(FBFallUntilSomething(direction));
        }
    }

    #endregion

    #region Side Moves

    public void MoveSide(Vector3 direction)
    {
        if (Physics.Raycast(transform.position, direction, 1f, LayerRefs.lR.blocMask))
        {
            StartCoroutine(BonkAgainstWall(direction));
        }
        else if (Physics.Raycast(transform.position, direction, 1f, LayerRefs.lR.sheepMask))
        {
            //Stack
        }
        else if (Physics.Raycast(transform.position + direction, Vector3.down, 1f, LayerRefs.lR.sheepMask))
        {
            //StackLevel+1
        }
        else if (Physics.Raycast(transform.position + direction, Vector3.down, 1f, LayerRefs.lR.blocMask))
        {
            StartCoroutine(MoveSNormal(direction));
        }
        else
        {
            StartCoroutine(MoveSThenFall(direction));
        }
    }

    IEnumerator MoveSNormal(Vector3 direction)
    {
        Vector3 originalPosition = transform.position;

        for (float i = 0; i < walkTime/2f; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / walkTime;
            transform.position += Vector3.up * Time.fixedDeltaTime / (walkTime*2f);

            yield return new WaitForFixedUpdate();
        }

        for (float i = 0; i < walkTime / 2f; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / walkTime;
            transform.position -= Vector3.up * Time.fixedDeltaTime / (walkTime * 2f);

            yield return new WaitForFixedUpdate();
        }



        transform.position = originalPosition + direction;
    }

    IEnumerator MoveSThenFall(Vector3 direction)
    {
        Vector3 originalPosition = transform.position;

        for (float i = 0; i < walkTime / 2f; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / walkTime;
            transform.position += Vector3.up * Time.fixedDeltaTime / (walkTime * 4f);

            yield return new WaitForFixedUpdate();
        }

        for (float i = 0; i < walkTime / 2f; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / walkTime;
            transform.position -= Vector3.up * Time.fixedDeltaTime / (walkTime * 4f);

            yield return new WaitForFixedUpdate();
        }

        transform.position = originalPosition + direction;

        StartCoroutine(SFallUntilSomething());
    }

    IEnumerator SFallUntilSomething()
    {
        Vector3 originalPosition = transform.position;

        for (float i = 0; i < walkTime / 2f; i += Time.fixedDeltaTime)
        {
            transform.position += Vector3.down * Time.fixedDeltaTime * 2f / (walkTime);

            yield return new WaitForFixedUpdate();
        }

        transform.position = originalPosition + Vector3.down;

        if (!Physics.Raycast(transform.position, Vector3.down, 1f, LayerRefs.lR.blocMask))
        {
            StartCoroutine(SFallUntilSomething());
        }
    }

    #endregion

    IEnumerator BonkAgainstWall(Vector3 direction)
    {
        Vector3 originalPosition = transform.position;

        for (float i = 0; i < 0.12f; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / (walkTime*4f);

            yield return new WaitForFixedUpdate();
        }

        for (float i = 0; i < 0.12f; i += Time.fixedDeltaTime)
        {
            transform.position -= direction * Time.fixedDeltaTime / (walkTime * 4f);

            yield return new WaitForFixedUpdate();
        }

        transform.position = originalPosition;
    }
}
