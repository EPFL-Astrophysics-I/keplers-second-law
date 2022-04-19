using UnityEngine;
using UnityEngine.UI;

public class SectorSlideController : SimulationSlideController
{
    [SerializeField] private Image A;

    private SectorAnimation sectorAnimation;

    private bool canEnable;  // Used for avoiding the first OnEnable() call

    private void Awake()
    {
        sectorAnimation = (SectorAnimation)simulation;
    }

    // Note: OnEnable() gets called once at the start before SlideManager disables all SlideControllers in Awake()
    private void OnEnable()
    {
        if (!canEnable)
        {
            canEnable = true;
            return;
        }

        sectorAnimation.Reset();

    }

    // TODO Fix this hacky solution
    private void Update()
    {
        if (sectorAnimation.percentComplete < 1)
        {
            float alpha = Mathf.Max(0, 2.5f * (sectorAnimation.percentComplete - 0.4f));

            if (A)
            {
                Color color = A.color;
                color.a = alpha;
                A.color = color;
            }
        }
    }

    public override void ShowAndHideUIElements()
    {

    }
}
