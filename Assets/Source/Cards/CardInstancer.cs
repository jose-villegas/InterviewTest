using UnityEngine;
using UnityEngine.Events;

namespace Cards
{
    /// <summary>
    /// Instances cards required for the game and animation requirements.
    /// </summary>
    public class CardInstancer : MonoBehaviour
    {
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private GameObject cardParent;
        [SerializeField] private int numberOfCards = 144;
        [SerializeField] private Vector2 translationPerInstance = new(0.1f, 0.1f);

        public UnityEvent<CardInstancer> OnCardsInstanced;

        private void Start()
        {
            InstanceCards();
        }

        private void InstanceCards()
        {
            if (cardPrefab == null || cardParent == null)
            {
                Debug.LogError("Card Prefab or Card Parent is not assigned.");
                return;
            }

            var currentPosition = cardParent.transform.position;

            for (var i = 0; i < numberOfCards; i++)
            {
                var cardInstance = Instantiate(cardPrefab, cardParent.transform);
                // translate position of new instance by translationPerInstance
                currentPosition.x += translationPerInstance.x;
                currentPosition.y += translationPerInstance.y;
                cardInstance.transform.position = currentPosition;
            }

            // Notify that cards have been instanced
            OnCardsInstanced?.Invoke(this);
        }
    }
}