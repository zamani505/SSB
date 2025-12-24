
using System;
using System.IO;
using System.Text;

namespace SSB.Service.SSBApi.LogService.Services
{
    public class ResponseCaptureStream : Stream
    {
        private readonly Stream _innerStream;
        private readonly MemoryStream _copyStream = new MemoryStream();

        public ResponseCaptureStream(Stream inner)
        {
            _innerStream = inner;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _copyStream.Write(buffer, offset, count);   // کپی برای لاگ
            _innerStream.Write(buffer, offset, count);  // ارسال واقعی
        }

        public string GetBody()
        {
            return Encoding.UTF8.GetString(_copyStream.ToArray());
        }

        #region Stream overrides
        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;
        public override long Length => _innerStream.Length;
        public override long Position
        {
            get => _innerStream.Position;
            set => _innerStream.Position = value;
        }
        public override void Flush() => _innerStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => _innerStream.SetLength(value);
        #endregion
    }
}