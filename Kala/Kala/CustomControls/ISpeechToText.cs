//https://github.com/ihassantariq/VoiceRecognitionSystem
using System;
using Xamarin.Forms;

namespace Kala
{
    public interface ISpeechToText
    {
        void Start();
        void Stop();
        event EventHandler<VoiceRecognitionEventArgs> TextChanged;
    }

    public class VoiceButton : Button
    {
        public Action<string> OnTextChanged { get; set; }
    }

    public class VoiceRecognitionEventArgs : EventArgs
    {
        public VoiceRecognitionEventArgs(string text, bool isFinal)
        {
            Text = text;
            IsFinal = isFinal;
        }

        public string Text { get; set; }
        public bool IsFinal { get; set; }
    }
}
