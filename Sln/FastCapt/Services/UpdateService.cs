using FastCapt.Services.Interfaces;
using Squirrel;

namespace FastCapt.Services
{
    public class UpdateService : IUpdateService
    {
        private const string APP_UPDATE_URL = "httpts://";
        private const string APPLICATION_ID = "FastCapt.Squirrel"; // nuget package id

        public void Run()
        {
            CheckForAppUpdates();
        }

        private async void CheckForAppUpdates()
        {
            using (var updateManager = new UpdateManager(APP_UPDATE_URL, APPLICATION_ID, FrameworkVersion.Net45))
            {
                await updateManager.UpdateApp();
            }
        }

        public void Stop()
        {
            
        }
    }
}
