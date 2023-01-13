using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Game.Vote.Data;
using Game.Windows.Data;
using Game.Windows.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Windows.Views
{
    public class WindowView:MonoBehaviour
    {
        public event Action<string,string> OnLogin;
        public event Action<string> OnRegister;
        public event Action<string, string, string> OnVote;
        
        [SerializeField] private List<CardView> _cardViews;
        [SerializeField] private Transform _cardContent;
        [Space(10)]
        [SerializeField] private TMP_InputField _IPN;
        [SerializeField] private TMP_InputField _regsiterIPN;
        [SerializeField] private TMP_InputField _login;
        [SerializeField] private TMP_InputField _password;
        [SerializeField] private TMP_Dropdown _selectOption;
        [SerializeField] private TMP_Dropdown _selectLab;
        [SerializeField] private RectTransform _errorLabel;
        [SerializeField] private TMP_Text _errorText;

        [Header("Forms")]
        [SerializeField] private Button _registerFormButton;
        [SerializeField] private Button _loginFormButton;
        [SerializeField] private Button _voteFormButton;
        [Space(10)]
        [SerializeField] private Button _registerButton;
        [SerializeField] private Button _loginButton;
        [SerializeField] private Button _voteButton;
        
        [SerializeField] private List<FormData> _forms;

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
            _registerButton.onClick.AddListener(() => OnRegister?.Invoke(_regsiterIPN.text));
            _loginButton.onClick.AddListener(() => OnLogin?.Invoke(_login.text, _password.text));
            
            _registerFormButton.onClick.AddListener(() => SetForm(EForm.REGISTER));
            _loginFormButton.onClick.AddListener(() => SetForm(EForm.LOGIN));
            _voteFormButton.onClick.AddListener(() => SetForm(EForm.VOTE));
        }

        private void SetForm(EForm type)
        {
            foreach (var form in _forms)
            {
                form.GameObject.SetActive(form.Type == type);
            }
        }

        private void OnDisable()
        {
            _voteButton.onClick.RemoveAllListeners();
            _registerButton.onClick.RemoveAllListeners();
            _loginButton.onClick.RemoveAllListeners();
            _registerFormButton.onClick.RemoveAllListeners();
            _loginFormButton.onClick.RemoveAllListeners();
            _voteFormButton.onClick.RemoveAllListeners();
        }
    }
}