using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITankFiringSystem : MonoBehaviour
{
    public GameObject _shellPrefab;
    public Transform _spawnPoint;


    public GameObject Fire()
    {
        //spawn shell
        GameObject shell = Instantiate(_shellPrefab, _spawnPoint.position, _spawnPoint.rotation);
        Destroy(shell, 2.0f);
        return shell;
    }
}
