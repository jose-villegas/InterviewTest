using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace Dialogue
{
    /// <summary>
    /// Utility class to extract dialogue data from the game API web endpoint
    /// </summary>
    public class FetchDialogueData : MonoBehaviour
    {
        [SerializeField] private string _dialogueEndPoint =
            "https://private-624120-softgamesassignment.apiary-mock.com/v2/magicwords";

        public UnityEvent<DialogueData> OnDialogueFetched;

        private void Start()
        {
            // Fetch the dialogue data from the endpoint
            StartCoroutine(FetchData(_dialogueEndPoint));
        }

        private IEnumerator FetchData(string dialogueEndPoint)
        {
            using (var webRequest = UnityWebRequest.Get(dialogueEndPoint))
            {
                // Send the request and wait for a response
                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                    webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError("Error fetching data: " + webRequest.error);
                }
                else
                {
                    // Parse the JSON data
                    var jsonData = webRequest.downloadHandler.text;
                    var dialogueData = JsonConvert.DeserializeObject<DialogueData>(jsonData);

                    Debug.Log("Fetched Data: " + jsonData);
                    
                    // invoke event with the fetched data
                    if (dialogueData != null)
                    {
                        OnDialogueFetched?.Invoke(dialogueData);
                    }
                    else
                    {
                        Debug.LogError("Failed to parse dialogue data.");
                    }
                }
            }
        }
    }
}