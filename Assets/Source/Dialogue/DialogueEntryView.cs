using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    public class DialogueEntryView : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private TMP_Text textLabel;
        [SerializeField] private Image avatarImage;
        [SerializeField] private Transform avatarContainer;
        [SerializeField] private float _toggleDuration = 0.25f;
        
        private CanvasGroup canvasGroup;
        private LayoutElement layoutElement;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            
            if (canvasGroup == null)
            {
                Debug.LogError("CanvasGroup component not found.");
            }
            
            layoutElement = GetComponent<LayoutElement>();
            
            if (layoutElement == null)
            {
                Debug.LogError("LayoutElement component not found.");
            }
        }

        public void Configure(Dialogue dialogue, AvatarFetcher avatarFetcher)
        {
            if (nameLabel != null)
            {
                nameLabel.text = dialogue.name;
            }
            else
            {
                Debug.LogError("Name label not found in the dialogue entry prefab.");
            }

            if (textLabel != null)
            {
                textLabel.text = dialogue.text;
            }
            else
            {
                Debug.LogError("Text label not found in the dialogue entry prefab.");
            }

            if (avatarImage != null)
            {
                // Assuming you have a method to set the avatar image
                SetAvatarImage(dialogue.name, avatarFetcher);
            }
        }

        public IEnumerator Hide()
        {
            yield return StartCoroutine(ToggleCoroutine(false));
        }
        
        public IEnumerator Show()
        {
            yield return StartCoroutine(ToggleCoroutine(true));
        }

        public IEnumerator ToggleCoroutine(bool toggled)
        {
            if (canvasGroup == null)
            {
                Debug.LogError("CanvasGroup component not found.");
                yield break;
            }

            float targetAlpha = toggled ? 1f : 0f;
            float startAlpha = canvasGroup.alpha;
            float elapsedTime = 0f;

            while (elapsedTime < _toggleDuration)
            {
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / _toggleDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;

            if (layoutElement != null)
            {
                layoutElement.ignoreLayout = !toggled;
            }
        }

        private void SetAvatarImage(string name, AvatarFetcher avatarFetcher)
        {
            if (avatarFetcher != null)
            {
                avatarFetcher.TryGetAvatar(name, out var sprite, out var side);
                if (sprite != null)
                {
                    // set image sprite
                    avatarImage.sprite = sprite;
                }
                else
                {
                    Debug.LogError($"Avatar not found for {name}");
                }
                
                // depending on the side we set the avatar gameobject first or last in hierarchy
                if (side == AvatarFetcher.AvatarSide.Left)
                {
                    avatarContainer.SetAsFirstSibling();
                }
                else
                {
                    avatarContainer.transform.SetAsLastSibling();
                }
            }
            else
            {
                Debug.LogError("Avatar fetcher not found.");
            }
        }
    }
}