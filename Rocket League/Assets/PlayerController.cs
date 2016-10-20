using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    Rigidbody body;
    bool isGrounded = false;
    public LayerMask driveableSurfaces;

    public float driveAccelleration;

    public float maxTireSpeed;

    public float torqueAir;
    public float torqueGround;
    public float distanceToGround = .6f;

    float tireSpeed = 0;

    void Start () {
        body = GetComponent<Rigidbody>();
	}
	
	
	void FixedUpdate () {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        bool b = Input.GetButton("Handbrake");
        bool j = Input.GetButton("Jump");

        CheckGround();

        if (isGrounded)
        {
            //   print("grounded");

            //Vector3 torque = new Vector3(0,h,0);            
            //body.AddRelativeTorque(torque * torqueGround);


            body.AddRelativeForce(new Vector3(0, 0, v * driveAccelleration));

            tireSpeed += v * driveAccelleration * Time.fixedDeltaTime;
            tireSpeed = Mathf.Clamp(tireSpeed, -maxTireSpeed, maxTireSpeed);

            float speedPercent = tireSpeed / maxTireSpeed;

            body.velocity = tireSpeed * transform.forward;

            if(v == 0)
            {
                if(tireSpeed > 0)
                {
                    tireSpeed -= driveAccelleration * Time.fixedDeltaTime *.1f;
                    if (tireSpeed < 0) tireSpeed = 0;
                }
                if(tireSpeed < 0)
                {
                    tireSpeed += driveAccelleration * Time.fixedDeltaTime * .1f; 
                    if (tireSpeed > 0) tireSpeed = 0;

                }
            }

            Quaternion newQ = Quaternion.AngleAxis(h * torqueGround * Time.deltaTime * speedPercent, transform.up) * transform.rotation;
            body.MoveRotation(newQ);

            //body.velocity = transform.forward * body.velocity.magnitude;

            //theory
            //body.velocity = new Vector3(body.velocity.x,0,body.velocity.z).normalized;
            //body.velocity = body.velocity * Vector3.forward;


        }//end grounded
        else
        {//player is in air:

            Vector3 torque = new Vector3();
            if (b)
            {//if holding handbrake
                torque.z = -h * torqueAir;
            }else
            {
                torque.y = h * torqueAir;
            }

            torque.x = v * torqueAir;
            body.AddRelativeTorque(torque);
        }


	} //end fixed update

    void CheckGround()
    {
        Ray ray = new Ray(transform.position,transform.up * -1);
        RaycastHit hit;

        //Debug.DrawRay(ray.origin,ray.direction * distanceToGround);

        if(Physics.Raycast(ray, out hit, distanceToGround, driveableSurfaces))
        {
            isGrounded = true;
           // print("True");
        }
        else
        {
            isGrounded = false;
        }
    }
}
