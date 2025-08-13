using System;
using System.Runtime.Serialization;
using xpTURN.Common;

namespace Samples
{
    /// <summary>
    /// The LocaleTableSet class manages locale data tables and provides functionality
    /// to set and retrieve text data corresponding to the current locale ID.
    /// When requesting text via the GetText method, it dynamically loads and returns the text.
    /// Loaded text is managed with a WeakReference, allowing it to be automatically released by the GC when no longer in use.
    /// </summary>
    public partial class Sample1TableSet
    {
        /// <summary>
        /// Current locale ID
        /// </summary>
        [IgnoreDataMember]
        public int LocaleId { get; private set; } = 0;

        
        /// <summary>
        /// tableId for TextDataTable
        /// </summary>
        private int _textDataTableId = 0;

        /// <summary>
        /// TextDataTable for the current locale
        /// </summary>
        private TextDataTable _textDataTable = null;

        /// <summary>
        /// Sets the TextDataTable corresponding to the current locale ID.
        /// </summary>
        /// <param name="localeId">The locale ID to set</param>
        /// <param name="oldLocaleUnload">Whether to unload the previous locale data</param>
        public bool SetLocale(int localeId, bool oldLocaleUnload = true)
        {
            var tableId = TableAlias[nameof(LocaleDataTable)];
            if (oldLocaleUnload)
            {
                _textDataTable = null;
            }

            // Set new locale data
            LocaleId = localeId;
            _textDataTable = GetDataById(tableId, LocaleId) as TextDataTable;
            if (_textDataTable == null)
                return false;

            DelTable(nameof(TextDataTable));
            AddTable(nameof(TextDataTable), _textDataTable);

            _textDataTableId = TableAlias[nameof(TextDataTable)];
            return true;
        }

        /// <summary>
        /// Retrieves the text corresponding to the current locale ID.
        /// </summary>
        /// <param name="alias">The alias of the text to retrieve</param>
        /// <returns>The text corresponding to the alias, or an empty string if not found</returns>
        /// <remarks>
        /// Returns an empty string if the locale is not set or if the TextDataTable for the locale is unavailable.
        /// </remarks>
        public String GetString(String alias)
        {
            if (_textDataTable == null)
            {
                // Locale is not set or the corresponding TextDataTable for the locale is not available
                Logger.Log.Error($"Locale not set or TextDataTable not found for LocaleId: {LocaleId}");
                return String.Empty;
            }

            // 
            var dataId = GetId(alias);
            if (dataId == 0)
            {
                Logger.Log.Error($"Alias '{alias}' not found in TextDataTable for LocaleId: {LocaleId}");
                return String.Empty;
            }

            var data = GetDataById(_textDataTableId, dataId) as TextData;
            return data != null ? data.Text : String.Empty;
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
            if (_textDataTable == null)
            {
                // Locale is not set or LocaleData for the current locale is not available
                Logger.Log.Error($"Locale not set or LocaleData not found for LocaleId: {LocaleId}");
                return String.Empty;
            }

            var data = GetDataById(_textDataTableId, dataId) as TextData;
            return data != null ? data.Text : String.Empty;
        }
    }
}