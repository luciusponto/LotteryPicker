using UnityEngine;

namespace DefaultNamespace
{
    public class GameSelectionPanel : MonoBehaviour
    {
        public GameObject ButtonPrefab;
        private RuleSetData[] GameTypesTop;

        private void Awake()
        {
            var GameTypes = Resources.LoadAll("GameData");
            foreach (var gameType in GameTypes)
            {
                var button = Instantiate(ButtonPrefab, transform);
                var buttonScript = button.GetComponent<RuleSetButton>();
                buttonScript.Data = gameType as RuleSetData;
            }
            
        }
    }
}