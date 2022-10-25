﻿using hassio_onedrive_backup.Graph;
using hassio_onedrive_backup.Hass;
using hassio_onedrive_backup.Storage;

namespace hassio_onedrive_backup
{
    internal class Program
    {
        private const string clientId = "b8a647cf-eccf-4c7f-a0a6-2cbec5d0b94d";
        private const string addonDirectory = "/data";
        private static readonly List<string> scopes = new() { "Files.ReadWrite.AppFolder" };

        static async Task Main(string[] args)
        {
#if DEBUG
            IHassioClient hassIoClient = new HassioClientMock();
#else
            Directory.SetCurrentDirectory(addonDirectory);
            string supervisorToken = Environment.GetEnvironmentVariable("SUPERVISOR_TOKEN")!;
            IHassioClient hassIoClient = new HassioClient(supervisorToken);
#endif
            IGraphHelper graphHelper = new GraphHelper(scopes, clientId, (info, cancel) =>
            {
                ConsoleLogger.LogInfo(info.Message);
                return Task.FromResult(0);
            });

            var addonOptions = AddonOptionsReader.ReadOptions();
            var backupManager = new BackupManager(addonOptions, graphHelper, hassIoClient);
            int syncIntervalHours = addonOptions.SyncIntervalHours > addonOptions.BackupIntervalHours ? 1 : addonOptions.SyncIntervalHours;
            TimeSpan intervalDelay = TimeSpan.FromHours(syncIntervalHours);
            ConsoleLogger.LogInfo($"Backup interval configured to every {addonOptions.BackupIntervalHours} hours");
            while (true)
            {
                try
                {
                    // Refresh Graph Token
                    await graphHelper.GetAndCacheUserTokenAsync();

                    ConsoleLogger.LogInfo("Checking backups");
                    await backupManager.PerformBackupsAsync();
                    ConsoleLogger.LogInfo("Interval Completed.");
                }
                catch (Exception ex)
                {
                    ConsoleLogger.LogError($"Unexpected error. {ex}");
                }

                await Task.Delay(intervalDelay);
            }
        }
    }
}
