﻿using Newtonsoft.Json;
using JsonIgnoreAttribute = Newtonsoft.Json.JsonIgnoreAttribute;

namespace hassio_onedrive_backup.Contracts
{
    public class AddonOptions
    {
        [JsonProperty("local_backup_num_to_keep")]
        public int MaxLocalBackups { get; set; }

        [JsonProperty("onedrive_backup_num_to_keep")]
        public int MaxOnedriveBackups { get; set; }

        [JsonProperty("backup_interval_days")]
        public float BackupIntervalDays { get; set; }

        [JsonProperty("backup_passwd")]
        public string? BackupPassword { get; set; }

        [JsonProperty("backup_name")]
        public string? BackupName { get; set; }

        [JsonProperty("notify_on_error")]
        public bool NotifyOnError { get; set; }

        [JsonProperty("sync_interval_hours")]
        public int SyncIntervalHours { get; set; }

        [JsonIgnore]
        public float BackupIntervalHours => BackupIntervalDays * 24;
    }
}
