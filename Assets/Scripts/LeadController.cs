using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.UI;
using UnityEngine;

public class LeadController : MonoBehaviour
{
    public LayerMask groundLayer;
    public LayerMask mouseAimMask;
    public GameObject weapon;
    public GameObject bullet;
    private MeshRenderer weaponRenderer;

    public float walkSpeed;
    public float jumpHeight = 10f;

    public float groundCheckRadius = 0.2f;
    public float wallCheckRadius = 0.2f;
    public float motionPlatformSpeed = 2f;
    public float motionPlatformMaximumMotionMagnitude = 6f;

    public Slider destinationSlider;

    public Transform muzzleTransform;
    public Transform[] groundCheckTransforms;
    public Transform[] wallCheckTransforms;
    public Transform targetTransform;
    public Transform finalLeadTransform;

    public AnimationCurve animationCurve;
    public float recoilDuration = 0.25f;
    public float recoilMaxRotatation = 45f;
    public Transform rightLowerArm;
    public Transform rightHand;
    private Material[] materials;
    private Animator animator;
    private Rigidbody rigidBody;
    private Camera mainCamera;


    public bool isDead = false;
    private bool isBlocked;
    public bool isHacking;
    private bool isGrounded;
    public bool isWeaponised = false;
    private bool shouldAnimateWeapon = false;
    private bool isJumpPressed;
    private float jumpTimer;
    private float jumpGracePeriod = 0.2f;
    private float inputMovement;
    private float unArmedFaceDirection;
    private float recoilTimer;
    private bool shouldLerpToDeath = false;

    public UnityEvent deathEvent;


    private int facingSign
    {
        get
        {
            Vector3 perpendicular = Vector3.Cross(transform.forward, Vector3.forward);
            float direction = Vector3.Dot(perpendicular, transform.up);
            return direction > 0 ? -1 : direction < 0 ? 1 : 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
        shouldLerpToDeath = false;
        materials = GetComponentInChildren<MeshRenderer>().materials;
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        weapon.SetActive(isWeaponised);
        weaponRenderer = weapon.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldLerpToDeath || isDead)
        {
            deathEvent?.Invoke();
            bool[] visibilities = new bool[materials.Length];
            var i = 0;
            foreach (var material in materials)
            {
                float lerp = Mathf.Lerp(material.GetFloat("_Visibility"), 1, 1.5f * Time.deltaTime);
                material.SetFloat("_Visibility", lerp);
                if (lerp > 0.85f)
                {
                    material.SetFloat("_Visibility", 1);
                    visibilities[i] = true;
                }
                i++;
            }
            if (visibilities.All(x => x))
            {
                isDead = true;
                shouldLerpToDeath = false;
                gameObject.SetActive(false);
                return;
            }
        }
        else
        {
            if (isHacking)
            {
                return;
            }

            HandleMovement();

            HandleJump();

            ToggleWeapon();

            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit aimHit;

            if (Physics.Raycast(ray, out aimHit, Mathf.Infinity, mouseAimMask))
            {
                targetTransform.position = aimHit.point;
            }

            HandleFire();

            destinationSlider.value = Mathf.Max(0, transform.position.x / finalLeadTransform.position.x);
        }

    }
    

