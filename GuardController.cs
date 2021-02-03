using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardController : MonoBehaviour
{
    private NavMeshAgent agent;
    [SerializeField] private Transform[] NPCPool;
    private Animator anim;
    private bool StopCarutine;
    [SerializeField] private int num;
    [SerializeField] private Transform playerPosition;
    private PlayerPosition PlayerState;
    [SerializeField] private GameObject greenRomb;
    [SerializeField] private GameObject redRomb;
    private float reachedDistance;
    
    public bool PlayerDetected;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        agent.SetDestination(NPCPool[Random.Range(0, NPCPool.Length)].position);
        StopCarutine = true;
        PlayerDetected = false;
        PlayerState = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPosition>();
    }

    
    void Update()
    {
        if (!HasReached())
        {
            anim.SetBool("WalkOn", true);
        }
        else anim.SetBool("WalkOn", false);

        if (HasReached() && StopCarutine)
        {
            StopCarutine = false;
            StartCoroutine(SetNewPosition());
        }
    }

    private void FixedUpdate()
    {
        if (PlayerState.PlayerOnSafeZone)
        {
            PlayerDetected = false;
        }

        if (!PlayerDetected)
        {
            reachedDistance = 2.8f;
            greenRomb.SetActive(true);
            redRomb.SetActive(false);
            agent.stoppingDistance = 3; 
            agent.SetDestination(NPCPool[num].position);
        }
        else if (PlayerDetected)
        {
            reachedDistance = 0.5f;
            greenRomb.SetActive(false);
            redRomb.SetActive(true);
            agent.stoppingDistance = 0;
            agent.SetDestination(playerPosition.position);
        }
    }

    private bool HasReached()
    {
        if (agent.remainingDistance < reachedDistance)
            return true;
        else
            return false;
    }

    IEnumerator SetNewPosition()
    {
        yield return new WaitForSeconds(Random.Range(2, 3));
        num = Random.Range(0, NPCPool.Length);
        StopCarutine = true;
    }
}
