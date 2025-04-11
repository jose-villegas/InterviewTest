using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    public class StepDialogueButtons : MonoBehaviour
    {
        [SerializeField] private RectTransform content;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button previousButton;
        [SerializeField] private float animationDuration = 0.5f;

        private int _currentIndex = 0;

        private void Start()
        {
            // assign the button click events
            if (nextButton != null || previousButton != null)
            {
                nextButton.onClick.AddListener(ShowNextEntry);
                previousButton.onClick.AddListener(ShowPreviousEntry);
            }
            else
            {
                Debug.LogError("Next or Previous button is not assigned.");
            }

            CheckIndexBounds();
        }

        public void CheckIndexBounds()
        {
            // check if the index is out of bounds
            previousButton.interactable = _currentIndex > 0;
            nextButton.interactable = _currentIndex < content.childCount - 1;
        }

        public void ShowNextEntry()
        {
            if (content == null)
            {
                Debug.LogError("Content rect is not assigned.");
                return;
            }

            // disable the button to prevent multiple clicks
            nextButton.interactable = false;
            previousButton.interactable = false;
            // scroll horizontally to the next dialogue entry
            StartCoroutine(AnimateToEntry(_currentIndex + 1));
        }

        public void ShowPreviousEntry()
        {
            if (content == null)
            {
                Debug.LogError("Content rect is not assigned.");
                return;
            }

            // disable the button to prevent multiple clicks
            previousButton.interactable = false;
            nextButton.interactable = false;
            // scroll horizontally to the next dialogue entry
            StartCoroutine(AnimateToEntry(_currentIndex - 1));
        }

        private IEnumerator AnimateToEntry(int targetIndex)
        {
            // hide current entry
            var currentEntry = content.GetChild(_currentIndex);
            var targetEntry = content.GetChild(targetIndex);
            
            // obtain dialogue entry
            var currentEntryRect = currentEntry.GetComponent<DialogueEntryView>();
            var targetEntryRect = targetEntry.GetComponent<DialogueEntryView>();
            
            if (currentEntryRect == null || targetEntryRect == null)
            {
                Debug.LogError("Dialogue Entry View component not found.");
                yield break;
            }
            
            // disable buttons to prevent multiple clicks
            nextButton.interactable = false;
            previousButton.interactable = false;
            
            // hide current entry
            currentEntryRect.Hide();
            // show target entry
            targetEntryRect.Show();
            
            // wait for the animation to finish
            yield return new WaitForSeconds(animationDuration);
            
            // update the current index
            _currentIndex = targetIndex;
            CheckIndexBounds();
        }
    }
}