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

    public Material material;
    public Color walkingColor;
    public Color attackingColor;
    public Color attackLag;
    public Color hitstunColor;
    public Color stunColor;
    public Color retreatColor;
    public Color retreatEndColor;
    public Color evadeSlashColor;
    public Color waterfoulWindupColor;
    public Color waterfoulDanceColor;



    public float dodgeWindow;


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
    public bool canWaterfoul;
    public bool isWaterfouling;
    public bool isWaterfoulWindingup;
    public bool isDashing;

    

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

        material.color = walkingColor;

        isWalking = true;
        canDodge = true;
        canAttack = true;
        canWaterfoul = false;
        StartCoroutine(waterfoulCooldown());
    }

    // Update is called once per frame
    void Update()
    {


        dodgeWindow = Vector3.Distance(transform.position, agent.destination);

        currentDistance = dodgeWindow;

        transform.LookAt(player.transform);

        if (isWaterfouling)
        {
            agent.destination = attackPoints[0].transform.position;
        }

        else
        {
            if (isWalking)
            {
                agent.destination = attackPoints[0].transform.position;
                isAttacking = false;
                material.color = walkingColor;

            }


          
                if
                (Input.GetKeyDown(KeyCode.Space) && canAttack && !isAttacking && dodgeWindow <= 3)
                {
                    // Trigger the dash.
                    StartCoroutine(dashingThrust());
                }
            



            /*

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

             */



            if (!isDashing && !isAttacking && !isRetreating && !isMiniStagered && !isStagered && canAttack && canWaterfoul && dodgeWindow <= 6)
            {
                StartCoroutine(waterfoulDance());
            }



            if (miniStagerThreshold >= 3)
            {



                StartCoroutine(Hitstun());

            }

            if (stagerThreshold >= 20)
            {
                StartCoroutine(Stunned());
            }
        }

       

    }
    private IEnumerator dashbackward()
    {
        yield return new WaitUntil(() => retreatThreshold >= 7);
        isRetreating = true;
        isAttacking = false;
        isWalking = false;
        agent.ResetPath();
        material.color = retreatColor;
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
       
        StartCoroutine(RetreatLag());
        isWalking = true;
        retreatThreshold = 0.0f;
        StartCoroutine(dashbackward());

    }

    private void OnTriggerEnter(Collider other)
    {

       
            if (other.tag == "Bullet")
            {

                if (!isWaterfouling || !isWaterfoulWindingup || !isDashing)
            {

                if (canAttack && canDodge && dodgeWindow <= 5)
                {

                    StartCoroutine(EvadeSlash());
                    StartCoroutine(Attack());
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

            if (other.tag == "AttackPoint")
            {

                 if (canAttack)
                 {
                     StartCoroutine(Attack());
                  }

                

            }
        

        

      


    }

    
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "AttackPoint")
        {
            if (canAttack && !isWaterfoulWindingup && !isWaterfouling)
            {
                StartCoroutine(Attack());
            }
        }
    }
    
    /*
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "AttackPoint")
        {

           
                isAttacking = false;
                isWalking = true;
            
                
        }
    }
    */

    IEnumerator RetreatLag()
    {

        isStill = true;
        isWalking = false;
        material.color = retreatEndColor;

        yield return new WaitForSeconds(2f);


        isStill = false;
        isWalking = true;

    }
    
    private IEnumerator EvadeSlash()
    {
                canDodge = false;
                isWalking = false;
                isAttacking = true;
               
                Vector3 _start = transform.position;

                Vector3 dodgePos = player.transform.position + Quaternion.Euler(0, 90, 0) * (tran.position - player.transform.position).normalized * dodgeDistance;

                float _dodgeTimer = 0;
                float _dodgeTime = 0.5f;

                while(_dodgeTimer < _dodgeTime)
                {
                    var dTemp = _start + (dodgePos - _start) * (_dodgeTimer / _dodgeTime);
                    transform.position = dTemp;
                    _dodgeTimer += Time.deltaTime;
            material.color = evadeSlashColor;

            yield return null;
                }
        material.color = attackingColor;
        
    }

    IEnumerator DodgeCooldown()
    {

        

        yield return new WaitForSeconds(13f);



        canDodge = true;

    }

    IEnumerator Hitstun()
    {
        isWalking = false;
        isAttacking = false;
        canAttack = false;
        isMiniStagered = true;
        material.color = hitstunColor;

        agent.Stop();

        yield return new WaitForSeconds(0.75f);

        
       
        isMiniStagered = false;
        agent.Resume();
  

        miniStagerThreshold = 0;
        isWalking = true;
        canAttack = true;

    }

    IEnumerator Stunned()
    {
        isWalking = false;
        isAttacking = false;
        canAttack = true;
        isStagered = true;
        material.color = stunColor;


        agent.Stop();

        yield return new WaitForSeconds(3f);

        isWalking = true;
        canAttack = true;
        
        isStagered = false;
        agent.Resume();

        stagerThreshold = 0;

    }
    
    IEnumerator Attack()
    {
        agent.Stop();
        isAttacking = true;
        isWalking = false;
        canAttack = false;
        material.color = attackingColor;
        yield return new WaitForSeconds(2.0f);

        
        if(dodgeWindow <= 2){
            StartCoroutine(dashingThrust());
        }
        

        else
        {
            material.color = attackLag;
            yield return new WaitForSeconds(1.3f);


            canAttack = true;
            isAttacking = false;
            isWalking = true;
            agent.Resume();
        }
       
    }

    
    IEnumerator waterfoulCooldown()
    {
        yield return new WaitForSeconds(40f);
        canWaterfoul = true;
    }

    

    
    IEnumerator waterfoulDance()
    {
        isWaterfoulWindingup = true;
        isWalking = false;
        isAttacking = false;
        canAttack = false;
        canWaterfoul = false;
        agent.Stop();
        material.color = waterfoulWindupColor;
        
        yield return new WaitForSeconds(2.75f);

        isWaterfoulWindingup = false;
        isWaterfouling = true;
        agent.Resume();
       
        agent.speed = 25.0f;
        material.color = waterfoulDanceColor;

        yield return new WaitForSeconds(1.7f);

        agent.Stop();

        yield return new WaitForSeconds(0.75f);

        agent.Resume();

        yield return new WaitForSeconds(0.85f);

        agent.Stop();

        yield return new WaitForSeconds(0.55f);

        agent.Resume();


        yield return new WaitForSeconds(1.35f);

        
        agent.speed = 1.5f;
        agent.Stop();
        material.color = attackLag;

        yield return new WaitForSeconds(1f);

        agent.Resume();
        isWaterfouling = false;

        isWalking = true;
        canAttack = true;


    }



    IEnumerator dashingThrust()
    {
        agent.Stop();
        isAttacking = true;
        isWalking = false;
        canAttack = false;
        isDashing = true;
        material.color = attackLag;
        yield return new WaitForSeconds(1f);

        float previousLocation = dodgeWindow;

        material.color = attackingColor;
         Vector3 dashDirection = (player.transform.position - transform.position).normalized;

            // Set the dash target position.
            Vector3 dashTargetPosition = transform.position + dashDirection * (previousLocation + 6);

        while (Vector3.Distance(transform.position, dashTargetPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, dashTargetPosition, 20 * Time.deltaTime);
            yield return null;
        }

        // Reset the flag after the dash is complete.
        isDashing = false;
        material.color = attackLag;

        yield return new WaitForSeconds(1.3f);


        canAttack = true;
        isAttacking = false;
        isWalking = true;
        agent.Resume();
    }




}
