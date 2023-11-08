using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BOM : MonoBehaviour
{
    public Transform tran;

    public GameObject player;

    [SerializeField] Rigidbody body;

    private NavMeshAgent agent;

    public GameObject[] attackPoints;





    public float dodgeSpeed;
    public float dodgeDistance;

    public float retreatThreshold;
    public float retreatSpeed;
    public float evadeDistance;

    public float stagerThreshold;
    public float miniStagerThreshold;



    public float currentDistance;


    public bool isWalking;
    public bool isAttacking;
    public bool canAttack;
    public bool canDodge;
    public bool isDodging;
    public bool isRetreating;
    public bool isStill;
    public bool isMiniStagered;
    public bool isStagered;

    private void Awake()
    {
        StartCoroutine(dashbackward());
        //StartCoroutine(EvadeSlash());
    }
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        tran.GetComponent<Transform>();

        body.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {



        if (isWalking)
        {
            agent.destination = attackPoints[0].transform.position;
            isAttacking = false;

        }

       

        if (agent.destination == attackPoints[0].transform.position)
        {
            isAttacking = true;
            isWalking = false;



        }
        else
        {
            isWalking = true;
            isAttacking = false;
        }

        transform.LookAt(player.transform);



        if (miniStagerThreshold >= 3)
        {
            StartCoroutine(Hitstun());

        }

        if (stagerThreshold >= 20)
        {
            StartCoroutine(Stunned());
        }

    }
    private IEnumerator dashbackward()
    {
        yield return new WaitUntil(() => retreatThreshold >= 5);
        isRetreating = true;
        isAttacking = false;
        isWalking = false;
        agent.ResetPath();
        var eY = transform.position;
        var pY = player.GetComponent<Transform>().position;

        eY.y = 0;
        pY.y = 0;

        Vector3 evadeDirection = (eY - pY).normalized;

        Vector3 _initialPosition = transform.position;

        Vector3 evadePosition = transform.position + evadeDirection * evadeDistance;
        float _timer = 0;
        float _dashTime = 2;
        while (_timer < _dashTime)
        {

            //lerp from a to b
            var temp = _initialPosition + (evadeDirection - _initialPosition) * (_timer / _dashTime);
            transform.position = temp;
            _timer += Time.deltaTime;

            yield return null;
        }

        //transform.position = evadePosition;

        isRetreating = false;
        isAttacking = true;
        StartCoroutine(RetreatLag());
        isWalking = true;
        retreatThreshold = 0.0f;
        StartCoroutine(dashbackward());

    }

    private void OnTriggerEnter(Collider other)
    {


        if (other.tag == "Bullet")
        {

            if (canAttack && canDodge)
            {

                StartCoroutine(EvadeSlash());
                StartCoroutine(DodgeCooldown());


            }

                
            else
            {
                retreatThreshold = retreatThreshold + 1.0f;
                stagerThreshold = stagerThreshold + 1.0f;

                miniStagerThreshold = miniStagerThreshold + 1.0f;

            }

        }


    }

    IEnumerator RetreatLag()
    {

        isStill = true;
        isWalking = false;

        yield return new WaitForSeconds(2f);


        isStill = false;
        isWalking = true;

    }
    
    private IEnumerator EvadeSlash()
    {
                canDodge = false;
                Vector3 _start = transform.position;

                Vector3 dodgePos = player.transform.position + Quaternion.Euler(0, 90, 0) * (tran.position - player.transform.position).normalized * dodgeDistance;

                float _dodgeTimer = 0;
                float _dodgeTime = 0.5f;

                while(_dodgeTimer < _dodgeTime)
                {
                    var dTemp = _start + (dodgePos - _start) * (_dodgeTimer / _dodgeTime);
                    transform.position = dTemp;
                    _dodgeTimer += Time.deltaTime;

                    yield return null;
                }
    }

    IEnumerator DodgeCooldown()
    {

        

        yield return new WaitForSeconds(20f);



        canDodge = true;

    }

    IEnumerator Hitstun()
    {
        isWalking = false;
        isAttacking = false;
        isMiniStagered = true;

        agent.Stop();

        yield return new WaitForSeconds(0.75f);

        isWalking = true;
        isAttacking = true;
        isMiniStagered = false;
        agent.Resume();

        miniStagerThreshold = 0;

    }

    IEnumerator Stunned()
    {
        isWalking = false;
        isAttacking = false;
        isStagered = true;

        agent.Stop();

        yield return new WaitForSeconds(5f);

        isWalking = true;
        isAttacking = true;
        isStagered = false;
        agent.Resume();

        stagerThreshold = 0;

    }


}
