using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Dialogue
{
    public class EmojiFetcher : MonoBehaviour
    {
        private Dictionary<string, Sprite> emojies;

        [SerializeField] private UnityEvent<DialogueData> OnEmojisFetched;

        public void TryGetEmoji(string name, out Sprite sprite)
        {
            sprite = null;

            if (emojies == null)
            {
                Debug.LogError("Emojis dictionary is not initialized.");
                return;
            }

            emojies.TryGetValue(name, out sprite);
        }

        public void FetchEmojies(DialogueData dialogueData)
        {
            if (dialogueData == null || dialogueData.emojies == null)
            {
                Debug.LogError("Emoji data is null or empty.");
                return;
            }

            emojies = new Dictionary<string, Sprite>();

            foreach (var emoji in dialogueData.emojies)
            {
                if (emoji.url != null)
                {
                    // load sprite using web request
                    var webRequest = UnityWebRequestTexture.GetTexture(emoji.url);

                    webRequest.SendWebRequest().completed += _ =>
                    {
                        if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                            webRequest.result == UnityWebRequest.Result.ProtocolError)
                        {
                            Debug.LogError("Error fetching emoji: " + webRequest.error);
                        }
                        else
                        {
                            var texture = DownloadHandlerTexture.GetContent(webRequest);
                            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                Vector2.zero);
                            emojies[emoji.name] = sprite;

                            // invoke event with the fetched data
                            if (emojies.Count == dialogueData.emojies.Count)
                            {
                                OnEmojisFetched?.Invoke(dialogueData);
                            }
                            webRequest.Dispose();
                        }
                    };
                }
            }
        }
    }
}