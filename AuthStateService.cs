namespace Flugger
{
    public class AuthStateService
    {
        public bool IsLoggedIn { get; private set; }

        public event Func<Task>? OnChangeAsync;

        public void SetLoggedIn(bool loggedIn)
        {
            IsLoggedIn = loggedIn;
            _ = NotifyStateChangedAsync(); // fire-and-forget async call
        }

        private async Task NotifyStateChangedAsync()
        {
            if (OnChangeAsync != null)
            {
                await OnChangeAsync.Invoke();
            }
        }
    }


}
