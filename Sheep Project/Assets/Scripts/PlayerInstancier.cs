using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Aurélien
/// </summary>
public class PlayerInstancier : MonoBehaviour
{
    public GameObject playerPrefab;

    private void Start()
    {
        if(!GameObject.Find("Player"))
        {
            Instantiate(playerPrefab).transform.position = transform.position;
        }

        Destroy(gameObject);
    }
}
