using UnityEngine;

public class SoundFeedback : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound, placeSound, removeSound, wrongPlacementSound, purchaseSound;

    [SerializeField] private AudioSource audioSource;

    public void PlaySound(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.Click:
                audioSource.PlayOneShot(clickSound);
                break;
            case SoundType.Place:
                audioSource.PlayOneShot(placeSound);
                break;
            case SoundType.Remove:
                audioSource.PlayOneShot(removeSound);
                break;
            case SoundType.WrongPlacement:
                audioSource.PlayOneShot(wrongPlacementSound);
                break;
            case SoundType.Purchase:
                audioSource.PlayOneShot(purchaseSound);
                break;
            default:
                break;
        }
    }

    public void PlayClickSound()
    {
        PlaySound(SoundType.Click);
    }

    public void PlayPlaceSound()
    {
        PlaySound(SoundType.Place);
    }

    public void PlayRemoveSound()
    {
        PlaySound(SoundType.Remove);
    }

    public void PlayWrongPlacementSound()
    {
        PlaySound(SoundType.WrongPlacement);
    }

    public void PlayPurchaseSound()
    {
        PlaySound(SoundType.Purchase);
    }
}

public enum SoundType
{
    Click,
    Place,
    Remove,
    WrongPlacement,
    Purchase
}