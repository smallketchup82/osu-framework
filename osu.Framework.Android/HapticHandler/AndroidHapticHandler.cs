// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Diagnostics.CodeAnalysis;
using Android.OS;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Framework.Logging;
using osu.Framework.Utils;

namespace osu.Framework.Android.HapticHandler
{
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public class AndroidHapticHandler : HapticManager
    {
        public AndroidHapticHandler(FrameworkConfigManager config)
            : base(config)
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (!SupportsHaptics)
            {
                Logger.Log("Haptics not supported on this device.");
                return;
            }

            engine = getAndroidHaptics();
        }

        public override bool SupportsHaptics => engine?.SupportsHaptics() ?? false;

        private IAndroidHaptics? engine;

        private static IAndroidHaptics? getAndroidHaptics()
        {
            try
            {
                // Try the modern handler
                if (OperatingSystem.IsAndroidVersionAtLeast(31))
                    return new VibratorManagerHandler();

                // Try the legacy handler
                if (OperatingSystem.IsAndroidVersionAtLeast(26))
                    return new VibratorHandler();
            }
            catch (InvalidOperationException)
            {
            }

            Logger.Log("Android Vibration handler not available");

            return null;
        }

        public override void PlayTransient(float intensity, float sharpness)
        {
            if (engine == null) return;

            base.PlayTransient(intensity, sharpness);

            int amplitude = GetAmplitude(intensity);

            if (amplitude == 0)
            {
                release();
                return;
            }

            long[] timings = { 50 };
            int[] amplitudes = { amplitude };

            release();
            engine.Vibrate(VibrationEffect.CreateWaveform(timings, amplitudes, -1)!);
        }

        public override void UpdateIntensity(float intensity, bool force = false)
        {
            if (engine == null) return;

            base.UpdateIntensity(intensity, force);

            int amplitude = GetAmplitude(intensity);

            if (amplitude == 0)
            {
                release();
                return;
            }

            long[] timings = { 1000 };
            int[] amplitudes = { amplitude };

            release();
            engine.Vibrate(VibrationEffect.CreateWaveform(timings, amplitudes, 0)!);
        }

        /// <remarks>
        /// Do nothing because Sharpness does not exist in on Android (can be recreated with custom amplitudes?)
        /// </remarks>
        public override void UpdateSharpness(float sharpness, bool force = false)
        {
        }

        private void release() => engine?.Cancel();

        public override void Crash(float intensity = 1, float sharpness = 1, float durationSeconds = 1)
        {
            if (engine == null) return;

            base.Crash(intensity, sharpness, durationSeconds);

            const int precision = 50;

            long durationMilliSeconds = (long)(durationSeconds * 1000f);

            long[] timings = new long[precision];
            int[] amplitudes = new int[precision];

            long step = durationMilliSeconds / precision;

            for (int i = 0; i < precision; i++)
            {
                timings[i] = step;

                float t = Interpolation.ValueAt(i, 1, 0, 0, precision - 1, Easing.OutExpo);
                amplitudes[i] = GetAmplitude(intensity * t);
            }

            release();
            engine.Vibrate(VibrationEffect.CreateWaveform(timings, amplitudes, -1)!);
        }

        public int GetAmplitude(float intensity)
        {
            return Math.Clamp((int)(intensity * 255f), 0, 255);
        }
    }
}
