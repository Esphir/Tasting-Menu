// Temporary combat diagnostics.
//
// Info is compiled out unless COMBAT_LOG is among the project's Scripting Define Symbols. That
// matters for far more than console spam: Debug.Log captures a managed stack trace and writes to the
// log file synchronously, and a single swing calls this a dozen or more times — once per target for
// damage, once per target for knockback, plus the attack's own start/finish lines. On a hit that
// added up to a visible frame spike.
//
// [Conditional] is what makes the fix complete: the compiler strips the call *and the interpolated
// string its argument builds* at every call site. A plain `if (Enabled)` inside the method cannot do
// that — by the time it runs, the caller has already allocated and formatted the string.
//
// To get the logs back: Player Settings ▸ Other Settings ▸ Scripting Define Symbols → add COMBAT_LOG.
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Signal.Combat
{
    public static class CombatLog
    {
        public static bool Enabled = true;

        [Conditional("COMBAT_LOG")]
        public static void Info(string message, Object context = null)
        {
            if (Enabled) Debug.Log($"[Combat] {message}", context);
        }

        // Deliberately always compiled. Warn only fires when something is genuinely misconfigured
        // (a collider on the hit mask with no IDamageable, a missing animator layer), and silencing
        // that class of message would hide real bugs to save a cost that should never be paid.
        public static void Warn(string message, Object context = null)
        {
            if (Enabled) Debug.LogWarning($"[Combat] {message}", context);
        }
    }
}
