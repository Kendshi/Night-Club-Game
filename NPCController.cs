using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;
    private bool OnDanceFloor;
    private bool StopCarutine;
    private bool rotateOnOff;
    private Vector3 rotatevectror;
    [SerializeField] private Transform[] target;

    public Transform rotatePoint;
    
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        agent.destination = target[Random.Range(0, target.Length)].position;
        StopCarutine = true;
        rotateOnOff = true;
        rotatevectror = rotatePoint.position;
    }

    void Update()
    {
        if (!HasReached())
        {
            anim.SetBool("WalkOn", true);
        }
        else anim.SetBool("WalkOn", false);

        if (HasReached() && OnDanceFloor)
        {
            anim.SetBool("Dance", true);
        }
        else anim.SetBool("Dance", false);

        if (HasReached() && StopCarutine)
        {
            StopCarutine = false;
            StartCoroutine(SetNewPosition());
        }

        if (HasReached() && rotateOnOff)
        {
            rotateOnOff = false;
            transform.rotation = Quaternion.LookRotation(rotatevectror);
        }
         else if (!HasReached() && !rotateOnOff)
        {
            rotateOnOff = true;
        }
    }

    private bool HasReached()
    {
        if (agent.remainingDistance < 1.5f)
            return true;
        else
            return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("DanceFloor"))
        {
            OnDanceFloor = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("DanceFloor"))
            OnDanceFloor = false;
    }

    IEnumerator SetNewPosition()
    {
        yield return new WaitForSeconds(Random.Range(20, 51));
        agent.SetDestination(target[Random.Range(0, target.Length)].position);
        StopCarutine = true;
    }
}
