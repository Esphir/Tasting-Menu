// Persistent player-facing settings, backed by PlayerPrefs.
using System;
using UnityEngine;

namespace Signal.UI
{
    public static class SettingsStore
    {
        public const float DefaultMouseSensitivity = 1f;

        public const int DefaultCameraSide = 1;

        // 0 means uncapped — the frame rate the game has always run at, so an existing player sees no change.
        public const int DefaultFrameRateCap = 0;

        private const string MouseSensitivityKey = "settings-mouse-sensitivity";
        private const string CameraSideKey = "settings-camera-side";
        private const string FrameRateCapKey = "settings-frame-rate-cap";

        private static float _mouseSensitivity = float.NaN;
        private static int _cameraSide;
        private static int _frameRateCap = NotLoaded;

        private const int NotLoaded = -1;

        public static float MouseSensitivity
        {
            get
            {
                if (float.IsNaN(_mouseSensitivity))
                    _mouseSensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey, DefaultMouseSensitivity);
                return _mouseSensitivity;
            }
            set
            {
                if (Mathf.Approximately(_mouseSensitivity, value)) return;
                _mouseSensitivity = value;
                PlayerPrefs.SetFloat(MouseSensitivityKey, value);
                PlayerPrefs.Save();
            }
        }

        public static int CameraSide
        {
            get
            {
                if (_cameraSide == 0)
                    _cameraSide = PlayerPrefs.GetInt(CameraSideKey, DefaultCameraSide) < 0 ? -1 : 1;
                return _cameraSide;
            }
            set
            {
                int side = value < 0 ? -1 : 1;
                if (_cameraSide == side) return;
                _cameraSide = side;
                PlayerPrefs.SetInt(CameraSideKey, side);
                PlayerPrefs.Save();
            }
        }

        // 0 = unlimited. Applied immediately on set, and on boot, so it survives without the panel being opened.
        public static int FrameRateCap
        {
            get
            {
                if (_frameRateCap == NotLoaded)
                    _frameRateCap = Mathf.Max(0, PlayerPrefs.GetInt(FrameRateCapKey, DefaultFrameRateCap));
                return _frameRateCap;
            }
            set
            {
                int cap = Mathf.Max(0, value);
                if (_frameRateCap == cap) return;
                _frameRateCap = cap;
                PlayerPrefs.SetInt(FrameRateCapKey, cap);
                PlayerPrefs.Save();
                ApplyFrameRateCap();
            }
        }

        // Unity ignores targetFrameRate while vSync is on, so this setting owns frame pacing outright
        // rather than letting the quality level silently override the player's choice.
        public static void ApplyFrameRateCap()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = FrameRateCap <= 0 ? -1 : FrameRateCap;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetCache()
        {
            _mouseSensitivity = float.NaN;
            _cameraSide = 0;
            _frameRateCap = NotLoaded;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ApplyOnBoot() => ApplyFrameRateCap();
    }
}
