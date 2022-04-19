using UnityEngine;
using UnityEngine.UI;

public class TriangleSlideController : SimulationSlideController
{
    [SerializeField] private Image dA;
    [SerializeField] private Image vdt;
    [SerializeField] private Color vdtColor = Color.black;

    private TriangleAnimation triangleAnimation;

    private bool canEnable;  // Used for avoiding the first OnEnable() call

    private void Awake()
    {
        triangleAnimation = (TriangleAnimation)simulation;
    }

    // Note: OnEnable() gets called once at the start before SlideManager disables all SlideControllers in Awake()
    private void OnEnable()
    {
        if (!canEnable)
        {
            canEnable = true;
            return;
        }

        triangleAnimation.Reset();

    }

    // TODO Fix this hacky solution
    private void Update()
    {
        if (triangleAnimation.percentComplete < 1)
        {
            float alpha = Mathf.Max(0, 2.5f * (triangleAnimation.percentComplete - 0.4f));

            if (dA != null)
            {
                Color color = dA.color;
                color.a = alpha;
                dA.color = color;
            }

            if (vdt != null)
            {
                Color color = vdt.color;
                color.a = alpha;
                vdt.color = color;
            }
        }
    }

    public override void ShowAndHideUIElements()
    {
        if (vdt)
        {
            vdt.color = vdtColor;
        }
    }
}
