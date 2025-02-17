using UnityEngine;
using UnityEngine.InputSystem;

public enum SlashState
{
    Windup,
    Release,
    Rest
};

public enum SlashDirection
{
    Upper       = -90,
   RightUp      = -45,
   RightToLeft  = 0,
   RightDown    = 45,
   StraightDown = 90,
   LeftDown     = 135,
   LeftToRight  = 180,
   LeftUp       = -135,
   Neutral      = 1
};

public class HeroControllor : MonoBehaviour
{
    private CharacterController characterController;
    public float speed = 0.01f;
    private Vector2 loadDirection = Vector2.zero;
    private Vector2 slashDirection = Vector2.zero;
    private float longestWindup = 0;
    private const float MIN_WINDUP_LENGTH = 0.25f;
    private SlashState state = SlashState.Windup;
    private SlashDirection slashState = SlashDirection.Neutral;
    
    private void OnEnable()
    {
        var playerInput = GetComponent<PlayerInput>();
        playerInput.onActionTriggered += OnActionTriggered;
        characterController = GetComponent<CharacterController>();
    }

    private void OnDisable()
    {
        var playerInput = GetComponent<PlayerInput>();
        playerInput.onActionTriggered -= OnActionTriggered;
    }

    private void OnActionTriggered(InputAction.CallbackContext context)
    {
        if (context.action.name == "Move")
        {
            Vector2 move = context.ReadValue<Vector2>();
            characterController.Move(move* speed);
            // Handle movement
        }
        else if (context.action.name == "Jump")
        {
            // Handle jump
        }
    }

    public void MoveHorizontal(InputAction.CallbackContext context)
    {
        if (context.action.name == "Move")
        {
            Vector2 move = context.ReadValue<Vector2>();
                characterController.Move(move * speed);
            // Handle movement
        }
    }

    public void Update()
    {
        
        //Vector2 move = context.ReadValue<Vector2>();
        Vector2 move = new Vector2(0, 0);
        float newLength = move.magnitude;
        //Debug.Log($" length: {newLength}");
        if (newLength < 0.05f)
            return;

        switch (state)
        {
            case SlashState.Windup:
                if (newLength >= longestWindup)
                {
                    longestWindup = newLength;
                    loadDirection = move;
                    //Debug.Log($"loading up ");
                }
                else if (newLength < MIN_WINDUP_LENGTH && longestWindup > MIN_WINDUP_LENGTH)
                {
                    longestWindup = 0;
                    state = SlashState.Release;
                    //Decide slash state
                    slashState = FindSlashState();
                    //Debug.Log($"aiming to {slashState}");
                }
                break;

            case SlashState.Release:
                if (newLength > longestWindup)
                {
                    longestWindup = newLength;
                    slashDirection = move;
                    //Debug.Log($"ready to release ");
                }
                else if (newLength < longestWindup)
                {
                    //Check if load and release angle is correct
                    float angle = Vector2.Angle(loadDirection, slashDirection);
                    if (angle < 160)
                    {
                        //Fail
                        longestWindup = 0;
                        loadDirection = Vector2.zero;
                        slashDirection = Vector2.zero;
                        state = SlashState.Rest;

                        return;
                    }

                    //Get power
                    float power = loadDirection.magnitude + longestWindup;

                    //SLASH!!!!
                    Debug.Log($"Slash type: {slashState}, power: {power}");
                    state = SlashState.Rest;
                }
                break;

            case SlashState.Rest:
                if (newLength < MIN_WINDUP_LENGTH)
                {
                    //Debug.Log($"Reset");
                    state = SlashState.Windup;
                    longestWindup = 0;
                    loadDirection = Vector2.zero;
                    slashDirection = Vector2.zero;
                }
                break;
        }

        //Debug.Log($"Slash type: {state}");
    }

    public void Slash(InputAction.CallbackContext context)
    {
        //Debug.Log($"called");

         Vector2 move = context.ReadValue<Vector2>();
        float newLength = move.magnitude;
        //Debug.Log($" length: {newLength}");
        

         switch(state)
         {
             case SlashState.Windup:
                 if (newLength >= longestWindup)
                 {
                     longestWindup = newLength;
                     loadDirection = move;
                     //Debug.Log($"loading up ");
                 }
                 else if (newLength < MIN_WINDUP_LENGTH && longestWindup > MIN_WINDUP_LENGTH)
                 {
                     longestWindup = 0;
                     state = SlashState.Release;
                     //Decide slash state
                     slashState = FindSlashState();
                     //Debug.Log($"aiming to {slashState}");
                 }
             break;
                 
             case SlashState.Release:
                 if (newLength > longestWindup)
                 {
                     longestWindup = newLength;
                     slashDirection = move;
                     //Debug.Log($"ready to release ");
                 }
                 else if(newLength < longestWindup)
                 {
                     //Check if load and release angle is correct
                     float angle = Vector2.Angle(loadDirection, slashDirection);
                     if (angle < 160)
                     {
                         //Fail
                         longestWindup = 0;
                         loadDirection = Vector2.zero;
                         slashDirection = Vector2.zero;
                         state = SlashState.Rest;

                         return;
                     }

                     //Get power
                     float power = loadDirection.magnitude + longestWindup;

                     //SLASH!!!!
                     Debug.Log($"Slash type: {slashState}, power: {power}");
                     state = SlashState.Rest;
                 }
                 break;

             case SlashState.Rest:
                 if (newLength < MIN_WINDUP_LENGTH)
                 {
                     //Debug.Log($"Reset");
                     state = SlashState.Windup;
                     longestWindup = 0;
                     loadDirection = Vector2.zero;
                     slashDirection = Vector2.zero;
                 }
             break;
         }

        //Debug.Log($"Slash type: {state}");

    }

    private SlashDirection FindSlashState()
    {
        SlashDirection slash = new SlashDirection();

        //float angle = Vector2.Angle(loadDirection, Vector2.right);
        float angle = Mathf.Atan2(loadDirection.y, loadDirection.x) * Mathf.Rad2Deg;
        int enumValue = Mathf.RoundToInt(angle / 45) * 45;
        enumValue = (enumValue == -180) ? 180 : enumValue;
        slash = (SlashDirection)enumValue;
        return slash;
    }

}
