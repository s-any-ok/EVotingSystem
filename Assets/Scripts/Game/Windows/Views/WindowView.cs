using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.Vote.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Windows.Views
{
    public class WindowView:MonoBehaviour
    {
        public event Action<string, string, string> OnVote;
        
        [SerializeField] private List<CardView> _cardViews;
        [SerializeField] private Transform _cardContent;
        [Space(10)]
        [SerializeField] private Button _voteButton;
        [SerializeField] private TMP_InputField _IPN;
        [SerializeField] private TMP_Dropdown _selectOption;
        [SerializeField] private TMP_Dropdown _selectLab;
        [SerializeField] private RectTransform _errorLabel;
        [SerializeField] private TMP_Text _errorText;

        public Transform CardContent => _cardContent;
        
        public void UpdateCards(IEnumerable<ElectionResultsData> votingResults)
        {
            var votingResultsData = votingResults.ToList();
            for (int i = 0; i < votingResultsData.Count(); i++)
            {
                var votingResult = votingResultsData[i];
                var cardView = _cardViews.FirstOrDefault(x =>x.Name == votingResult.CandidateName);

                if (cardView != null)
                {
                    cardView.SetNumber(votingResult.CandidateId);
                    cardView.SetTitle(votingResult.CandidateName);
                    cardView.SetVotes(votingResult.Votes);
                }
            }
        }

        public void ShowError(string text)
        {
            _errorText.text = text;
            Sequence mySequence = DOTween.Sequence();
            mySequence
                .Append(_errorLabel.DOAnchorPosX(-400, 1F))
                .AppendInterval(3)
                .Append(_errorLabel.DOAnchorPosX(400, 1F));
        }

        public void AddCard(CardView cardView)
        {
            _cardViews.Add(cardView);
        }
        
        public void SetLabs(List<TMP_Dropdown.OptionData> optionData)
        {
            _selectLab.options = optionData;
        }

        public void SetOptions(List<TMP_Dropdown.OptionData> optionData)
        {
            _selectOption.options = optionData;
        }
        
        private void OnEnable()
        {
            _voteButton.onClick.AddListener(() => 
                OnVote?.Invoke(_IPN.text, _selectOption.captionText.text, _selectLab.captionText.text));
        }

        private void OnDisable()
        {
            _voteButton.onClick.RemoveAllListeners();
        }
    }
}