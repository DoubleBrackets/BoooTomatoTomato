using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using FishNet.Object;
using TMPro;
using UnityEngine;
using Random = System.Random;

namespace Gameplay.GameplaySystems
{
    public class JokeTellManager : NetworkBehaviour
    {
        [Serializable]
        private struct Joke
        {
            [TextArea]
            public string JokeStart;

            [TextArea]
            public string Punchline;
        }

        private List<Joke> _jokes;

        [SerializeField]
        private TMP_Text _jokeText;

        [SerializeField]
        private TMP_Text _jokePunchlineText;

        [SerializeField]
        private float _initialDelay;

        [SerializeField]
        private float _delayBetweenParts;

        [SerializeField]
        private float _lingerTime;

        [ObserversRpc(RunLocally = true)]
        private void TellJoke(string joke, string punchline)
        {
            _jokeText.text = joke;
            _jokePunchlineText.text = punchline;
            ShowJoke(joke, punchline).Forget();
        }

        [Client]
        private async UniTaskVoid ShowJoke(string joke, string punchline)
        {
            await UniTask.WaitForSeconds(_initialDelay);
            _jokeText.text = joke;
            await UniTask.WaitForSeconds(_delayBetweenParts);
            _jokePunchlineText.text = punchline;
            await UniTask.WaitForSeconds(_lingerTime);
            _jokeText.text = "";
            _jokePunchlineText.text = "";
        }

        [Server]
        public void PlayJoke()
        {
            if (_jokes.Count == 0)
            {
                return;
            }

            Joke joke = _jokes[new Random().Next(_jokes.Count)];
            TellJoke(joke.JokeStart, joke.Punchline);
        }
    }
}