    private void LateUpdate()
    {
        if (shouldLerpToDeath)
        {
            return;
        }
        if (recoilTimer < 0)
        {
            return;
        }
        float curveTime = (Time.time - recoilTimer) / recoilDuration;
        if (curveTime > 1f)
        {
            recoilTimer = -1;
        }
        else
        {
            rightLowerArm.Rotate(Vector3.forward, animationCurve.Evaluate(curveTime) * recoilMaxRotatation, Space.Self);
        }
        
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (shouldLerpToDeath)
        {
            return;
        }
        if (isWeaponised)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            animator.SetIKPosition(AvatarIKGoal.RightHand, targetTransform.position);
            animator.SetLookAtWeight(0.5f);
            animator.SetLookAtPosition(targetTransform.position);
        }
    }

    private void FixedUpdate()
    {
        if (shouldLerpToDeath || isDead)
        {
            rigidBody.velocity = Vector3.zero;
            return;
        }
        //Facing Direction
        if (isHacking)
        {
            rigidBody.velocity = Vector3.Lerp(rigidBody.velocity, new Vector3(0,rigidBody.velocity.y,0), Time.deltaTime * 2f);
            rigidBody.MoveRotation(Quaternion.Euler(new Vector3(0, 90 * Mathf.Sign(unArmedFaceDirection), 0)));
            animator.SetFloat("speed", facingSign * rigidBody.velocity.x / walkSpeed);
            isGrounded = false;
            foreach (var groundCheck in groundCheckTransforms)
            {
                if (Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore))
                {
                    isGrounded = true;
                    break;
                }
            }
            animator.SetBool("isGrounded", isGrounded);
            return;
        }

        isBlocked = false;

        foreach (var wallCheck in wallCheckTransforms)
        {
            if (Physics.CheckSphere(wallCheck.position, wallCheckRadius, groundLayer, QueryTriggerInteraction.Ignore))
            {
                isBlocked = true;
                break;
            }
        }
        if (!isBlocked)
        {
            rigidBody.velocity = new Vector3(inputMovement * walkSpeed, rigidBody.velocity.y, 0);

        }
        else
        {
            rigidBody.velocity = new Vector3(0, rigidBody.velocity.y, 0);
        }
        if (!isWeaponised)
        {
            rigidBody.MoveRotation(Quaternion.Euler(new Vector3(0, 90 * Mathf.Sign(unArmedFaceDirection), 0)));
            animator.SetFloat("speed", Mathf.Abs(rigidBody.velocity.x) / walkSpeed);
        }
        else
        {
            rigidBody.MoveRotation(Quaternion.Euler(new Vector3(0, 90 * Mathf.Sign(targetTransform.position.x - transform.position.x), 0)));
            animator.SetFloat("speed", facingSign * rigidBody.velocity.x / walkSpeed);
        }

        //Ground Check
        isGrounded = false;
        foreach (var groundCheck in groundCheckTransforms)
        {
            if (Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore))
            {
                isGrounded = true;
                break;
            }
        }
        bool isParented = transform.parent != null;
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("VerticalSpeed", rigidBody.velocity.y);
        animator.SetBool("isWeaponised", isWeaponised);

        animator.SetBool("IsParented", isParented);

    }

    private void HandleMovement()
    {
        inputMovement = Input.GetAxis("Horizontal");

        if (inputMovement != 0)
        {
            unArmedFaceDirection = inputMovement;
        }
    }

    private void HandleJump()
    {
        isJumpPressed = Input.GetButtonDown("Jump");

        if (isJumpPressed)
        {
            jumpTimer = Time.time;
        }


        if (isGrounded && (isJumpPressed || (jumpTimer > 0 && Time.time < jumpTimer + jumpGracePeriod)))
        {
            rigidBody.velocity = new Vector3(rigidBody.velocity.x, 0, 0);
            rigidBody.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -1 * Physics.gravity.y), ForceMode.VelocityChange);
            jumpTimer = -1;
        }
    }


    private void ToggleWeapon()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            weapon.SetActive(true);
            isWeaponised = !isWeaponised;
            weaponRenderer.material.SetFloat("_Visibility", isWeaponised ? shouldAnimateWeapon ? weaponRenderer.material.GetFloat("_Visibility") : 1f : shouldAnimateWeapon ? weaponRenderer.material.GetFloat("_Visibility") : 0f);
            shouldAnimateWeapon = true;
        }

        if (shouldAnimateWeapon)
        {
            float lerp = Mathf.Lerp(weaponRenderer.material.GetFloat("_Visibility"), isWeaponised ? 0 : 1, 3f * Time.deltaTime);
            weaponRenderer.material.SetFloat("_Visibility", lerp);
            if (isWeaponised)
            {
                if (lerp < 0.005)
                {
                    weaponRenderer.material.SetFloat("_Visibility", 0);
                    shouldAnimateWeapon = false;
                }
            }
            else
            {
                if (lerp > 0.9)
                {
                    weaponRenderer.material.SetFloat("_Visibility", 0);
                    shouldAnimateWeapon = false;
                    weapon.SetActive(false);
                }
            }
        }
    }

    private void HandleFire()
    {
        if (isWeaponised && Input.GetButtonDown("Fire1"))
        {
            recoilTimer = Time.time;
            var lazerBulletObject = Instantiate(bullet);
            lazerBulletObject.transform.position = muzzleTransform.transform.position;
            var script = lazerBulletObject.GetComponent<LazerBullet>();
            script.Fire(lazerBulletObject.transform.position, muzzleTransform.eulerAngles);
        }
    }


    internal void LerpToDeath()
    {
        shouldLerpToDeath = true;
    }

    public void Respwan(Transform spawnTransform)
    {
        isDead = false;
        gameObject.transform.position = spawnTransform.position + Vector3.up;
        rigidBody.velocity = Vector3.zero;
        rigidBody.MoveRotation(Quaternion.Euler(Vector3.back));
        gameObject.SetActive(true);
    }
}
