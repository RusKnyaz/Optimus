using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Knyaz.Optimus.Dom;
using Knyaz.Optimus.Dom.Css;
using Knyaz.Optimus.Dom.Elements;
using Knyaz.Optimus.ResourceProviders;
using Knyaz.Optimus.Tools;

namespace Knyaz.Optimus.TestingTools
{
    /// <summary>
    /// Contains async helper methods for testing with optimus Engine.
    /// </summary>
    public static class EngineAsyncExtension
    {
        /// <summary>
        /// Wait for the loading of document asynchronously.
        /// </summary>
        public static async Task WaitDocumentLoadAsync(this Engine engine)
        {
            if (engine.Document.ReadyState != DocumentReadyStates.Loading)
                return;

            var signal = new TaskCompletionSource<bool>();

            Action<Document> handler = null;
            handler = document =>
            {
                signal.TrySetResult(true);
                engine.Document.DomContentLoaded -= handler;
            };

            engine.Document.DomContentLoaded += handler;

            if (engine.Document.ReadyState == DocumentReadyStates.Loading)
            {
                var timeoutTask = Task.Delay(EngineExtension.DefaultTimeout);
                var completedTask = await Task.WhenAny(signal.Task, timeoutTask).ConfigureAwait(false);
            }
            else
            {
                engine.Document.DomContentLoaded -= handler;
            }
        }

        /// <summary>
        /// Waits until an item with the specified ID appears in the document.
        /// </summary>
        /// <param name="engine">The engine with the document to wait in.</param>
        /// <param name="id">The identifier to be awaited.</param>
        /// <returns>Element with specified Id, <c>null</c> if the element with the specified identifier has not appeared in the document for the default timeout.</returns>
        public static Task<Element> WaitIdAsync(this Engine engine, string id)
        {
            return WaitIdAsync(engine, id, EngineExtension.DefaultTimeout);
        }

        /// <summary>
        /// Waits until an item with the specified ID appears in the document.
        /// </summary>
        /// <param name="engine">Document owner.</param>
        /// <param name="id">Id of element waiting for.</param>
        /// <param name="timeout">The time to wait in milliseconds</param>
        /// <returns>Element with specified Id, <c>null</c> if the element with the specified identifier has not appeared in the document for a given time.</returns>
        public static async Task<Element> WaitIdAsync(this Engine engine, string id, int timeout)
        {
            await engine.WaitDocumentLoadAsync();
            var timespan = 100;
            for (int i = 0; i < timeout / timespan; i++)
            {
                var doc = engine.Document;
                lock (doc)
                {
                    try
                    {
                        var elt = doc.GetElementById(id);
                        if (elt != null)
                            return elt;
                    }
                    catch
                    {
                        //catch 'collection was changed...'
                    }
                }

                await Task.Delay(timespan);
            }
            return engine.Document.GetElementById(id);
        }

        /// <summary>
        /// Waits until the element with specified id disappears.
        /// </summary>
        /// <param name="engine">Document owner.</param>
        /// <param name="id">Identifier of the item to be disappeared.</param>
        /// <returns>Element if found, <c>null</c> otherwise.</returns>
        public static Task<Element> WaitDisappearingOfIdAsync(this Engine engine, string id)
        {
            return WaitDisappearingOfIdAsync(engine, id, EngineExtension.DefaultTimeout);
        }

        /// <summary>
        /// Waits until the element with specified id disappears.
        /// </summary>
        /// <param name="engine">Document owner.</param>
        /// <param name="id">Identifier of the item to be disappeared.</param>
        /// <param name="timeout">The timeout</param>
        /// <returns>Element if found, <c>null</c> otherwise.</returns>
        public static async Task<Element> WaitDisappearingOfIdAsync(this Engine engine, string id, int timeout)
        {
            var timespan = 100;
            for (int i = 0; i < timeout / timespan; i++)
            {
                var doc = engine.Document;
                lock (doc)
                {
                    var elt = doc.GetElementById(id);
                    if (elt == null)
                        return null;
                }

                await Task.Delay(timespan);
            }
            return engine.Document.GetElementById(id);
        }

        /// <summary>
        /// Waits while element with specified id appears in document.
        /// </summary>
        public static Task<Element> WaitIdAsync(this HtmlDocument document, string id)
        {
            return WaitIdAsync(document, id, EngineExtension.DefaultTimeout);
        }

        /// <summary>
        /// Waits while element with specified id appears in document.
        /// </summary>
        public static async Task<Element> WaitIdAsync(this HtmlDocument document, string id, int timeout)
        {
            var timespan = 100;
            for (int i = 0; i < timeout / timespan; i++)
            {
                try
                {
                    var elt = document.GetElementById(id);
                    if (elt != null)
                        return elt;
                }
                catch (InvalidOperationException)
                {
                    //skip 'Collection was modified'
                }

                await Task.Delay(timespan);
            }
            return document.GetElementById(id);
        }

        /// <summary>
        /// Waits at least one element which satisfies to a given query selector.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        public static Task<IEnumerable<Element>> WaitSelectorAsync(this Engine engine, string query)
        {
            return WaitSelectorAsync(engine, query, EngineExtension.DefaultTimeout);
        }

        /// <summary>
        /// Waits until an at least one item which satisfies specified selector appears in the document.
        /// </summary>
        /// <param name="engine">Document owner.</param>
        /// <param name="query">Css selector.</param>
        /// <param name="timeout">Time to wait in milliseconds.</param>
        /// <returns>Collection of found elements.</returns>
        public static async Task<IEnumerable<Element>> WaitSelectorAsync(this Engine engine, string query, int timeout)
        {
            await engine.WaitDocumentLoadAsync();

            return await WaitSelectorAsync(engine.Document, query, timeout);
        }

        /// <summary>
        /// Waits until at least one element that matches the query appears in the document.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="query"></param>
        /// <param name="timeout"></param>
        /// <returns>Matched elements.</returns>
        public static async Task<IEnumerable<Element>> WaitSelectorAsync(this HtmlDocument doc, string query, int timeout = 0)
        {
            if (timeout == 0)
                timeout = EngineExtension.DefaultTimeout;

            var selector = new CssSelector(query);
            var timespan = 100;
            for (int i = 0; i < timeout / timespan; i++)
            {
                try
                {
                    var elt = selector.Select(doc).ToListOrNull();
                    if (elt != null)
                        return elt;
                }
                catch
                {
                }

                await Task.Delay(timespan);
            }

            return selector.Select(doc);
        }

        /// <summary>
        /// Dumps the entire documents html to the file asynchronously.
        /// </summary>
        /// <param name="engine">Document owner.</param>
        /// <param name="fileName">Target file name.</param>
        public static async Task DumpToFileAsync(this Engine engine, string fileName)
        {
            var data = engine.Document.InnerHTML;
            using (var stream = File.CreateText(fileName))
            {
                await stream.WriteAsync(data);
            }
        }

        public static async Task<IResource> DownloadAsync(this Engine engine, string href) =>
            (await engine.ResourceProvider.SendRequestAsync(new Request("GET", new Uri(href))
            {
                Cookies = engine.CookieContainer
            }));
    }
}
