using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Dialogue
{
    [RequireComponent(typeof(AvatarFetcher))]
    public class DialogueEntriesInstancer : MonoBehaviour
    {
        [SerializeField] private GameObject dialogueEntryPrefab;
        [SerializeField] private GameObject dialogueEntriesParent;
        [SerializeField] private AvatarFetcher avatarFetcher;
        
        public UnityEvent<DialogueData> OnDialogueEntriesInstanced;

        public void InstanceDialogueEntries(DialogueData dialogueData)
        {
            if (dialogueEntryPrefab == null || dialogueEntriesParent == null)
            {
                Debug.LogError("Dialogue Entry Prefab or Dialogue Entries Parent is not assigned.");
                return;
            }

            for (var index = 0; index < dialogueData.dialogue.Count; index++)
            {
                var dialogue = dialogueData.dialogue[index];
                var dialogueEntryInstance = Instantiate(dialogueEntryPrefab, dialogueEntriesParent.transform);
                // obtain the dialogue entry view component and set the dialogue
                var dialogueEntryView = dialogueEntryInstance.GetComponent<DialogueEntryView>();

                if (dialogueEntryView != null)
                {
                    dialogueEntryView.Configure(dialogue, avatarFetcher);
                }
                else
                {
                    Debug.LogError("Dialogue Entry View component not found in the dialogue entry prefab.");
                }
                
                // hide all entries except the first one
                if (index > 0)
                {
                    dialogueEntryView.Hide();
                }
            }

            // Notify that dialogue entries have been instanced
            OnDialogueEntriesInstanced?.Invoke(dialogueData);
        }
    }
}