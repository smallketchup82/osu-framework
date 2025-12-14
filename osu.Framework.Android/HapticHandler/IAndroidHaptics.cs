// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Runtime.Versioning;
using Android.OS;

namespace osu.Framework.Android.HapticHandler
{
    [SupportedOSPlatform("android26.0")]
    public interface IAndroidHaptics
    {
        void Vibrate(VibrationEffect effect);

        void Cancel();

        bool SupportsHaptics();
    }
}
