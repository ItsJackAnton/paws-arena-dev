using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchingForOpponentAnimation : MonoBehaviour
{
    [SerializeField] private PlayerCustomization playerCustomization;
    [SerializeField] private EquipmentsConfig equipmentsConfig;
    [SerializeField] private List<Sprite> groundBack;
    [SerializeField] private List<Sprite> groundFront;

    private IEnumerator Start()
    {
        while (gameObject.activeSelf)
        {
            EquipmentData _equipmentData = equipmentsConfig.Head[Random.Range(0, equipmentsConfig.Head.Count)];
            playerCustomization.SetEquipmentBySprite(EquipmentType.HAT,_equipmentData.Thumbnail);
            
            _equipmentData = equipmentsConfig.Eyes[Random.Range(0, equipmentsConfig.Eyes.Count)];
            playerCustomization.SetEquipmentBySprite(EquipmentType.EYES,_equipmentData.Thumbnail);
            
            _equipmentData = equipmentsConfig.Mouth[Random.Range(0, equipmentsConfig.Mouth.Count)];
            playerCustomization.SetEquipmentBySprite(EquipmentType.MOUTH,_equipmentData.Thumbnail);
            
            _equipmentData = equipmentsConfig.Body[Random.Range(0, equipmentsConfig.Body.Count)];
            playerCustomization.SetEquipmentBySprite(EquipmentType.BODY,_equipmentData.Thumbnail);
            
            playerCustomization.SetEquipmentBySprite(EquipmentType.GROUND_BACK, groundBack[Random.Range(0,groundBack.Count)]);
            playerCustomization.SetEquipmentBySprite(EquipmentType.GROUND_FRONT, groundFront[Random.Range(0,groundFront.Count)]);
            
            switch (Random.Range(0,3))
            {
                case 0:
                    _equipmentData= equipmentsConfig.TailsAnimated[Random.Range(0, equipmentsConfig.TailsAnimated.Count)];
                    break;
                case 1:
                    _equipmentData= equipmentsConfig.TailsFloating[Random.Range(0, equipmentsConfig.TailsFloating.Count)];
                    break;
                case 2:
                    _equipmentData= equipmentsConfig.TailsOverlay[Random.Range(0, equipmentsConfig.TailsOverlay.Count)];
                    break;
            }

            if (_equipmentData!=null)
            {
                playerCustomization.SetEquipmentBySprite(EquipmentType.TAIL,_equipmentData.Thumbnail);
            }
            
            yield return new WaitForSeconds(0.2f);
        }
    }
}
