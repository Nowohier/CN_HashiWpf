namespace Hashi.Gui.Helpers
{
    public static class GeneralHelper
    {
        /// <summary>
        /// Executes a given action within the STA context
        /// </summary>
        /// <param name="action">The action to execute</param>
        /// <returns></returns>
        public static Task StartStaTask(Action action)
        {
            var tcs = new TaskCompletionSource<object>();
            var thread = new Thread(() =>
            {
                try
                {
                    action();
                    tcs.SetResult(new object());
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            return tcs.Task;
        }
    }
}
