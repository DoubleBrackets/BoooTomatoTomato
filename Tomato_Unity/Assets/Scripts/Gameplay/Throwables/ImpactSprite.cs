using System;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

namespace Gameplay.Throwables
{
    public class ImpactSprite : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private List<SpriteInfo> _spriteInfos;

        [SerializeField]
        private TweenSettings _fadeTweenSettings;

        private Color _clearColor = new(1f, 1f, 1f, 0f);

        [Serializable]
        public struct SpriteInfo
        {
            public string SpriteName;
            public Sprite Sprite;
        }

        public void Initialize(string spriteName)
        {
            SpriteInfo spriteInfo = _spriteInfos.Find(info => info.SpriteName.Equals(spriteName));
            _spriteRenderer.sprite = spriteInfo.Sprite;

            Tween tween = Tween.Color(_spriteRenderer,
                new TweenSettings<Color>(Color.white, _clearColor, _fadeTweenSettings));
            tween.OnComplete(OnFadeComplete);
        }

        private void OnFadeComplete()
        {
            Destroy(gameObject);
        }
    }
}