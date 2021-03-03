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

    public Collider col;

    public int fallSinceXCases;

    public SheepController childSheep;

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
        ParentEverySheep(this);

        if (direction == transform.forward || direction == -transform.forward)
        {
            MoveFrontBehindUntilSomething(direction);
        }
        else
        {
            MoveSide(direction);
        }
    }

    public void ParentEverySheep(SheepController sheepToParent)
    {
        sheepToParent.transform.SetParent(transform);

        Vector3 closestPoint;

        RaycastHit hit;

        closestPoint = col.ClosestPoint(transform.position + Vector3.up);

        if (Physics.Raycast(closestPoint, Vector3.up, out hit, 0.5f, LayerRefs.lR.sheepMask))
        {
            sheepToParent.ParentEverySheep(hit.transform.parent.GetComponent<SheepController>());
        }
    }

    public void UnParentEverySheep(SheepController sheepToParent)
    {
        sheepToParent.transform.SetParent(null);

        Vector3 closestPoint;

        RaycastHit hit;

        closestPoint = col.ClosestPoint(transform.position + Vector3.up);

        if (Physics.Raycast(closestPoint, Vector3.up, out hit, 0.5f, LayerRefs.lR.sheepMask))
        {
            sheepToParent.UnParentEverySheep(hit.transform.parent.GetComponent<SheepController>());
        }
    }

    #region FrontBehind Moves

    public void MoveFrontBehindUntilSomething(Vector3 direction)
    {
        Vector3 closestPoint = col.ClosestPoint(transform.position + direction);

        if (Physics.Raycast(transform.position, direction, 1f, LayerRefs.lR.blocMask))
        {
            StartCoroutine(BonkAgainstWall(direction));
        }
        else if (Physics.Raycast(closestPoint, direction, 0.5f, LayerRefs.lR.sheepMask))
        {
            if (!Physics.Raycast(transform.position + Vector3.up, direction, 1f, LayerRefs.lR.blocMask + LayerRefs.lR.sheepMask))
                StartCoroutine(MoveTowardSheepFB(direction));
            else
                UnParentEverySheep(this);
        }
        else if (Physics.Raycast(transform.position + direction, Vector3.down, 1f, LayerRefs.lR.blocMask + LayerRefs.lR.sheepMask))
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

        StartCoroutine(FBFallUntilSomething(direction, true));
    }

    IEnumerator FBFallUntilSomething(Vector3 direction, bool bounce)
    {
        fallSinceXCases += 1;

        Vector3 originalPosition = transform.position;

        for (float i = 0; i < walkTime / 2f; i += Time.fixedDeltaTime)
        {
            transform.position += Vector3.down * Time.fixedDeltaTime * 2f / (walkTime);

            yield return new WaitForFixedUpdate();
        }

        transform.position = originalPosition + Vector3.down;

        if (Physics.Raycast(transform.position, Vector3.down, 1f, LayerRefs.lR.blocMask))
        {
            fallSinceXCases = 0;

            MoveFrontBehindUntilSomething(direction);
        }
        else if (Physics.Raycast(col.ClosestPoint(transform.position + Vector3.down), Vector3.down, 0.5f, LayerRefs.lR.sheepMask))
        {
            if (bounce)
                StartCoroutine(BounceBackFromSheepFB(direction, fallSinceXCases));
            else
                UnParentEverySheep(this);

            fallSinceXCases = 0;
        }
        else
        {
            StartCoroutine(FBFallUntilSomething(direction, bounce));
        }
    }

    IEnumerator BounceBackFromSheepFB(Vector3 direction, int casesToGain)
    {
        while (casesToGain > 0 && (!Physics.Raycast(transform.position, Vector3.up, 1f, LayerRefs.lR.blocMask)))
        {
            casesToGain -= 1;

            Vector3 originalPosition = transform.position;

            for (float i = 0; i < walkTime / 4f; i += Time.fixedDeltaTime)
            {
                transform.position += Vector3.up * Time.fixedDeltaTime * 4f / (walkTime);

                yield return new WaitForFixedUpdate();
            }

            transform.position = originalPosition + Vector3.up;
        }

        if (Physics.Raycast(transform.position, direction, 1f, LayerRefs.lR.blocMask + LayerRefs.lR.sheepMask))
        {
            StartCoroutine(FBFallUntilSomething(direction, false));
        }
        else
        {
            Vector3 originalPosition = transform.position;

            for (float i = 0; i < walkTime / 4f; i += Time.fixedDeltaTime)
            {
                transform.position += direction * Time.fixedDeltaTime * 2f / walkTime;
                transform.position += Vector3.up * Time.fixedDeltaTime * 2f / (walkTime);

                yield return new WaitForFixedUpdate();
            }

            for (float i = 0; i < walkTime / 4f; i += Time.fixedDeltaTime)
            {
                transform.position += direction * Time.fixedDeltaTime * 2f / walkTime;
                transform.position -= Vector3.up * Time.fixedDeltaTime * 2f / (walkTime);

                yield return new WaitForFixedUpdate();
            }

            transform.position = originalPosition + direction;

            if (!Physics.Raycast(transform.position, Vector3.down, 1f, LayerRefs.lR.blocMask + LayerRefs.lR.sheepMask))
            {
                StartCoroutine(FBFallUntilSomething(direction, true));
            }
            else
            {
                MoveFrontBehindUntilSomething(direction);
            }
        }
    }

    IEnumerator MoveTowardSheepFB(Vector3 direction)
    {
        Vector3 originalPosition = transform.position;

        for (float i = 0; i < walkTime / 2f; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / walkTime;
            transform.position += Vector3.up * Time.fixedDeltaTime*2.5f / walkTime;

            yield return new WaitForFixedUpdate();
        }

        for (float i = 0; i < walkTime / 2f; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / walkTime;
            transform.position -= Vector3.up * Time.fixedDeltaTime * 0.5f / walkTime;

            yield return new WaitForFixedUpdate();
        }

        transform.position = originalPosition + direction + Vector3.up;

        MoveFrontBehindUntilSomething(direction);
    }


    #endregion

    #region Side Moves

    public void MoveSide(Vector3 direction)
    {
        Vector3 closestPoint = col.ClosestPoint(transform.position + direction);

        if (Physics.Raycast(transform.position, direction, 1f, LayerRefs.lR.blocMask))
        {
            StartCoroutine(BonkAgainstWall(direction));
        }
        else if (Physics.Raycast(closestPoint, direction, 0.5f, LayerRefs.lR.sheepMask))
        {
            if (!Physics.Raycast(transform.position, Vector3.up, 1f, LayerRefs.lR.blocMask) && !Physics.Raycast(transform.position + Vector3.up, direction, 1f, LayerRefs.lR.blocMask + LayerRefs.lR.sheepMask))
            {
                StartCoroutine(MoveTowardSheepS(direction));
            }
            else
            {
                StartCoroutine(BonkAgainstWall(direction));
            }
        }
        else if (Physics.Raycast(transform.position + direction, Vector3.down, 1f, LayerRefs.lR.blocMask + LayerRefs.lR.sheepMask))
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

        UnParentEverySheep(this);
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

        StartCoroutine(SFallUntilSomething(direction, true));
    }

    IEnumerator SFallUntilSomething(Vector3 direction, bool bounce)
    {
        fallSinceXCases += 1;

        Vector3 originalPosition = transform.position;

        for (float i = 0; i < walkTime / 2f; i += Time.fixedDeltaTime)
        {
            transform.position += Vector3.down * Time.fixedDeltaTime * 2f / (walkTime);

            yield return new WaitForFixedUpdate();
        }

        transform.position = originalPosition + Vector3.down;

        if (Physics.Raycast(col.ClosestPoint(transform.position + Vector3.down), Vector3.down, 0.5f, LayerRefs.lR.sheepMask))
        {
            if(bounce)
                StartCoroutine(BounceBackFromSheepS(direction, fallSinceXCases));
            else
                UnParentEverySheep(this);

            fallSinceXCases = 0;
        }
        else if (!Physics.Raycast(transform.position, Vector3.down, 1f, LayerRefs.lR.blocMask))
        {
            StartCoroutine(SFallUntilSomething(direction, bounce));
        }
        else
        {
            fallSinceXCases = 0;
        }
    }

    IEnumerator BounceBackFromSheepS(Vector3 direction, int casesToGain)
    {
        while (casesToGain > 0 && (!Physics.Raycast(transform.position, Vector3.up, 1f, LayerRefs.lR.blocMask)))
        {
            casesToGain -= 1;

            Vector3 originalPosition = transform.position;

            for (float i = 0; i < walkTime / 4f; i += Time.fixedDeltaTime)
            {
                transform.position += Vector3.up * Time.fixedDeltaTime * 4f / (walkTime);

                yield return new WaitForFixedUpdate();
            }

            transform.position = originalPosition + Vector3.up;
        }

        if (Physics.Raycast(transform.position, direction, 1f, LayerRefs.lR.blocMask + LayerRefs.lR.sheepMask))
        {
            StartCoroutine(SFallUntilSomething(direction, false));
        }
        else
        {
            Vector3 originalPosition = transform.position;

            for (float i = 0; i < walkTime / 4f; i += Time.fixedDeltaTime)
            {
                transform.position += direction * Time.fixedDeltaTime * 2f / walkTime;
                transform.position += Vector3.up * Time.fixedDeltaTime * 2f / (walkTime);

                yield return new WaitForFixedUpdate();
            }

            for (float i = 0; i < walkTime / 4f; i += Time.fixedDeltaTime)
            {
                transform.position += direction * Time.fixedDeltaTime * 2f / walkTime;
                transform.position -= Vector3.up * Time.fixedDeltaTime * 2f / (walkTime);

                yield return new WaitForFixedUpdate();
            }

            transform.position = originalPosition + direction;

            if (!Physics.Raycast(transform.position, Vector3.down, 1f, LayerRefs.lR.blocMask + LayerRefs.lR.sheepMask))
            {
                StartCoroutine(SFallUntilSomething(direction, true));
            }
            else
            {
                UnParentEverySheep(this);
            }
        }
    }

    IEnumerator MoveTowardSheepS(Vector3 direction)
    {
        Vector3 originalPosition = transform.position;

        for (float i = 0; i < walkTime / 2f; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / walkTime;
            transform.position += Vector3.up * Time.fixedDeltaTime * 2.25f / walkTime;

            yield return new WaitForFixedUpdate();
        }

        for (float i = 0; i < walkTime / 2f; i += Time.fixedDeltaTime)
        {
            transform.position += direction * Time.fixedDeltaTime / walkTime;
            transform.position -= Vector3.up * Time.fixedDeltaTime * 0.25f / walkTime;

            yield return new WaitForFixedUpdate();
        }

        transform.position = originalPosition + direction + Vector3.up;

        UnParentEverySheep(this);
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

        UnParentEverySheep(this);
    }
}
