﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Nefarius.DsHidMini.Properties;
using Newtonsoft.Json;
using Serilog;

namespace Nefarius.DsHidMini.Util.Web
{
    public static class Updater
    {
        public static Version AssemblyVersion => Assembly.GetEntryAssembly().GetName().Version;

        public static Uri ReleasesUri => new Uri("https://api.github.com/repos/ViGEm/DsHidMini/releases");

        /// <summary>
        ///     True if tag on latest GitHub release is newer than own assembly version, false otherwise.
        /// </summary>
        public static bool IsUpdateAvailable
        {
            get
            {
                Log.Information("Checking for new version, current version is {Version}",
                    AssemblyVersion);

                // Get cached value
                var lastChecked = (DateTime) Settings.Default["LastCheckedForUpdate"];
                Log.Debug("Last checked for update on {LastCheckedForUpdate}", lastChecked);

                // If we already checked today, return cached value to reduce HTTP calls
                if (lastChecked.AddDays(1) >= DateTime.UtcNow)
                {
                    var storedValue = (bool) Settings.Default["IsUpdateAvailable"];
                    Log.Information("Update check already occurred within the last day, returning {StoredValue}",
                        storedValue);
                    return storedValue;
                }

                try
                {
                    // Query for releases/tags and store information
                    using (var client = new WebClient())
                    {
                        Log.Information("Checking for updates, preparing web request");

                        // Required or result is HTTP-403
                        client.Headers["User-Agent"] =
                            "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) " +
                            "(compatible; MSIE 6.0; Windows NT 5.1; " +
                            ".NET CLR 1.1.4322; .NET CLR 2.0.50727)";

                        // Get body
                        var response = client.DownloadString(ReleasesUri);

                        // Get JSON objects
                        var result = JsonConvert.DeserializeObject<IList<Root>>(response);
                        Log.Debug("Found {ReleaseCount} release(s)", result.Count);

                        // Top release is latest of interest
                        var latest = result.FirstOrDefault();

                        // No release found to compare to, bail out
                        if (latest == null)
                            return false;

                        Log.Debug("Latest tag name: {Tag}", latest.TagName);

                        var tag = new string(latest.TagName.Skip(1).ToArray());
                        Log.Debug("Stripped tag name: {Tag}", tag);

                        // Expected format e.g. "v1.2.3" so strip first character
                        var version = Version.Parse(tag);
                        Log.Debug("Tag to version conversion: {Version}", version);

                        // Store values in user settings
                        Settings.Default["LastCheckedForUpdate"] = DateTime.UtcNow;
                        Log.Debug("Updating last checked for update to {Timestamp}",
                            Settings.Default["LastCheckedForUpdate"]);

                        var isOutdated = version.CompareTo(AssemblyVersion) > 0;
                        if (isOutdated)
                            Log.Information("Update available");

                        Settings.Default["IsUpdateAvailable"] = isOutdated;
                        Log.Debug("Updating update available value to {IsUpdateAvailable}",
                            isOutdated);
                        Settings.Default.Save();

                        return isOutdated;
                    }
                }
                catch(Exception ex)
                {
                    Log.Error("Updated check failed: {Exception}", ex);

                    // May happen on network issues, ignore
                    return false;
                }
            }
        }
    }
}