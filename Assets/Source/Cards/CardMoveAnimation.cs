using System.Collections;
using UnityEngine;

namespace Cards
{
    public class CardMoveAnimation : MonoBehaviour
    {
        // this contains within its hierarchy the original deck position
        [SerializeField] private GameObject deckParentSource;
        [SerializeField] private GameObject deckParentTarget;
        [SerializeField] private float animationDuration = 2.0f;
        [SerializeField] private float animationDelay = 1f;
        [SerializeField] private Vector2 translation = Vector2.zero;

        private void Start()
        {
            if (deckParentSource == null || deckParentTarget == null)
                Debug.LogError("Deck Parent Source or Deck Parent Target is not assigned.");
        }

        public void StartAnimation()
        {
            // start the animation
            StartCoroutine(AnimateDeckMovement());
        }

        private IEnumerator AnimateDeckMovement()
        {
            // get all childs from deckParentSource
            var cards = deckParentSource.transform.childCount;
            var currentPosition = deckParentTarget.transform.position;

            for (var i = cards - 1; i >= 0; i--)
            {
                // take the top card
                var card = deckParentSource.transform.GetChild(i);

                // move the card to the deckParentSource position on time
                var startPosition = card.position;
                var targetPosition = currentPosition + (Vector3) translation;

                // update current position
                currentPosition = targetPosition;
                // reparent card
                card.SetParent(deckParentTarget.transform, true);

                yield return StartCoroutine(MoveAnimation(card, startPosition, targetPosition));
            }

            yield return null;
        }

        private IEnumerator MoveAnimation(Transform card, Vector3 startPosition, Vector3 targetPosition)
        {
            // move the card to the deckParentTarget position on time
            var elapsedTime = 0f;

            while (elapsedTime < animationDuration)
            {
                card.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / animationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // set the card to the target position
            card.position = targetPosition;

            // wait before next card animation
            yield return new WaitForSeconds(animationDelay);
        }
    }
}