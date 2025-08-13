using System;
using System.Runtime.Serialization;

using xpTURN.Common;
using xpTURN.MegaData;

namespace Tests.Locale.Type1
{
    // <summary>
    // The LocaleTableSet class manages locale data tables,
    // providing functionality to set and retrieve text data for the current locale ID.
    // Text data is batch loaded per LocaleId.
    // </summary>
    public partial class Locale1TableSet
    {
        // Current Locale ID
        [IgnoreDataMember]
        public int LocaleId { get; private set; } = 0;

        // LocaleData for the current locale
        private LocaleData localeData = null;

        /// <summary>
        /// Sets the LocaleData for the current Locale ID.
        /// </summary>
        /// <param name="localeId">The Locale ID to set</param>
        /// <param name="oldLocaleUnload">Whether to unload the previous locale data</param>
        public bool SetLocale(int localeId, bool oldLocaleUnload = true)
        {
            var tableId = TableAlias[nameof(LocaleDataTable)];
            if (oldLocaleUnload)
            {
                localeData = null;
                GetTable(tableId).GetMap().Clear();
            }

            // Set new locale data
            LocaleId = localeId;
            localeData = GetDataById(tableId, LocaleId) as LocaleData;
            if (localeData == null)
                return false;

            return true;
        }

        /// <summary>
        /// Retrieves the text corresponding by alias.
        /// </summary>
        /// <param name="alias">The alias of the text to retrieve</param>
        /// <returns>The text for the given alias, or an empty string if not found</returns>
        /// <remarks>
        /// Returns an empty string if the locale is not set or if there is no LocaleData for the current locale.
        /// </remarks>
        public String GetString(String alias)
        {
            if (localeData == null)
            {
                // Locale is not set or LocaleData for the current locale is not available
                Logger.Log.Error($"Locale not set or LocaleData not found for LocaleId: {LocaleId}");
                return String.Empty;
            }

            var textId = GetId(alias);
            if (textId == 0)
            {
                // No text ID found for the alias
                Logger.Log.Debug($"Text ID not found for alias: {alias}");
                return String.Empty;
            }

            localeData.Map.TryGetValue(textId, out var text);
            return text?.Text ?? String.Empty;
        }

        /// <summary>
        /// Retrieves the text corresponding by dataId.
        /// </summary>
        /// <param name="dataId">The data ID of the text to retrieve</param>
        /// <returns>The text for the given data ID, or an empty string if not found</returns>
        /// <remarks>
        /// Returns an empty string if the locale is not set or if there is no LocaleData for the current locale.
        /// </remarks>
        public String GetString(int dataId)
        {
            if (localeData == null)
            {
                // Locale is not set or LocaleData for the current locale is not available
                Logger.Log.Error($"Locale not set or LocaleData not found for LocaleId: {LocaleId}");
                return String.Empty;
            }

            localeData.Map.TryGetValue(dataId, out var text);
            return text?.Text ?? String.Empty;
        }
    }
}