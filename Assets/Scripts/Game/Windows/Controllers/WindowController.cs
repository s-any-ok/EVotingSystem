using System;
using System.Collections.Generic;
using Game.Candidates.Data;
using Game.DataProvider.Interfaces;
using Game.Vote.Enum;
using Game.Vote.Interfaces;
using Game.Windows.Controllers.Factory;
using Game.Windows.Interfaces;
using Game.Windows.Views;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Windows.Controllers
{
    public class WindowController: IWindowController, IInitializable, IDisposable
    {
        private readonly IDataProviderController _dataProviderController;
        private readonly WindowView _view;
        private readonly IClientVoteController _clientVoteController;
        private readonly IClientStrategies _clientStrategies;
        private readonly IServerVoteController _serverVoteController;
        private readonly CardFactory _cardFactory;
        private Transform _cardContent;

        private IClientStrategy _strategy;
        
        public void Initialize() { }

        public WindowController(IDataProviderController dataProviderController, CardFactory cardFactory, WindowView view,
            IClientVoteController clientVoteController, IClientStrategies clientStrategies, IServerVoteController serverVoteController)
        {
            _view = view;
            _clientVoteController = clientVoteController;
            _clientStrategies = clientStrategies;
            _serverVoteController = serverVoteController;
            _cardFactory = cardFactory;
            _dataProviderController = dataProviderController;
            
            _cardContent = _view.CardContent;
            
            InitializeCards();
            InitializeCandidates();
            InitializeStrategies();

            _view.OnVote += OnVote;
            _view.OnRegister += OnRegister;
            _view.OnLogin += OnLogin;
            _clientVoteController.OnError += OnError;
            _clientStrategies.OnErrorE += OnError;
            _serverVoteController.OnErrorE += OnError;
        }

        private void OnLogin(string login, string password)
        {
            _clientVoteController.LogIn(login, password);
        }

        private void OnRegister(string ipn)
        {
            _clientVoteController.Register(Int32.Parse(ipn));
        }

        private void OnVote(string ipn, string option, string type)
        {
            if (ipn.Length > 0)
            {
                var strings = type.Split("_");
                var len = strings.Length;
                var typeId = strings[len - 1];
                var strategyType = (EStrategy)int.Parse(typeId);
                _strategy = _clientStrategies.GetStrategy(strategyType);
            
                _strategy.OnError += OnError;
                _clientVoteController.SetStrategy(_strategy);
            
                var candidate = _dataProviderController.GetCandidateByName(option);
                int userId = Convert.ToInt32(ipn);
                _clientVoteController.Vote(userId, candidate.Id);
            
                var results = _clientVoteController.GetElectionResults();
                _view.UpdateCards(results);

                Debug.Log($"ipn {ipn}, option {option}");
            }
        }

        private void OnError(string val)
        {
            _view.ShowError(val);
            throw new Exception(val);
        }

        private void InitializeCards()
        {
            var candidates = _dataProviderController.Candidates;
            candidates.ForEach(CreateCard);
        }
        
        private void InitializeCandidates()
        {
            var candidates = _dataProviderController.Candidates;
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            foreach (var candidate in candidates)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(candidate.Name);
                options.Add(option);
            }
            _view.SetOptions(options);
        }

        private void InitializeStrategies()
        {
            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            var values = Enum.GetValues(typeof(EStrategy));
            foreach (var value in values)
            {
                TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData($"{value}_LAB_{value.GetHashCode()}");
                options.Add(option);
            }
            _view.SetLabs(options);
        }

        private void CreateCard(Candidate candidate)
        {
            var card = _cardFactory.Create();
            card.gameObject.transform.SetParent(_cardContent);
            card.SetNumber(candidate.Id);
            card.SetTitle(candidate.Name);
            card.SetVotes(0);
            
            _view.AddCard(card);
        }

        public void Dispose()
        {
            _view.OnVote -= OnVote;
            _view.OnRegister -= OnRegister;
            _view.OnLogin -= OnLogin;
            _strategy.OnError -= OnError;
            _clientVoteController.OnError -= OnError;
            _clientStrategies.OnErrorE -= OnError;
            _serverVoteController.OnErrorE -= OnError;
        }
    }
}