using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    [RequireComponent(typeof(Button))]
    public class RuleSetButton : MonoBehaviour
    {
        public RuleSetData Data;
        public Button Button;
        public Image BackgroundImage;
        public TextMeshProUGUI TextMesh;
        public Image IconImage;

        private void Awake()
        {
            Button.onClick = new Button.ButtonClickedEvent();
        }

        private void Start()
        {
            BackgroundImage.color = Data.BackgroundColor;
            var hasIcon = Data.Icon != null;
            TextMesh.enabled = !hasIcon;
            IconImage.enabled = hasIcon;
            
            if (hasIcon)
            {
                SetIcon();
            }
            else
            {
                SetLabel();
            }
        }

        private void SetIcon()
        {
            IconImage.sprite = Data.Icon;
        }

        private void SetLabel()
        {
            if (TextMesh != null)
            {
                TextMesh.text = Data.DisplayName;
            }
        }

        private void OnEnable()
        {
            Button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            Button.onClick.RemoveListener(OnClick);
        }

        public void OnClick()
        {
            Controller.GetInstance().SetRuleSet(Data);
            Controller.GetInstance().StartRunning();
        }

    }
}