using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform cameraPivot;
    [SerializeField, Range(0.1f,2f)] private float mouseSensitivity = 1f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField, Range(0f,90f)] private float maxLookAngle = 85f;
    [SerializeField, Range(-90,0f)] private float minLookAngle = -85f;

    private CharacterController characterController;
    private Transform playerTransform;
    private Vector3 horizontalDirection = Vector3.zero;
    private Vector3 horizontalVelocity = Vector3.zero;
    private Vector2 lookDelta = Vector2.zero;
    private Vector3 newRotation = Vector3.zero;

    public UnityEvent<bool> onFocusedChanged;

    private bool Focused
    {
        get => Cursor.lockState == CursorLockMode.Locked;
        set
        {
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !value;
            onFocusedChanged?.Invoke(value);
        }
    }

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerTransform = transform;
        newRotation.x = cameraPivot.localRotation.eulerAngles.x;
        newRotation.y = playerTransform.localRotation.eulerAngles.y;
    }

    private void Update()
    {
        HandleHorizontalMovement();
        HandleRotation();
    }

    private void HandleHorizontalMovement()
    {
        horizontalVelocity = playerTransform.forward * horizontalDirection.z + playerTransform.right * horizontalDirection.x;
        horizontalVelocity = horizontalVelocity.normalized * moveSpeed;
        characterController.Move(horizontalVelocity * Time.deltaTime);
    }

    private void HandleRotation()
    {
        newRotation.x += lookDelta.y * mouseSensitivity;
        newRotation.x = Mathf.Clamp(newRotation.x, minLookAngle, maxLookAngle);
        cameraPivot.localRotation = Quaternion.Euler(newRotation.x, 0f, 0f);

        newRotation.y += lookDelta.x * mouseSensitivity;
        newRotation.y = Mathf.Repeat(newRotation.y, 359f);
        playerTransform.localRotation = Quaternion.Euler(0f, newRotation.y, 0f);
    }

    public void SetMouseSense(float sense)
    {
        mouseSensitivity = sense;
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!Focused) return;

        if (context.performed)
        {
            horizontalDirection.x = context.ReadValue<Vector2>().x;
            horizontalDirection.z = context.ReadValue<Vector2>().y;
        }

        if (context.canceled)
        {
            horizontalDirection = Vector3.zero;
        }
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (!Focused) return;

        if (context.performed)
        {
            lookDelta.x = context.ReadValue<Vector2>().x;
            lookDelta.y = -context.ReadValue<Vector2>().y;
        }

        if (context.canceled)
        {
            lookDelta = Vector2.zero;
        }
    }

    public void ChangeFocus(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Focused = !Focused;

            if (!Focused)
            {
                horizontalDirection = Vector3.zero;
                lookDelta = Vector2.zero;
            }
        }
    }
}
