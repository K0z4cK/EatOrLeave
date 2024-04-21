using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    private enum ItemKeys
    {
        Water = 1,
        EnergyDrink = 2,
        Bin = 3,
        Needle = 4
    }

    [SerializeField, Range(0f, 1f)] private float afterUseTransparancy;
    [SerializeField] private Image waterImage;
    [SerializeField] private Image energyDrinkImage;
    [SerializeField] private Image binImage;
    [SerializeField] private Image needleImage;

    private bool _isWaterItemAvailable;
    private bool _isEnergyDrinkItemAvailable;
    private bool _isBinItemAvailable;
    private bool _isNeedleItemAvailable;

    private int _weight;
    private const int _weightLimit = 666;

    private int WaterWeightDecrease => (int)(_weight / 10);

    public int Weigth
    {
        get => _weight;
        set
        {
            _weight = Mathf.Clamp(value, 50, _weightLimit);

            if (_weight >= _weightLimit) {
                print("OBESE FUCK JUST DIED! YOU LOSE");
            }
        }
    }

    private void Start()
    {
        InitiateItemsStatus();
        _weight = 200;
    }

    private void InitiateItemsStatus()
    {
         _isWaterItemAvailable = true;
         _isEnergyDrinkItemAvailable = true;
         _isBinItemAvailable = true;
         _isNeedleItemAvailable = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            UseWater();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            UseEnergyDrink();
        if (Input.GetKeyDown(KeyCode.Alpha3))
            UseBin();
        if (Input.GetKeyDown(KeyCode.Alpha4))
            UseNeedle();
    }

    private void UseWater()
    {
        if (!_isWaterItemAvailable)
            return;

        Weigth -= WaterWeightDecrease;
        ConsumeItem(waterImage, ref _isWaterItemAvailable);
    }

    private void UseEnergyDrink()
    {
        if (!_isEnergyDrinkItemAvailable)
            return;
        // + more time than water and + calories
        // 30% 
        ConsumeItem(energyDrinkImage, ref _isEnergyDrinkItemAvailable);
    }

    private void UseBin()
    {
        if (!_isBinItemAvailable)
            return;

        ConsumeItem(binImage, ref _isBinItemAvailable);
    }

    private void UseNeedle()
    {
        if (!_isNeedleItemAvailable)
            return;

        ConsumeItem(needleImage, ref _isNeedleItemAvailable);
    }

    private void ConsumeItem(Image itemImage, ref bool availavailabilityStatus)
    {
        itemImage.color = new(itemImage.color.r, itemImage.color.g, itemImage.color.b, afterUseTransparancy);
        availavailabilityStatus = false;
    }
}
