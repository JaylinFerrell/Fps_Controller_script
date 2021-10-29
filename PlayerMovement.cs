using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RootMotion.FinalIK;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    [Header("Camera Axis")]
    private string m_verticalLookAxis = "Mouse Y";
    private string m_horizontalLookAxis = "Mouse X";
    private float m_xAxisSensitivity = 0.2f;
    private float m_yAxisSensitivity = 0.2f;
    protected float m_xAxis;
    protected float m_yAxis;
    [SerializeField]
    private Rigidbody m_headTargetRigidbody = default;
    [Header("LookAt Settings")]
    [SerializeField]
    private FullBodyBipedIK m_fbbIK = default;
    [SerializeField]
    private Transform m_camera = default;
    [SerializeField]
    private Transform m_headTarget = default;
    [Header("Head Effector Settings")]
    [SerializeField]
    private Transform m_headEffector = default;
    //public GameObject playerAnimator; 
    public CharacterController controller;
    public GameObject animatorLink;
    Animator animator;
    //Speed
    public float speed = 0f;
    private float idle = 0f; 
    private float minSpeed = 2f;
    private float maxSpeed = 6f;
    //Stamina
    public float stamina = 0f;
    private float minStamina = 0f;
    private float maxStamina = 20f;
    private float cantJump = 6f;
    //Health
    public float health = 0f;
    private float minHealth = 0f;
    private float maxHealth = 100f;
    private float healthRegen = 25f;
    //Gravity and GroundCheck
    public float gravity = -9.81f;
    public float jumpHeight = 0f;
    private float minJumpHeight = 0f;
    private float maxJumpHeight = 3f;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask GroundMask;
    //Vectors
    Vector3 velocity;
    Vector3 checkMove;
    Vector3 lastPosition;
    //bools
    public bool isGrounded;
    public bool isMoving;
    bool istired;
    public bool hasFireArm;
    //UI
    public Slider staminaBar;
    public Slider healthBar;
    #endregion

    //void Awake()
    {
        lastPosition = transform.position; 
        
    }

    #region PlayerController
    void PlayerController()
    {
       
        if (Input.GetKey(KeyCode.LeftShift) && !istired)
        {
            speed = maxSpeed;
            if (Input.GetKey(KeyCode.W))
            {
                animator.SetBool("IsSprinting", true);
                transform.Translate(Vector3.forward * speed * Time.deltaTime);
               
            }
           
            if (Input.GetKeyUp(KeyCode.W))
            {
                speed = idle;
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsWalkingBackwards", false);
                animator.SetBool("IsWalkingLeft", false);
                animator.SetBool("IsWalkingRight", false);
                animator.SetBool("IsSprinting", false);
            }
            if (hasFireArm)
            {
                speed = maxSpeed;
                if (Input.GetKey(KeyCode.W))
                {

                    transform.Translate(Vector3.forward * speed * Time.deltaTime);
                    animator.SetBool("IsRunningWithGun", true);
                }
                
                
            }

        }
        else 
        {
            animator.SetBool("IsSprinting", false);
            animator.SetBool("IsRunningWithGun", false);
            speed = minSpeed;
            if (Input.GetKey(KeyCode.A))
            {

                transform.Translate(Vector3.left * speed * Time.deltaTime);
                animator.SetBool("IsWalkingLeft", true);
            }
            if (Input.GetKey(KeyCode.D))
            {

                transform.Translate(Vector3.right * speed * Time.deltaTime);
                animator.SetBool("IsWalkingRight", true);
            }
            if (Input.GetKey(KeyCode.W))
            {

                transform.Translate(Vector3.forward * speed * Time.deltaTime);
                animator.SetBool("IsWalking", true);
                
            }
            if (Input.GetKey(KeyCode.S))
            {

                transform.Translate(Vector3.back * speed * Time.deltaTime);
                animator.SetBool("IsWalkingBackwards", true);
            }
            if (!Input.anyKey)
            {
                speed = idle;
                animator.SetBool("IsWalking", false);
                animator.SetBool("IsWalkingBackwards", false);
                animator.SetBool("IsWalkingLeft", false);
                animator.SetBool("IsWalkingRight", false);
               

            }
            if (hasFireArm)
            {
                animator.SetBool("IsSprinting", false);
                speed = minSpeed;
                if (Input.GetKey(KeyCode.A))
                {

                    transform.Translate(Vector3.left * speed * Time.deltaTime);
                    animator.SetBool("IsWalkingWithGunLeft", true);
                }
                if (Input.GetKey(KeyCode.D))
                {

                    transform.Translate(Vector3.right * speed * Time.deltaTime);
                    animator.SetBool("IsWalkingWithGunRight", true);
                }
                if (Input.GetKey(KeyCode.W))
                {

                    transform.Translate(Vector3.forward * speed * Time.deltaTime);
                    animator.SetBool("IsWalkingWithGun", true);

                }
                if (Input.GetKey(KeyCode.S))
                {

                    transform.Translate(Vector3.back * speed * Time.deltaTime);
                    animator.SetBool("IsWalkingWithGunBackwards", true);
                }
            }
            if (Input.GetKey(KeyCode.Q))
            {
                hasFireArm = false;
                animator.SetBool("IsHoldingGun", false);
            }
        }
        if (Input.GetKeyUp(KeyCode.A))
        {

            animator.SetBool("IsWalkingLeft", false);
            animator.SetBool("IsWalkingWithGunLeft", false);
        }
        if (Input.GetKeyUp(KeyCode.D))
        {

            animator.SetBool("IsWalkingRight", false);
            animator.SetBool("IsWalkingWithGunRight", false);
        }
        if (Input.GetKeyUp(KeyCode.W))
        {

            animator.SetBool("IsWalking", false);
            animator.SetBool("IsWalkingWithGun", false);

        }
        if (Input.GetKeyUp(KeyCode.S))
        {

            animator.SetBool("IsWalkingBackwards", false);
            animator.SetBool("IsWalkingWithGunBackwards", false);

        }
        m_xAxis = Input.GetAxis(m_horizontalLookAxis) * m_xAxisSensitivity;
        m_yAxis = Input.GetAxis(m_verticalLookAxis) * m_yAxisSensitivity;
    }
    #endregion
    public float XLookAxis
    {
        get { return m_xAxis; }
    }
    public float YLookAxis
    {
        get { return m_yAxis; }
    }

    void Start()
    {
        m_fbbIK.enabled = false;
        hasFireArm = false;
        speed = idle;
        animator = GetComponent<Animator>();
        istired = false;
        health = 100f;
        isMoving = false;
        //Stamina Bar UI
        staminaBar.minValue = minStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = stamina;
        //Health Bar UI
        healthBar.minValue = minHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
    }

    // Update is called once per frame
    void Update()   
    {
        m_fbbIK.solver.FixTransforms();

        #region Are WE Jumping?? 
        // Checks to see if we are on the ground and then if so we can jump
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, GroundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
       
        if(Input.GetButtonDown("Jump") && isGrounded && stamina > cantJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("IsJumping", true);
            stamina = stamina - 5f;
            staminaBar.value = stamina;
        }
        else
        {
            animator.SetBool("IsJumping", false);
        }
     
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        m_headTargetRigidbody.velocity = velocity;
        #endregion

        // calls PlayerController Function
        PlayerController();
        
        //Checks if player is Moving
        if (lastPosition != transform.position)
        {
            isMoving = true;

        }
        else
        {
            isMoving = false;

        }
        

        #region Stamina Manager
        // makes sure that stamina cant go over max 
        if (stamina > maxStamina)
        {
            istired = false;
            stamina = maxStamina;
        }
        // make sure stamina cant go below min
        else if (stamina < minStamina)
        {
            istired = true;
            stamina = minStamina;
        }
       // if we are tired charge stamina
        if (istired)
        {
            speed = minSpeed;
            stamina += Time.deltaTime;
            staminaBar.value += Time.deltaTime;
        }
       // if speed is 0 or speed is 3 or we are nit moving stamina will Charge
       if (speed == idle || speed == minSpeed || !isMoving)
       {
            stamina += Time.deltaTime;
            staminaBar.value += Time.deltaTime;
       }
       // if we are runnibg and we are not tired and we are moving decrease stamina
       else if (speed == maxSpeed && !istired && isMoving)
       {
            stamina -= Time.deltaTime;
            staminaBar.value -= Time.deltaTime; 
       }
       // when we cant jump
        if (stamina <= cantJump)
        {
            jumpHeight = minJumpHeight;
        }
        //when we can jump
        else if (stamina > cantJump)
        {
            jumpHeight = maxJumpHeight;
        }
        #endregion

        #region Health Manager
        //Health code start
        if (health <= 25f)
        {
            speed = minSpeed;
            jumpHeight = minJumpHeight;
        }
        if (health >= 100)
        {
            health = 100;
        }
        else if (health <= 0)
        {
            this.gameObject.SetActive(false);
        }
        #endregion

        // sets lastPosition to playlerCharacter position so we can detect if we are moving
        lastPosition = this.transform.position;

    }

    private void LateUpdate()
    {
        FBBIKUpdate();
    }

    private void FBBIKUpdate()
    {
        m_fbbIK.solver.Update();

        m_camera.LookAt(m_headTarget);
        m_headEffector.LookAt(m_headTarget);
    }

    #region ON COLLISION
    void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            health = health - 25f;
            healthBar.value = health;
        }
        if (collision.gameObject.tag == "Regen iteam" && health <= 99f)
        {
            health = health + healthRegen;
            healthBar.value = health;
        }
        else if (collision.gameObject.tag == "Regen iteam" && health == 100f)
        {
            //WILL LET PLAYER PICK UP ITEAM (Coming Soon)
        }

        if (collision.gameObject.tag == "Weapon")
        {
                hasFireArm = true;
                animator.SetBool("IsHoldingGun", true);
               // idle_leftHandTarget = m_leftHandTarget;
        }

    }
    #endregion

    // works with Slider UI
    public void onValueChanged(float value)
 {
   //Lets me detect slider value and changes it im pretty sure it dose more but for now this is all i need ;p
 }

}

// TODO: Animation
// Add Shooting Animation
// Add Reloading Animation
// figure out how to pick iteams up 
// add JumpwithWeapon animation
// find out why i cant turn off left arm
// fix aiming animation





