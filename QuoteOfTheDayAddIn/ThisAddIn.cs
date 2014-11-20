using System;
using System.Net.Http;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;

namespace QuoteOfTheDayAddIn
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Cache-Control", "max-age=0");

            _quoteFactory = new HttpQuoteFactory(client);

            _inspectors = Application.Inspectors;
            _inspectors.NewInspector += InspectorsOnNewInspector;
        }

        private void InspectorsOnNewInspector(Outlook.Inspector inspector)
        {
            var mailItem = inspector.CurrentItem as Outlook.MailItem;

            if (mailItem == null)
                return;

            if (mailItem.ReceivedTime != Constants.DefaultDateTime)
                return;

            Outlook.InspectorEvents_10_ActivateEventHandler onActivateEvent = null;

            onActivateEvent = () =>
            {
                _quoteFactory
                    .Get()
                    .ContinueWith(AppendQuoteToEmailBody(mailItem));

                ((Outlook.InspectorEvents_10_Event)inspector).Activate -= onActivateEvent;
            };

            ((Outlook.InspectorEvents_10_Event)inspector).Activate += onActivateEvent;
        }

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
        }

        private static Action<Task<QuoteOfTheDay>> AppendQuoteToEmailBody(Outlook.MailItem mailItem)
        {
            return t =>
            {
                if (!t.IsCompleted)
                    return;

                var quote = t.Result;

                var template = string.Empty;

                switch (mailItem.BodyFormat)
                {
                    case Outlook.OlBodyFormat.olFormatUnspecified:
                        break;
                    case Outlook.OlBodyFormat.olFormatPlain:
                        template = "\"{0}\" -{1}";
                        break;
                    case Outlook.OlBodyFormat.olFormatHTML:
                        template = "<div style='padding-left:20px;'><i style='color:#787878'>\"{0}\"<br/>-{1}</i></div>";
                        break;
                    case Outlook.OlBodyFormat.olFormatRichText:
                        break;
                }

                mailItem.HTMLBody += string.Format(template, quote.Quote, quote.Author);
            };
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion

        private Outlook.Inspectors _inspectors;
        private IQuoteFactory _quoteFactory;
    }
}
