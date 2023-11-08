using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public Transform trans;

    [SerializeField] Rigidbody bodies;

    [SerializeField] Transform mal;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawn;

    public float moveSpeed;

    public float timeToNextDash;
    public float shootDelay;
    public float bulletSpeed;

    bool initialDelayApplied;

    [HideInInspector] [SerializeField] new Renderer renderer;

    // Start is called before the first frame update

    private void Reset()
    {
        renderer = GetComponent<MeshRenderer>();
    }
    void Start()
    {
        trans.GetComponent<Transform>();

        renderer = GetComponent<Renderer>();

        bodies = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput) * moveSpeed;
        transform.Translate(movement);

        
           

            transform.LookAt(mal);

           
        

      


        if (Input.GetKey(KeyCode.Mouse0) && canShoot())
        {
            Shoot();
            //StartCoroutine(AttackLag());
        }

       
    }


    public bool canShoot()
    {
        if (timeToNextDash < Time.realtimeSinceStartup)
        {
            timeToNextDash = Time.realtimeSinceStartup + shootDelay;
            return true;
        }

        else
        {
            
            return false;
            
        }

    }

    void Shoot()
    {
        var bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.Euler(new Vector3(0, 0, 0)));
        bullet.GetComponent<Rigidbody>().velocity = (mal.position - transform.position).normalized * bulletSpeed;
        Destroy(bullet, 5);



    }
    /*
    IEnumerator AttackLag()
    {
        for (int i = 1; i <= 2; i++)
        {
            bodies.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            bodies.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            yield return new WaitForSeconds(1f);
        }

        bodies.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
        bodies.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

    }
    */


}


