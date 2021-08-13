using System;

namespace CodexWebServer
{
    /// <summary>
    /// Container for the text and metadata for a book.
    /// </summary>
    public class Document
    {
        /// <summary>
        /// The document's text.
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// The date and time the document was created.
        /// </summary>
        public DateTime CreationDate { get; set; }
        
        /// <summary>
        /// The date and time the document was last modified.
        /// </summary>
        public DateTime LastModifiedDate { get; set; }
    }
}