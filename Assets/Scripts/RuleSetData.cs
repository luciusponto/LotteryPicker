using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "New Rule Set", menuName = "Rule Set/New", order = 0)]
    public class RuleSetData : ScriptableObject
    {
        public string DisplayName;
        public Sprite Icon;
        public Color BackgroundColor = Color.white;
        public NumberSet[] NumberSets;
        public bool LogChecks;
        
        [Serializable]
        public struct NumberSet
        {
            public int Quantity;
            public int RangeStart;
            public int RangeEnd;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            foreach (var numberSet in NumberSets)
            {
                builder.Append($"{numberSet.Quantity} x {numberSet.RangeStart}-{numberSet.RangeEnd}\n");
            }
            return builder.ToString().Trim();
        }

        public bool IsCompleted(List<List<int>> results)
        {
            if (results.Count < NumberSets.Length)
            {
                if (LogChecks)
                {
                    Debug.Log($"Results count: {results.Count}");
                }
                return false;
            }

            for (var i = 0; i < NumberSets.Length; i++)
            {
                if (!CompletedRow(i, results))
                {
                    return false;
                }
            }
            
            return true;
        }

        public bool CompletedRow(int numSetIndex, List<List<int>> results)
        {
            var row = results[numSetIndex];
            var expectedRowSize = NumberSets[numSetIndex].Quantity;
            var actualRowSize = row.Count;
            var result = actualRowSize == expectedRowSize;
            if (LogChecks)
            {
                Debug.Log($"Completed row {numSetIndex}: {result}. Expected: {expectedRowSize}. Actual: {actualRowSize}");
            }
            return result;
        }
    }
}