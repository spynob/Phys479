using UnityEngine;
using UnityEngine.InputSystem;

public class InputSubscription : MonoBehaviour {
    public bool Swing { get; private set; } = false;
    public bool Swing2 { get; private set; } = false;
    InputSystem_Actions _Input = null;

    private void OnEnable() {
        _Input = new InputSystem_Actions();
        _Input.SpiderMan.Enable();
        // Subscribe to inputs
        _Input.SpiderMan.Swing.started += SetSwing;
        _Input.SpiderMan.Swing.canceled += SetSwing;
        _Input.SpiderMan.Swing2.started += SetSwing2;
        _Input.SpiderMan.Swing2.canceled += SetSwing2;
    }
    private void OnDisable() {
        // Unsubscribe to inputs
        _Input.SpiderMan.Swing.started -= SetSwing;
        _Input.SpiderMan.Swing.canceled -= SetSwing;
        _Input.SpiderMan.Swing2.started -= SetSwing2;
        _Input.SpiderMan.Swing2.canceled -= SetSwing2;
        _Input.SpiderMan.Disable();
    }

    void SetSwing(InputAction.CallbackContext ctx) {
        Swing = ctx.started;
    }
    void SetSwing2(InputAction.CallbackContext ctx) {
        Swing2 = ctx.started;
    }
}
