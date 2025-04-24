using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour
{
    // VARIABLES
    public GameObject centerCam;
    [Space]
    public float moveSpeed;
    public float InputX;
    public float InputZ;
    public Vector3 desiredMoveDirection;
    public bool blockRotationPlayer;
    public float desiredRotationSpeed = 0.1f;
    public Animator anim;
    public float Speed;
    public float allowPlayerRotation = 0.1f;
    public Camera cam;
    public CharacterController controller;
    public bool isGrounded;

    [Header("Animation")]
    [Range(0, 1f)]
    public float HorizontalAnimSmoothTime = 0.2f;
    [Range(0, 1f)]
    public float VerticalAnimTime = 0.2f;
    [Range(0, 1f)]
    public float StartAnimTime = 0.3f;
    [Range(0, 1f)]
    public float StopAnimTime = 0.15f;

    public float verticalVel;
    private Vector3 moveVector;

    // FUNCTIONS
    void Start()
    {   // Set variables.
        anim = this.GetComponent<Animator>();
        cam = Camera.main;
        controller = this.GetComponent<CharacterController>();
    }

    void Update()
    {   // Move Player.
        InputMagnitude();
        isGrounded = controller.isGrounded;
        if (isGrounded)
        {
            verticalVel -= 0;
        }
        else
        {
            verticalVel -= 1;
        }
        moveVector = new Vector3(0, verticalVel * .2f * Time.deltaTime, 0);
        controller.Move(moveVector);
    }

    void PlayerMoveAndRotation()
    {   // Assign movement vars.
        InputX = Input.GetAxis("Horizontal");

        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        desiredMoveDirection = forward * 0 + right * InputX;
        // Move Player to assigned vars.
        if (blockRotationPlayer == false)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
            controller.Move(desiredMoveDirection * Time.deltaTime * moveSpeed);
        }
    }

    public void LookAt(Vector3 pos)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), desiredRotationSpeed);
    }

    void InputMagnitude()
    {
        // Calculate Input Vectors.
        InputX = Input.GetAxis("Horizontal");

        // Calculate the Input Magnitude.
        Speed = new Vector2(InputX, 0).sqrMagnitude;

        // Physically move player.
        if (Speed > allowPlayerRotation)
        {
            anim.SetFloat("Blend", Speed, StartAnimTime, Time.deltaTime);
            PlayerMoveAndRotation();
        }
        else if (Speed < allowPlayerRotation)
        {
            anim.SetFloat("Blend", Speed, StopAnimTime, Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {   // Rotate camera on rotation trigger.
        if (other.tag == "Rotation" && InputX > 0 && centerCam.GetComponent<GameCamera>().canRotate == true)
        {
            centerCam.GetComponent<GameCamera>().rotation -= 90;
            StartCoroutine(StopPlayer());
        }

        if (other.tag == "Rotation" && InputX < 0)
        {
            centerCam.GetComponent<GameCamera>().rotation += 90;
            StartCoroutine(StopPlayer());
        }

        if (other.tag == "Finish")
        {
            Debug.Log("FINISHED"); //test
            int currentScene = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentScene + 1);
        }
    }

    IEnumerator StopPlayer()
    {   // Stop Player from moving & rotating.
        centerCam.GetComponent<GameCamera>().canRotate = false;
        blockRotationPlayer = true;
        yield return new WaitForSeconds(1.2f);
        centerCam.GetComponent<GameCamera>().canRotate = true;
        blockRotationPlayer = false;
    }
}