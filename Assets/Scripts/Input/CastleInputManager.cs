using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public enum CastlePlayerId
{
    PlayerOne,
    PlayerTwo
}

public readonly struct CastlePlayerInputSnapshot
{
    public CastlePlayerInputSnapshot(Vector2 move, bool jumpPressed, bool jumpHeld, bool interactPressed, bool interactHeld, bool crawlHeld, bool pausePressed)
    {
        Move = move;
        JumpPressed = jumpPressed;
        JumpHeld = jumpHeld;
        InteractPressed = interactPressed;
        InteractHeld = interactHeld;
        CrawlHeld = crawlHeld;
        PausePressed = pausePressed;
    }

    public Vector2 Move { get; }
    public bool JumpPressed { get; }
    public bool JumpHeld { get; }
    public bool InteractPressed { get; }
    public bool InteractHeld { get; }
    public bool CrawlHeld { get; }
    public bool PausePressed { get; }
}

public class CastleInputManager : MonoBehaviour
{
    public static CastleInputManager Instance { get; private set; }

    [SerializeField] private bool enableGamepads = true;

    public CastlePlayerInputSnapshot PlayerOne { get; private set; }
    public CastlePlayerInputSnapshot PlayerTwo { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        PlayerOne = ReadPlayerOne();
        PlayerTwo = ReadPlayerTwo();
    }

    public CastlePlayerInputSnapshot GetSnapshot(CastlePlayerId playerId)
    {
        return playerId == CastlePlayerId.PlayerOne ? PlayerOne : PlayerTwo;
    }

    private CastlePlayerInputSnapshot ReadPlayerOne()
    {
        Keyboard keyboard = Keyboard.current;
        Vector2 move = Vector2.zero;
        bool jumpPressed = false;
        bool jumpHeld = false;
        bool interactPressed = false;
        bool interactHeld = false;
        bool crawlHeld = false;
        bool pausePressed = false;

        if (keyboard != null)
        {
            move.x += ReadAxis(keyboard.aKey, keyboard.dKey);
            jumpPressed |= keyboard.wKey.wasPressedThisFrame;
            jumpHeld |= keyboard.wKey.isPressed;
            crawlHeld |= keyboard.sKey.isPressed;
            interactPressed |= keyboard.leftCtrlKey.wasPressedThisFrame;
            interactHeld |= keyboard.leftCtrlKey.isPressed;
            pausePressed |= keyboard.escapeKey.wasPressedThisFrame;
        }

        AddGamepadInput(0, ref move, ref jumpPressed, ref jumpHeld, ref interactPressed, ref interactHeld, ref crawlHeld, ref pausePressed);
        return CreateSnapshot(move, jumpPressed, jumpHeld, interactPressed, interactHeld, crawlHeld, pausePressed);
    }

    private CastlePlayerInputSnapshot ReadPlayerTwo()
    {
        Keyboard keyboard = Keyboard.current;
        Vector2 move = Vector2.zero;
        bool jumpPressed = false;
        bool jumpHeld = false;
        bool interactPressed = false;
        bool interactHeld = false;
        bool crawlHeld = false;
        bool pausePressed = false;

        if (keyboard != null)
        {
            move.x += ReadAxis(keyboard.leftArrowKey, keyboard.rightArrowKey);
            jumpPressed |= keyboard.upArrowKey.wasPressedThisFrame;
            jumpHeld |= keyboard.upArrowKey.isPressed;
            crawlHeld |= keyboard.downArrowKey.isPressed;
            interactPressed |= keyboard.rightCtrlKey.wasPressedThisFrame;
            interactHeld |= keyboard.rightCtrlKey.isPressed;
            pausePressed |= keyboard.escapeKey.wasPressedThisFrame;
        }

        AddGamepadInput(1, ref move, ref jumpPressed, ref jumpHeld, ref interactPressed, ref interactHeld, ref crawlHeld, ref pausePressed);
        return CreateSnapshot(move, jumpPressed, jumpHeld, interactPressed, interactHeld, crawlHeld, pausePressed);
    }

    private void AddGamepadInput(int gamepadIndex, ref Vector2 move, ref bool jumpPressed, ref bool jumpHeld, ref bool interactPressed, ref bool interactHeld, ref bool crawlHeld, ref bool pausePressed)
    {
        if (!enableGamepads)
        {
            return;
        }

        IReadOnlyList<Gamepad> gamepads = Gamepad.all;
        if (gamepadIndex < 0 || gamepadIndex >= gamepads.Count)
        {
            return;
        }

        Gamepad gamepad = gamepads[gamepadIndex];
        Vector2 stick = gamepad.leftStick.ReadValue();
        Vector2 dpad = gamepad.dpad.ReadValue();
        move += Mathf.Abs(dpad.x) > Mathf.Abs(stick.x) ? dpad : stick;
        jumpPressed |= gamepad.buttonSouth.wasPressedThisFrame;
        jumpHeld |= gamepad.buttonSouth.isPressed;
        interactPressed |= gamepad.buttonWest.wasPressedThisFrame;
        interactHeld |= gamepad.buttonWest.isPressed;
        crawlHeld |= gamepad.buttonEast.isPressed || dpad.y < -0.5f || stick.y < -0.5f;
        pausePressed |= gamepad.startButton.wasPressedThisFrame;
    }

    private static float ReadAxis(ButtonControl negative, ButtonControl positive)
    {
        float value = 0f;

        if (negative.isPressed)
        {
            value -= 1f;
        }

        if (positive.isPressed)
        {
            value += 1f;
        }

        return value;
    }

    private static CastlePlayerInputSnapshot CreateSnapshot(Vector2 move, bool jumpPressed, bool jumpHeld, bool interactPressed, bool interactHeld, bool crawlHeld, bool pausePressed)
    {
        return new CastlePlayerInputSnapshot(
            Vector2.ClampMagnitude(move, 1f),
            jumpPressed,
            jumpHeld,
            interactPressed,
            interactHeld,
            crawlHeld,
            pausePressed);
    }
}
