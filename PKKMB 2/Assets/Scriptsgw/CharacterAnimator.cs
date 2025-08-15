using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    private Animator mAnimator;
    private Vector3 lastPosition;
    private float movementThreshold = 0.00001f;
    private bool isWalking = false;

    void Start()
    {
        mAnimator = GetComponent<Animator>();
        lastPosition = transform.position;
    }

    void Update()
    {
        if (mAnimator != null)
        {
            float distance = Vector3.Distance(transform.position, lastPosition);

            if (distance > movementThreshold && !isWalking)
            {
                // mAnimator.ResetTrigger("Idle");
                mAnimator.SetTrigger("Walk");
                isWalking = true;
            }
            else if (distance <= movementThreshold && isWalking)
            {
                mAnimator.ResetTrigger("Walk");
                // mAnimator.SetTrigger("Idle");
                isWalking = false;
            }

            lastPosition = transform.position;
        }
    }
}
