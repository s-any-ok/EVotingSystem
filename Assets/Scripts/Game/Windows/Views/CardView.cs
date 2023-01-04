using System;
using TMPro;
using UnityEngine;

namespace Game.Windows.Views
{
    public class CardView:MonoBehaviour
    {
        [SerializeField] private TMP_Text _number;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _votes;
        
        public string Name => _title.text;

        public void SetNumber(int value)
        {
            _number.text = $"№{value}";
        }
        
        public void SetTitle(string value)
        {
            _title.text = value;
        }
        
        public void SetVotes(int value)
        {
            _votes.text = $"votes: {value}";
        }
    }
}