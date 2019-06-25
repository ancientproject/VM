namespace System.Linq
{
    using Diagnostics;

    public class StopwatchOperation : IDisposable
    {
        public string Name { get; set; }
        protected Stopwatch InnerStopwatch { get; set; }

        public StopwatchOperation(string name)
        {
            InnerStopwatch = new Stopwatch();
            Name = name ?? throw new ArgumentNullException(nameof(name));

            InnerStopwatch.Start();
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (InnerStopwatch == null) return;

            InnerStopwatch.Stop();
            Trace.WriteLine(
                $"Duration of '{Name}' is {InnerStopwatch.ElapsedMilliseconds} milliseconds.");
            InnerStopwatch = null;
        }
        #endregion
    }
}