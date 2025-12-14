// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Runtime.Versioning;
using Android.App;
using Android.Content;
using Android.OS;

namespace osu.Framework.Android.HapticHandler
{
    /// <summary>
    /// The handler of <see cref="Vibrator"/>, the legacy way to control vibration on Android.
    /// Need at least API 26 (Android 8).
    /// Android doc: https://developer.android.com/reference/android/os/Vibrator.
    /// </summary>
    /// <remarks>
    /// <see cref="Vibrator"/> exist since API 1. BUT we need API 26 because we want to use <see cref="VibrationEffect"/>.
    /// Without <see cref="VibrationEffect"/>, vibration would just be a simple on/off at full power, which would be bad.
    /// </remarks>
    [SupportedOSPlatform("android26.0")]
    [ObsoletedOSPlatform("android31.0")]
    public class VibratorHandler : IAndroidHaptics
    {
        private readonly Vibrator engine;

        public VibratorHandler()
        {
            engine = Application.Context.GetSystemService(Context.VibratorService) as Vibrator
                     ?? throw new InvalidOperationException("Vibration not available.");
        }

        public void Vibrate(VibrationEffect effect)
        {
            engine.Vibrate(effect);
        }

        public void Cancel()
        {
            engine.Cancel();
        }

        public bool SupportsHaptics()
        {
            return engine.HasVibrator;
        }
    }
}
