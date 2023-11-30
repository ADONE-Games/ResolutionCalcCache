using System.Collections;
using System.Collections.Generic;

using TMPro;

using UnityEngine;

namespace TestSample
{
    [RequireComponent( typeof( TextMeshProUGUI ) )]
    public class FrameText : MonoBehaviour
    {
        private TextMeshProUGUI _textMeshProUGUICache = null;

        private void FixedUpdate()
        {
            _textMeshProUGUICache = _textMeshProUGUICache != null ? _textMeshProUGUICache : GetComponent<TextMeshProUGUI>();

            _textMeshProUGUICache.text = $"{Time.frameCount}";
        }
    }

}
