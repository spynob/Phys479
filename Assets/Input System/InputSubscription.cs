using UnityEngine;
using UnityEngine.InputSystem;

public class InputSubscription : MonoBehaviour {
    public Vector2 MoveInput { get; private set; } = Vector2.zero;
    public bool Swing { get; private set; } = false;
    InputSystem_Actions _Input = null;

    private void OnEnable() {
        _Input = new InputSystem_Actions();
        _Input.Player.Enable();
        // Subscribe to inputs
        _Input.Player.Move.performed += SetMovement;
        _Input.Player.Move.canceled += SetMovement;
        _Input.Player.Swing.started += SetSwing;
        _Input.Player.Swing.canceled += SetSwing;
    }
    private void OnDisable() {
        // Unsubscribe to inputs
        _Input.Player.Move.performed -= SetMovement;
        _Input.Player.Move.canceled -= SetMovement;
        _Input.Player.Swing.started -= SetSwing;
        _Input.Player.Swing.canceled -= SetSwing;
        _Input.Player.Disable();
    }

    void SetMovement(InputAction.CallbackContext ctx) {
        MoveInput = ctx.ReadValue<Vector2>();
    }

    void SetSwing(InputAction.CallbackContext ctx) {
        Swing = ctx.started;
    }
}
