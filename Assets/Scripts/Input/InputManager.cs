using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerId
{
    PlayerOne,
    PlayerTwo
}

public readonly struct PlayerInputSnapshot
{
    public PlayerInputSnapshot(Vector2 move, bool jumpPressed, bool jumpHeld, bool interactPressed, bool interactHeld, bool pausePressed)
    {
        Move = move;
        JumpPressed = jumpPressed;
        JumpHeld = jumpHeld;
        InteractPressed = interactPressed;
        InteractHeld = interactHeld;
        PausePressed = pausePressed;
    }

    public Vector2 Move { get; }
    public bool JumpPressed { get; }
    public bool JumpHeld { get; }
    public bool InteractPressed { get; }
    public bool InteractHeld { get; }
    public bool PausePressed { get; }
}

[DefaultExecutionOrder(-100)]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [SerializeField] private InputActionAsset inputActions;

    public PlayerInputSnapshot PlayerOne { get; private set; }
    public PlayerInputSnapshot PlayerTwo { get; private set; }

    private PlayerActionSet playerOneActions;
    private PlayerActionSet playerTwoActions;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (inputActions == null)
        {
            Debug.LogError($"{nameof(InputManager)} needs an InputActionAsset assigned.", this);
            enabled = false;
            return;
        }

        playerOneActions = new PlayerActionSet(inputActions.FindActionMap("PlayerOne", true));
        playerTwoActions = new PlayerActionSet(inputActions.FindActionMap("PlayerTwo", true));
    }

    private void OnEnable()
    {
        playerOneActions?.Enable();
        playerTwoActions?.Enable();
    }

    private void OnDisable()
    {
        playerOneActions?.Disable();
        playerTwoActions?.Disable();
    }

    private void OnDestroy()
    {
        if (Instance != this)
        {
            return;
        }

        Instance = null;
    }

    private void Update()
    {
        if (playerOneActions == null || playerTwoActions == null)
        {
            PlayerOne = default;
            PlayerTwo = default;
            return;
        }

        PlayerOne = playerOneActions.ReadSnapshot();
        PlayerTwo = playerTwoActions.ReadSnapshot();
    }

    public PlayerInputSnapshot GetSnapshot(PlayerId playerId)
    {
        return playerId == PlayerId.PlayerOne ? PlayerOne : PlayerTwo;
    }

    private sealed class PlayerActionSet
    {
        private readonly InputActionMap actionMap;
        private readonly InputAction move;
        private readonly InputAction jump;
        private readonly InputAction interact;
        private readonly InputAction pause;

        public PlayerActionSet(InputActionMap actionMap)
        {
            this.actionMap = actionMap;
            move = actionMap.FindAction("Move", true);
            jump = actionMap.FindAction("Jump", true);
            interact = actionMap.FindAction("Interact", true);
            pause = actionMap.FindAction("Pause", true);
        }

        public void Enable()
        {
            actionMap.Enable();
        }

        public void Disable()
        {
            actionMap.Disable();
        }

        public PlayerInputSnapshot ReadSnapshot()
        {
            return new PlayerInputSnapshot(
                new Vector2(move.ReadValue<float>(), 0f),
                jump.WasPressedThisFrame(),
                jump.IsPressed(),
                interact.WasPressedThisFrame(),
                interact.IsPressed(),
                pause.WasPressedThisFrame());
        }

    }
}
