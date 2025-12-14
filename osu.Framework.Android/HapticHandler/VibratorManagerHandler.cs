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
    /// The handler of <see cref="VibratorManager"/>, the modern way to control vibration on Android.
    /// Need at least API 31 (Android 12).
    /// Android doc: https://developer.android.com/reference/android/os/VibratorManager.
    /// </summary>
    [SupportedOSPlatform("android31.0")]
    public class VibratorManagerHandler : IAndroidHaptics
    {
        private readonly VibratorManager engine;

        public VibratorManagerHandler()
        {
            engine = Application.Context.GetSystemService(Context.VibratorManagerService) as VibratorManager
                     ?? throw new InvalidOperationException("VibratorManager not available");
        }

        public void Vibrate(VibrationEffect effect)
        {
            engine.Vibrate(CombinedVibration.CreateParallel(effect));
        }

        public void Cancel()
        {
            engine.Cancel();
        }

        public bool SupportsHaptics()
        {
            return engine.GetVibratorIds().Length > 0;
        }
    }
}
