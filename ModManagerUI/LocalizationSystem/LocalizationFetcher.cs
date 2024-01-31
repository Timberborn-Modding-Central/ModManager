using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Timberborn.Localization;

namespace ModManagerUI.LocalizationSystem
{
    internal class LocalizationFetcher
    {  /// <summary>
       ///     Modified timberborn method Timberborn.Localization.LocalizationRepository.GetLocalization
       /// </summary>
       /// <param name="localizationKey"></param>
       /// <returns></returns>
        public static Dictionary<string, string> GetLocalization(string localizationKey)
        {
            Dictionary<string, string> localizedRecords = GetLocalizationRecordsFromFiles(localizationKey, GetLocalizationFilePathsFromDependencies(localizationKey))
                .ToDictionary(record => record.Id, record => record.Text);

            foreach (var defaultRecord in GetDefaultLocalization())
            {
                var id = defaultRecord.Id;
                if (!localizedRecords.TryGetValue(id, out var text) || string.IsNullOrEmpty(text))
                {
                    localizedRecords[id] = defaultRecord.Text;
                }
            }

            return localizedRecords;
        }

        /// <summary>
        ///     Parses text files into LocalizationRecords
        /// </summary>
        /// <param name="localization"></param>
        /// <param name="filePaths"></param>
        /// <returns></returns>
        private static IEnumerable<LocalizationRecord> GetLocalizationRecordsFromFiles(string localization, IEnumerable<string> filePaths)
        {
            List<LocalizationRecord> records = new();
            foreach (var path in filePaths)
            {
                records.AddRange(TryToReadRecords(localization, path));
            }

            return records;
        }

        /// <summary>
        ///     Timberborn method Timberborn.Localization.LocalizationRepository.TryToReadRecords
        /// </summary>
        /// <param name="localization"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="InvalidDataException"></exception>
        private static IEnumerable<LocalizationRecord> TryToReadRecords(string localization, string filePath)
        {
            try
            {
                return new CsvContext().Read<LocalizationRecord>(filePath);
            }
            catch (Exception ex)
            {
                var message = "Unable to parse file for " + localization + ".";
                if (ex is AggregatedException aggregatedException)
                {
                    message = message + " First error: " + aggregatedException.m_InnerExceptionsList[0].Message;
                }

                if (localization == LocalizationCodes.Default)
                {
                    throw new InvalidDataException(message, ex);
                }

                ModManagerUIPlugin.Log.LogError(message);
                return new List<LocalizationRecord>();
            }
        }

        /// <summary>
        ///     Returns the default localization
        /// </summary>
        private static IEnumerable<LocalizationRecord> GetDefaultLocalization()
        {
            return GetLocalizationRecordsFromFiles(LocalizationCodes.Default, GetLocalizationFilePathsFromDependencies(LocalizationCodes.Default));
        }

        /// <summary>
        ///     Searches for depencies
        /// </summary>
        /// <param name="localizationKey"></param>
        /// <returns></returns>
        private static List<string> GetLocalizationFilePathsFromDependencies(string localizationKey)
        {
            List<string> localizationFilePaths = new();
            var pluginLocalizationPath = UIPaths.ModManagerUI.Lang;

            (var hasLocalization, var localizationName) = LocalizationNameOrDefault(pluginLocalizationPath, localizationKey);

            if (!hasLocalization)
            {
                return localizationFilePaths;
            }

            localizationFilePaths.Add(Path.Combine(pluginLocalizationPath, localizationName));

            return localizationFilePaths;
        }

        /// <summary>
        ///     Check if localization file exists, return default if not
        ///     Returns false if default and localization file doesn't exists
        /// </summary>
        /// <param name="pluginLocalizationPath"></param>
        /// <param name="localizationName"></param>
        private static (bool, string) LocalizationNameOrDefault(string pluginLocalizationPath, string localizationName)
        {
            if (string.IsNullOrEmpty(localizationName))
            {
                return (false, "");
            }

            if (!Directory.Exists(pluginLocalizationPath))
            {
                return (false, "");
            }

            if (File.Exists(Path.Combine(pluginLocalizationPath, localizationName + ".txt")))
            {
                return (true, localizationName + ".txt");
            }

            if (File.Exists(Path.Combine(pluginLocalizationPath, LocalizationCodes.Default + ".txt")))
            {
                return (true, LocalizationCodes.Default + ".txt");
            }

            return (false, "");
        }
    }
}
