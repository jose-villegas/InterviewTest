using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Dialogue
{
    /// <summary>
    /// This class obtains avatars from the <see cref="DialogueData"/> and saves their references for ease of usage
    /// </summary>
    public class AvatarFetcher : MonoBehaviour
    {
        public enum AvatarSide
        {
            Left,
            Right
        }
        
        private Dictionary<string, Sprite> avatars;
        private Dictionary<string, AvatarSide> avatarSide;
        
        [SerializeField] private UnityEvent<DialogueData> OnAvatarsFetched;

        public void TryGetAvatar(string name, out Sprite sprite, out AvatarSide side)
        {
            sprite = null;
            side = AvatarSide.Left;

            if (avatars == null || avatarSide == null)
            {
                Debug.LogError("Avatars or AvatarSide dictionaries are not initialized.");
                return;
            }

            avatars.TryGetValue(name, out sprite);
            avatarSide.TryGetValue(name, out side);
        }

        public void FetchAvatars(DialogueData dialogueData)
        {
            if (dialogueData == null || dialogueData.dialogue == null)
            {
                Debug.LogError("Dialogue data is null or empty.");
                return;
            }

            avatars = new Dictionary<string, Sprite>();
            avatarSide = new Dictionary<string, AvatarSide>();

            foreach (var avatar in dialogueData.avatars)
            {
                if (avatar.url != null)
                {
                    // load sprite using web request
                    var webRequest = UnityWebRequestTexture.GetTexture(avatar.url);
                    
                    webRequest.SendWebRequest().completed += _ =>
                    {
                        if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                            webRequest.result == UnityWebRequest.Result.ProtocolError)
                        {
                            Debug.LogError("Error fetching avatar: " + webRequest.error);
                        }
                        else
                        {
                            var texture = ((DownloadHandlerTexture) webRequest.downloadHandler).texture;
                            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                                new Vector2(0.5f, 0.5f));
                            avatars.Add(avatar.name, sprite);
                            
                            switch (avatar.position)
                            {
                                // determine side
                                case "left":
                                    avatarSide.Add(avatar.name, AvatarSide.Left);
                                    break;
                                case "right":
                                    avatarSide.Add(avatar.name, AvatarSide.Right);
                                    break;
                                default:
                                    Debug.LogError("Avatar side is not defined.");
                                    break;
                            }
                        }
                        
                        // invoke event with the fetched data
                        if (avatars.Count == dialogueData.avatars.Count)
                        {
                            OnAvatarsFetched?.Invoke(dialogueData);
                        }
                        webRequest.Dispose();
                    };
                    
                }
                else
                {
                    Debug.LogError("Avatar URL is null.");
                }
            }
        }
    }
}