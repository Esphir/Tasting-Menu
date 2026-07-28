// One room's tile in the minimap.
using UnityEngine;
using UnityEngine.UI;

namespace Signal.Minimap
{
    [RequireComponent(typeof(RectTransform))]
    public sealed class MinimapRoomUI : MonoBehaviour
    {
        private RectTransform _rect;
        private MinimapDatabase _db;
        private Image _background, _border, _icon, _indicator;
        private MinimapIcon _iconDef;

        private float _reveal;
        private float _targetReveal;
        private float _scale = 1f;
        private float _targetScale = 1f;
        private float _opacity = 1f;
        private float _brightness = 1f;
        private bool _pulse;
        private bool _settled;
        private float _appliedReveal = float.NaN;

        // Below this the easing is visually done; without a cutoff Mathf.Lerp only ever approaches
        // its goal, so the tile would repaint forever and never reach the settled state.
        private const float ScaleEpsilon = 0.0005f;

        public void Build(MinimapDatabase db, float tileSize, float iconSize)
        {
            _db = db;
            _rect = GetComponent<RectTransform>();
            _rect.sizeDelta = new Vector2(tileSize, tileSize);

            _background = MakeImage("Background", tileSize);
            _indicator = MakeImage("Indicator", tileSize * 1.25f);
            _icon = MakeImage("Icon", iconSize);
            _border = MakeImage("Border", tileSize);

            if (_db.border != null) _border.sprite = _db.border;
            if (_db.currentIndicator != null) _indicator.sprite = _db.currentIndicator;
        }

        private Image MakeImage(string childName, float size)
        {
            var go = new GameObject(childName, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            var rt = go.GetComponent<RectTransform>();
            rt.SetParent(_rect, false);
            rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(size, size);
            rt.anchoredPosition = Vector2.zero;
            var img = go.GetComponent<Image>();
            img.raycastTarget = false;
            return img;
        }

        public void SetState(MinimapRoom room, float opacity, bool animate, bool pulse, bool revealAll)
        {
            _opacity = opacity;
            _pulse = pulse && room.IsCurrentRoom;
            _settled = false;
            // Invalidate the repaint cache: opacity, brightness and the icon can all have changed
            // here, and the hidden early-return below leaves without repainting at all.
            _appliedReveal = float.NaN;

            if (!room.IsVisible && !revealAll)
            {
                _targetReveal = 0f;
                if (!animate) { _reveal = 0f; gameObject.SetActive(false); }
                return;
            }

            gameObject.SetActive(true);
            _targetReveal = 1f;

            _brightness = room.IsVisited || room.IsCurrentRoom ? 1f : 0.45f;
            _targetScale = room.IsCurrentRoom ? 1.12f : 1f;

            _background.sprite = _db.Background(room.IsCurrentRoom, room.IsVisited, room.IsDiscovered || revealAll);

            _iconDef = _db.GetIcon(room.RoomType);
            bool hasIcon = _iconDef != null && _iconDef.sprite != null;
            _icon.enabled = hasIcon;
            if (hasIcon) _icon.sprite = _iconDef.sprite;

            _border.enabled = _db.border != null;
            _indicator.enabled = room.IsCurrentRoom && _db.currentIndicator != null;

            if (!animate) { _reveal = 1f; _scale = _targetScale; }
            ApplyColours();
            ApplyTransform();
            _appliedReveal = _reveal;
        }

        private void Update()
        {
            float step = Time.unscaledDeltaTime;
            _reveal = Mathf.MoveTowards(_reveal, _targetReveal, step * 4f);

            float scaleGoal = _targetScale;
            if (_pulse) scaleGoal += 0.06f * Mathf.Sin(Time.unscaledTime * 5f);
            // Frame-rate-independent easing: a plain Lerp with a constant t eases faster the higher
            // the frame rate, so derive t from elapsed time as 1 - e^(-k·dt). Same feel at any fps.
            _scale = Mathf.Lerp(_scale, scaleGoal, 1f - Mathf.Exp(-step * 10f));

            if (_targetReveal == 0f && _reveal <= 0.001f)
            {
                if (gameObject.activeSelf) gameObject.SetActive(false);
                return;
            }

            // Once the reveal and scale have finished animating and this isn't the pulsing current
            // room, the tile is a static image. Writing Image.color calls SetVerticesDirty, so
            // repainting all four graphics every frame — across every room on the map — forced the
            // minimap canvas to rebuild every frame for no visible change. Repaint the last frame,
            // then stop until SetState says something actually changed.
            if (!_pulse
                && Mathf.Approximately(_reveal, _targetReveal)
                && Mathf.Abs(_scale - _targetScale) < ScaleEpsilon)
            {
                if (_settled) return;
                _settled = true;
                _scale = _targetScale;
            }
            else
            {
                _settled = false;
            }

            // Within Update the colours depend only on the reveal fade — opacity, brightness and the
            // icon only ever change via SetState, which repaints directly. So the pulsing current
            // room rewrites just its scale each frame instead of dirtying four Images' geometry.
            if (!Mathf.Approximately(_reveal, _appliedReveal))
            {
                _appliedReveal = _reveal;
                ApplyColours();
            }

            ApplyTransform();
        }

        private void ApplyColours()
        {
            float a = _opacity * _reveal;
            _background.color = new Color(_brightness, _brightness, _brightness, a);
            Color border = _db != null ? _db.borderColor : Color.black;
            _border.color = new Color(border.r, border.g, border.b,
                                      border.a * a * (_targetScale > 1f ? 1f : 0.5f));
            _indicator.color = new Color(1f, 1f, 1f, a);
            if (_iconDef != null)
                _icon.color = new Color(_iconDef.tint.r, _iconDef.tint.g, _iconDef.tint.b,
                                        _iconDef.tint.a * _brightness * a);
        }

        private void ApplyTransform() => _rect.localScale = Vector3.one * _scale;
    }
}
