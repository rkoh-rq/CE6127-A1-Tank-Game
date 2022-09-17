using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITankManager : MonoBehaviour
{
    public GameObject _spawnPointContainer;
    public GameObject _tankPrefab;

    protected List<AITank> mTanks = new List<AITank>();
    protected List<Transform> mSpawnPoints = new List<Transform>();

    public delegate void OnAITankDefeat();
    public OnAITankDefeat dOnAITankDefeat = null;

    private void Awake()
    {
        // Setup the spawn points from spawn parent
        Transform spawnTrans = _spawnPointContainer.transform;
        for (int i = 0; i < spawnTrans.childCount; i++)
            mSpawnPoints.Add(spawnTrans.GetChild(i));

        SpawnTanks();
    }

    public void OnAITankDeath(AITank target)
    {
        dOnAITankDefeat.Invoke();
        target.Restart(mSpawnPoints[target._playerNum].position, mSpawnPoints[target._playerNum].rotation);
    }

    public void End()
    {
        foreach (AITank tank in mTanks)
        {
            tank.Death();
        }
    }

    public void Restart()
    {
        foreach (AITank tank in mTanks)
        {
            int num = tank._playerNum;
            tank.Restart(mSpawnPoints[num].position, mSpawnPoints[num].rotation);
            tank.Revive();
        }
    }

    // Spawn and setup their color
    public void SpawnTanks()
    {
        int mPlayerCount = mSpawnPoints.Count;

        for (int i = 0; i < mPlayerCount; i++)
        {
            // Spawn Tank and store it
            GameObject tank = Instantiate(_tankPrefab, mSpawnPoints[i].position, mSpawnPoints[i].rotation);
            mTanks.Add(tank.GetComponent<AITank>());
            mTanks[i]._playerNum = i;
            mTanks[i].dTankDestroyed = OnAITankDeath;

            // Color Setup
            MeshRenderer[] renderers = mTanks[i].GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer rend in renderers)
                rend.material.color = Color.red;
        }
    }

    public Transform[] GetTanksTransform()
    {
        int count = mTanks.Count;
        Transform[] tanksTrans = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            tanksTrans[i] = mTanks[i].transform;
        }

        return tanksTrans;
    }

    public int NumberOfPlayers
    {
        get { return mSpawnPoints.Count; }
    }
}
