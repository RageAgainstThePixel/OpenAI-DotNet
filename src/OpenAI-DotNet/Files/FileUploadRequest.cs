using System;
using System.IO;

namespace OpenAI.Files
{
    public sealed class FileUploadRequest : IDisposable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filePath">
        /// Local file path to upload.
        /// </param>
        /// <param name="purpose">
        /// The intended purpose of the uploaded documents.
        /// If the purpose is set to "fine-tune", each line is a JSON record with "prompt" and "completion"
        /// fields representing your training examples.
        /// </param>
        /// <exception cref="FileNotFoundException"></exception>
        public FileUploadRequest(string filePath, string purpose)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException($"Could not find the {nameof(filePath)} file located at {filePath}");
            }

            this.File = System.IO.File.OpenRead(filePath);
            this.FileName = Path.GetFileName(filePath);

            this.Purpose = purpose;
        }

        ~FileUploadRequest()
        {
            this.Dispose(false);
        }

        public Stream File { get; }

        public string FileName { get; }

        public string Purpose { get; }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.File?.Close();
                this.File?.Dispose();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
