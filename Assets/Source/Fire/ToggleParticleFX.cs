using System;
using UnityEngine;
using UnityEngine.UI;

namespace Fire
{
    /// <summary>
    /// Handles the toggling of particle effects using a button.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ToggleParticleFX : MonoBehaviour
    {
        [SerializeField] private Animator animator;
       
        private Button button;
        private int toggleParameter;

        private void Awake()
        {
            button = GetComponent<Button>();
            
            if (animator == null)
            {
                Debug.LogError("Animator component not found.");
                return;
            }
            
            // check if the button is assigned
            if (button == null)
            {
                Debug.LogError("Button component not found.");
                return;
            }
            
            // obtain toggle parameter from animator
            var animatorController = animator.runtimeAnimatorController;
            if (animatorController == null)
            {
                Debug.LogError("Animator Controller not found.");
                return;
            }

            toggleParameter = Animator.StringToHash("Toggle");
        }

        private void Start()
        {
            // subscribe events
            button.onClick.AddListener(ToggleParticleFXState);
        }

        private void OnDestroy()
        {
            // unsubscribe events
            if (button != null)
            {
                button.onClick.RemoveListener(ToggleParticleFXState);
            }
        }

        private void ToggleParticleFXState()
        {
            var isOn = animator.GetBool(toggleParameter);
            animator.SetBool(toggleParameter, !isOn);
        }
    }
}