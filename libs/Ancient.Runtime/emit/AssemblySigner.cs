namespace flame.runtime.emit
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;

    public class AssemblySigner : IDisposable
    {
        #region Constats
        private const int HeadSize = 11;
        private const int ConstBufferCoefficient = 256 * 1024;
        #endregion

        #region Private Values
        private readonly SymmetricAlgorithm cryptAlgorithm;
        private readonly HashAlgorithm keyHashAlgorithm;
        private int bufferCoefficient;
        #endregion

        #region Properties
        /// <summary>
        /// Получает коэфициент, определяющий размер буффера (в пределах 10-100).
        /// </summary>
        public int BufferCoefficient
        {
            get 
            {
                ThrowIfDisposed();
 
                return bufferCoefficient; 
            }
            set 
            {
                ThrowIfDisposed();
 
                bufferCoefficient = (value >= 10 && value <= 100) ? value : 20; 
            }
        }
        #endregion
 
        #region Events

        /// <summary>
        /// Выполняется при завершении обработки каждой части файла. Позволяет следить за процессом обработки.
        /// </summary>
        public event EventHandler<(long len, long pos)> Progress;
 
        private void OnProgress((long len, long pos) e)
        {
            var temp = Interlocked.CompareExchange(ref Progress, null, null);
            temp?.Invoke(this, e);
        }
        #endregion
 
        #region Constructors
        /// <summary>
        /// Создает экемпляр класса FileCrypter.
        /// </summary>
        /// <param name="symmetricAlg">Алгоритм для шифрования.</param>
        /// <param name="hashAlg">Алгоритм для хеширования ключей.</param>
        public AssemblySigner(SymmetricAlgorithm symmetricAlg, HashAlgorithm hashAlg)
        {
            if (symmetricAlg == null || hashAlg == null)
                throw new ArgumentNullException();
 
            cryptAlgorithm = symmetricAlg;
            keyHashAlgorithm = hashAlg;
            cryptAlgorithm.Padding = PaddingMode.PKCS7;
            bufferCoefficient = 10;
        }
        #endregion
 
        #region Public Methods
        public void SetPassword(SecureString Password)
        {
            if (Password.Length == 0)
                return;
 
            ThrowIfDisposed();
 
            var UnmanagedDecipherPassword = IntPtr.Zero;
            byte[] BytePassword = null;
 
            try
            {
                UnmanagedDecipherPassword = Marshal.SecureStringToCoTaskMemUnicode(Password);
                BytePassword = new byte[Password.Length * 2];
                Marshal.Copy(UnmanagedDecipherPassword, BytePassword, 0, BytePassword.Length);
                Console.WriteLine(Convert.ToBase64String(keyHashAlgorithm.ComputeHash(BytePassword)));
                var key = new Rfc2898DeriveBytes(BytePassword, BytePassword, 1);
                cryptAlgorithm.Key = key.GetBytes(cryptAlgorithm.KeySize / 8);
                cryptAlgorithm.IV = key.GetBytes(cryptAlgorithm.BlockSize / 8);
                cryptAlgorithm.Padding = PaddingMode.Zeros;
                
            }
            finally
            {
                if (UnmanagedDecipherPassword != IntPtr.Zero)
                    Marshal.ZeroFreeCoTaskMemUnicode(UnmanagedDecipherPassword);
 
                if (BytePassword != null)
                {
                    for (var i = 0; i < BytePassword.Length; i++)
                        BytePassword[i] = 0;
 
                    BytePassword = null;
                }
            }
        }
 
        public long EncryptStream(Stream InputStream, Stream OutputStream, CancellationToken token)
        {
            ThrowIfDisposed();
            if (InputStream == null) throw new ArgumentNullException(nameof(InputStream));
            if (OutputStream == null) throw new ArgumentNullException(nameof(OutputStream));
            
            using var encrypt = cryptAlgorithm.CreateEncryptor(cryptAlgorithm.Key, cryptAlgorithm.IV);
            
            var MaxBufferSizeValue = bufferCoefficient * ConstBufferCoefficient;
 
            var csEncrypt = new CryptoStream(OutputStream, encrypt, CryptoStreamMode.Write);
            
            OutputStream.Write(Encoding.ASCII.GetBytes("FLX"), 0, 3);
            OutputStream.Write(BitConverter.GetBytes(InputStream.Length), 0, sizeof(long));
                
            InputStream.Position = 0;
            var DataBuffer = new byte[MaxBufferSizeValue];
 
            while (InputStream.Position < InputStream.Length)
            {
                token.ThrowIfCancellationRequested();
 
                var DataSize = (InputStream.Length - InputStream.Position > MaxBufferSizeValue) ? MaxBufferSizeValue : (int)(InputStream.Length - InputStream.Position);
 
                var WriteDataSize = CalculateDataSize(DataSize, MaxBufferSizeValue);
 
                InputStream.Read(DataBuffer, 0, DataSize);
                csEncrypt.Write(DataBuffer, 0, WriteDataSize);
 
                OnProgress((InputStream.Length, InputStream.Position));
            }

            return OutputStream.Length;
        }
 
        public void DecryptStream(Stream InputStream, Stream OutputStream, CancellationToken token)
        {
            ThrowIfDisposed();
            if (InputStream == null) throw new ArgumentNullException(nameof(InputStream));
            if (OutputStream == null) throw new ArgumentNullException(nameof(OutputStream));


            var MaxBufferSizeValue = bufferCoefficient * ConstBufferCoefficient;
 
            var OriginFileLengthArray = new byte[sizeof(long)];
 
            InputStream.Seek(3, SeekOrigin.Begin);
            InputStream.Read(OriginFileLengthArray, 0, sizeof(long));
 
            var DeltaLength = InputStream.Length - HeadSize - cryptAlgorithm.BlockSize / 8 - BitConverter.ToInt64(OriginFileLengthArray, 0);

            using var decryptor = cryptAlgorithm.CreateDecryptor(cryptAlgorithm.Key, cryptAlgorithm.IV);
            var csEncrypt = new CryptoStream(InputStream, decryptor, CryptoStreamMode.Read);
 
            var DataBuffer = new byte[MaxBufferSizeValue];
 
            while (InputStream.Position < InputStream.Length)
            {
                token.ThrowIfCancellationRequested();
 
                var DataSize = (InputStream.Length - InputStream.Position > MaxBufferSizeValue) ? MaxBufferSizeValue : (int)(InputStream.Length - InputStream.Position - DeltaLength);
                var ReadWriteData = CalculateDataSize(DataSize, MaxBufferSizeValue);
                csEncrypt.Read(DataBuffer, 0, ReadWriteData);
                OutputStream.Write(DataBuffer, 0, DataSize);
 
                OnProgress((InputStream.Length, InputStream.Position));
            }
        }
        #endregion
 
        #region private methods
        private int CalculateDataSize(int DataSize, int MaxDataSize)
        {
            if (DataSize == MaxDataSize)
                return DataSize;
 
            var BlockSize = cryptAlgorithm.BlockSize / 8;
            DataSize = (int)Math.Ceiling((double)DataSize / BlockSize) * BlockSize;
 
            return DataSize;
        }
        #endregion
 
        #region IDisposable
        private bool disposed = false;
 
        private void ThrowIfDisposed()
        {
            if (disposed)
                throw new ObjectDisposedException("Object Disposed");
        }
 
        public void Dispose()
        {
            if (disposed == true) return;
 
            cryptAlgorithm.Clear();
 
            disposed = true;
        }
        #endregion
 
 
        #region Static Methods
        /// <summary>
        /// Проверяет зашифрован ли файл.
        /// </summary>
        /// <param name="FileName">Задает полный путь к файлу, который надо проверить.</param>
        /// <returns>Если файл зашифрован, то возвращает true, если нет - false.</returns>
        public static bool IsEncryptedFile(string FileName)
        {
            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                var RealFileHead = new byte[3];
                var EncFileHead = Encoding.ASCII.GetBytes("FLX");
                fileStream.Read(RealFileHead, 0, 3);
 
                for (var i = 0; i < 3; i++)
                {
                    if (RealFileHead[i] != EncFileHead[i])
                        return false;
                }
 
                return true;
            }
            finally
            {
                fileStream?.Dispose();
            }
        }
        #endregion
    }
}