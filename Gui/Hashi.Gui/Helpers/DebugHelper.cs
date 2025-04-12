using System.Diagnostics;
using System.Reflection;

namespace Hashi.Gui.Helpers
{
    public static class DebugHelper
    {
        /// <summary>
        /// Determines if the current build is a debug build.
        /// </summary>
        public static bool IsDebugBuild => Assembly.GetExecutingAssembly().GetCustomAttributes(false).OfType<DebuggableAttribute>().Any((Func<DebuggableAttribute, bool>)(da => da.IsJITTrackingEnabled));
    }
}
