using UnityEngine;

public class PlayerPlatformBehaviour : MonoBehaviour
{
    public PlayerCustomization playerCustomization;
    public bool isMyCat = true;
    public GameObject Platform;

    protected virtual void OnEnable()
    {
        if (isMyCat)
        {
            playerCustomization.SetCat(GameState.selectedNFT.imageUrl, GameState.selectedNFT.ids);
        }
        else
        {
            playerCustomization.wrapper.SetActive(false);
        }
    }


    public async void SetCat(string imageUrl)
    {
        NFT nft = new NFT()
        {
            imageUrl = imageUrl
        };

        playerCustomization.wrapper.SetActive(false);

        await nft.GrabImage();

        playerCustomization.wrapper.SetActive(true);
        playerCustomization.SetCat(nft.imageUrl, nft.ids);
    }
}
