namespace EasyMarkup
{
    using System;

    internal class EmException : Exception
    {
        internal StringBuffer CurrentBuffer { get; private set; } = null;

        public EmException()
        {
        }

        public EmException(string message) : base(message)
        {
        }

        public EmException(string message, StringBuffer currentBuffer) : base(message)
        {
            this.CurrentBuffer = currentBuffer;
        }

        public EmException(StringBuffer currentBuffer)
        {
            this.CurrentBuffer = currentBuffer;
        }

        public override string ToString()
        {
            if (!(this.CurrentBuffer is null) && !this.CurrentBuffer.IsEmpty)
            {
                return $"Error reported: {this.Message}{Environment.NewLine}" +
                       $"Current text in buffer: {this.CurrentBuffer}";
            }

            return base.ToString();
        }
    }
}
