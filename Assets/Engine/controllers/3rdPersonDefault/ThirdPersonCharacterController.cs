using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ThirdPersonCharacterController : ControllerBase
{
    public Camera c;
    public float speed = 8.0f;
    public float turnSpeedSmooth = 0.1f;
    float turnSmoothVelocity;
    public CharacterController controller;

    
    int jumpCount;

    public float defaultGravity = 0.2f;

    public void onFixedUpdate(PawnBase obj)
    {

    }

    Vector3 onUpdate_MoveDir;
    Vector3 onUpdate_Direction;
    float onUpdate_VerticalSpeed;
    float onUpdate_GravityMultiplier;
    float onUpdate_TimeSinceGrounded;

    public override void onUpdate(PawnBase obj)
    {
        onUpdate_GravityMultiplier = 1;
        if (controller.isGrounded)
        {
            onUpdate_VerticalSpeed = 0;
            onUpdate_TimeSinceGrounded = 0;
        }else
        {
            onUpdate_TimeSinceGrounded += Time.deltaTime;
        }
        if (obj.isPossessed())
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            onUpdate_Direction.x = horizontal;
            onUpdate_Direction.z = vertical;

            Camera camera = GameModeBase.current.getCamera();
            if (onUpdate_Direction.magnitude >= 0.1 && controller)
            {
                float targetAngle = Mathf.Atan2(horizontal, vertical) * Mathf.Rad2Deg + camera.transform.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(obj.transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSpeedSmooth);
                obj.transform.rotation = Quaternion.Euler(0f, angle, 0f);
                onUpdate_MoveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                onUpdate_MoveDir = onUpdate_MoveDir.normalized * speed * Time.deltaTime;
            }else
            {
                onUpdate_MoveDir.x = 0;
                onUpdate_MoveDir.z = 0;
            }

            //Jumping Code
            if (controller.isGrounded || (onUpdate_TimeSinceGrounded < 0.5f && onUpdate_VerticalSpeed <= 0))
            {
                if (Input.GetKeyDown("space"))
                { // unless it jumps:
                    onUpdate_VerticalSpeed = 0.1f;
                }
            }
            if (Input.GetKey("space") && onUpdate_VerticalSpeed > 0)
            {
                onUpdate_GravityMultiplier = 0.65f;
            }

        }
        onUpdate_VerticalSpeed -= defaultGravity * onUpdate_GravityMultiplier * Time.deltaTime;
        onUpdate_MoveDir.y = onUpdate_VerticalSpeed;

        controller.Move(onUpdate_MoveDir);
    }

    public void onRemoveFromPawn(PawnBase obj)
    {
       
    }

    public override void onAddToPawn(PawnBase obj)
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = obj.GetComponent<CharacterController>();
    }
}
