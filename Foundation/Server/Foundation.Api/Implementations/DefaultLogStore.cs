﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Foundation.Core.Contracts;
using Foundation.Core.Models;
using System.Linq;

namespace Foundation.Api.Implementations
{
    public class DefaultLogStore : ILogStore
    {
        private readonly AppEnvironment _activeAppEnvironment;
        private readonly IContentFormatter _contentFormatter;

        protected DefaultLogStore()
        {
        }

        public DefaultLogStore(IContentFormatter contentFormatter, IAppEnvironmentProvider appEnvironmentProvider)
        {
            if (contentFormatter == null)
                throw new ArgumentNullException(nameof(contentFormatter));

            if (appEnvironmentProvider == null)
                throw new ArgumentNullException(nameof(appEnvironmentProvider));

            _contentFormatter = contentFormatter;
            _activeAppEnvironment = appEnvironmentProvider.GetActiveAppEnvironment();
        }

        public virtual Task SaveLogAsync(LogEntry logEntry)
        {
            if (logEntry == null)
                throw new ArgumentNullException(nameof(logEntry));

            EventLog appLog = new EventLog("Application")
            {
                Source = _activeAppEnvironment.AppInfo.Name
            };

            EventLogEntryType eventLogsSeverity;

            if (logEntry.Severity == "Warning")
                eventLogsSeverity = EventLogEntryType.Warning;
            else if (logEntry.Severity == "Information")
                eventLogsSeverity = EventLogEntryType.Information;
            else
                eventLogsSeverity = EventLogEntryType.Error;

            string logContents = _contentFormatter.Serialize(logEntry);

            if (logContents.Length >= 30000)
                logContents = logContents.Substring(0, 29999);

            if (_activeAppEnvironment.Configs.Any(c => string.Equals(c.Key, "EventLogId", StringComparison.CurrentCultureIgnoreCase)))
            {
                appLog.WriteEntry(logContents, eventLogsSeverity, Convert.ToInt32(_activeAppEnvironment.GetConfig<long>("EventLogId")));
            }
            else
            {
                appLog.WriteEntry(logContents, eventLogsSeverity);
            }

            return Task.FromResult<object>(null);
        }

        public virtual void SaveLog(LogEntry logEntry)
        {
            if (logEntry == null)
                throw new ArgumentNullException(nameof(logEntry));

            EventLog appLog = new EventLog("Application")
            {
                Source = _activeAppEnvironment.AppInfo.Name
            };

            EventLogEntryType eventLogsSeverity;

            if (logEntry.Severity == "Warning")
                eventLogsSeverity = EventLogEntryType.Warning;
            else if (logEntry.Severity == "Information")
                eventLogsSeverity = EventLogEntryType.Information;
            else
                eventLogsSeverity = EventLogEntryType.Error;

            string logContents = _contentFormatter.Serialize(logEntry);

            if (logContents.Length >= 30000)
                logContents = logContents.Substring(0, 29999);

            if (_activeAppEnvironment.Configs.Any(c => string.Equals(c.Key, "EventLogId", StringComparison.CurrentCultureIgnoreCase)))
            {
                appLog.WriteEntry(logContents, eventLogsSeverity, Convert.ToInt32(_activeAppEnvironment.GetConfig<long>("EventLogId")));
            }
            else
            {
                appLog.WriteEntry(logContents, eventLogsSeverity);
            }
        }
    }
}