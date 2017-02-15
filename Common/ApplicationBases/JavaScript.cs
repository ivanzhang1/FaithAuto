using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Selenium;
using Gallio.Framework;
using MbUnit.Framework;
using Gallio.Framework.Pattern;

namespace FTTests {
    public class JavaScript {
        private const string basescript = "selenium.browserbot.getCurrentWindow()";

        /// <summary>
        /// Returns a JavaScript string to determine if an element is present
        /// </summary>
        /// <param name="id">Id of the element</param>
        /// <returns></returns>
        public string IsElementPresent(string id) {
            return string.Format("{0}.document.getElementById('{1}')", basescript, id);
        }

        /// <summary>
        /// Determines if a given element is present based on a CSS selector.
        /// </summary>
        /// <param name="cssSelector">The css selector expression</param>
        /// <returns></returns>
        public string IsElementPresentSelector(string cssSelector) {
            return string.Format("{0}.document.querySelector('{1}')", basescript, cssSelector);
        }

        /// <summary>
        /// Returns a JavaScript string to determine if select has elements.
        /// </summary>
        /// <param name="id">Id of the select</param>
        /// <returns></returns>
        public string DoesSelectHaveElements(string id) {
            return string.Format("{0}.document.getElementById('{1}').length", basescript, id);
        }

        /// <summary>
        /// Closes the current window.
        /// </summary>
        /// <returns></returns>
        public string CloseCurrentWindow() {
            return string.Format("{0}.close()", basescript);
        }

        /// <summary>
        /// Gets the number of rows in a given table.
        /// </summary>
        /// <param name="tableId">Table Id</param>
        /// <returns></returns>
        public string GetNumberOfTableRows(string tableId) {
            return string.Format("{0}.document.getElementByID('{1}').rows.length", basescript, tableId);
        }

        /// <summary>
        /// Gets the number of columns for a given row.
        /// </summary>
        /// <param name="tableId">Table Id</param>
        /// <param name="tableRow">Current table row</param>
        /// <returns></returns>
        public string GetNumberOfTableColumns(string tableID, string tableRow) {
            return string.Format("{0}.document.getElementByID('{1}').rows['{2}'].cells.length", basescript, tableID, tableRow);
        }

        /// <summary>
        /// Determines if the specified option exists in the given select element.
        /// </summary>
        /// <param name="selectId">Select Id</param>
        /// <param name="optionText">Option text</param>
        /// <returns></returns>
        public string OptionExistsInSelect(string selectId, string optionText) {
            StringBuilder script = new StringBuilder();

            script.Append(" {");
            script.AppendFormat(" var selectElement = {0}.document.getElementById('{1}'); ", basescript, selectId);
            script.Append(" var currentOption; ");
            script.Append(" var foundText = 'blah'; ");
            script.Append(" for (index = 0; index < selectElement.options.length; index++) ");
            script.Append(" {");
            script.Append(" currentOption = selectElement.options[index]; ");
            script.AppendFormat(" if (currentOption.text.match('{0}'))", optionText);
            script.Append(" {");
            script.Append(" foundText = currentOption.text; ");
            script.Append(" break; ");
            script.Append(" }");
            script.Append(" }");
            script.AppendFormat(" foundText.match('{0}'); ", optionText);
            script.Append(" }");

            return script.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="selectId"></param>
        /// <param name="optionText"></param>
        /// <returns></returns>

        public string TextboxValue(string selectId, string value)
        {
            StringBuilder script = new StringBuilder();
            script.Append(" {");
            script.AppendFormat(" var textElement = {0}.document.getElementById('{1}'); ", basescript, selectId);
            script.AppendFormat(" return textElement.value.match('{0}');", value);
            script.Append(" }");

            return script.ToString();

        }
    }
}
