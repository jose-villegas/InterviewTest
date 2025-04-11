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
        [SerializeField] private Image emojiContainer;
        [SerializeField] private Transform avatarContainer;
        [SerializeField] private float _toggleDuration = 0.25f;

        private string dialogueText;
        private CanvasGroup canvasGroup;
        private LayoutElement layoutElement;
        private EmojiFetcher emojiFetcher;

        private void Awake()
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

        public void Configure(Dialogue dialogue, AvatarFetcher avatarFetcher, EmojiFetcher emojiFetcher)
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
                textLabel.text = string.Empty;
                dialogueText = dialogue.text;
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

            // save the emoji fetcher reference
            this.emojiFetcher = emojiFetcher;
        }

        private IEnumerator TypeWriterAnimation(string text)
        {
            textLabel.text = string.Empty;
            bool skipping = false;
            var currentEmoji = string.Empty;

            for (var index = 0; index < text.Length; index++)
            {
                var letter = text[index];
                // detect emoji token and skip
                if (letter == '{')
                {
                    // skip to the next
                    skipping = true;
                    currentEmoji = string.Empty;
                    continue;
                }

                if (letter == '}')
                {
                    // skip to the next
                    skipping = false;
                    // set emoji container
                    StartCoroutine(SetCurrentEmoji(currentEmoji));
                    continue;
                }

                if (skipping)
                {
                    currentEmoji += letter;
                    continue;
                }

                textLabel.text += letter;
                yield return new WaitForSeconds(0.05f);
            }
        }

        private IEnumerator SetCurrentEmoji(string name)
        {
            // fetch the emoji
            if (emojiFetcher != null)
            {
                emojiFetcher.TryGetEmoji(name, out var sprite);
                
                if (sprite != null)
                {
                    // set image sprite
                    emojiContainer.sprite = sprite;
                    emojiContainer.gameObject.SetActive(true);
                    yield return AnimateEmojiCanvas();
                }
                else
                {
                    Debug.LogError($"Emoji not found for {name}");
                }
            }
            else
            {
                Debug.LogError("Emoji fetcher not found.");
            }
        }

        private IEnumerator AnimateEmojiCanvas()
        {
            // lerp alpha for emoji
            var elapsedTime = 0f;
            var startAlpha = 0f;
            var targetAlpha = 1f;

            while (elapsedTime < _toggleDuration)
            {
                var color = emojiContainer.color;
                color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / _toggleDuration);
                emojiContainer.color = color;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // set the emoji to the target alpha
            var finalColor = emojiContainer.color;

            // wait for a while
            yield return new WaitForSeconds(2f);
                    
            // hide the emoji
            elapsedTime = 0f;
            startAlpha = 1f;
            targetAlpha = 0f;
                    
            while (elapsedTime < _toggleDuration)
            {
                var color = emojiContainer.color;
                color.a = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / _toggleDuration);
                emojiContainer.color = color;
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // set the emoji to the target alpha
            finalColor = emojiContainer.color;
            finalColor.a = 0f;
            emojiContainer.color = finalColor;
                    
            // set the emoji to inactive
            emojiContainer.gameObject.SetActive(false);
            emojiContainer.sprite = null;
        }

        public void Hide()
        {
            StartCoroutine(ToggleCoroutine(false));
        }

        public void Show()
        {
            StartCoroutine(ToggleCoroutine(true));
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

            // trigger typewriter animation
            if (toggled)
            {
                StartCoroutine(TypeWriterAnimation(dialogueText));
            }

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