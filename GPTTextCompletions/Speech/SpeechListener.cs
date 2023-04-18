using System.Speech.Recognition;

namespace GPTTextCompletions.Speech
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Validate platform compatibility",
        Justification = "This App is Windows Only.")]
    public class SpeechListener
    {
        private Grammar grammar;

        public SpeechRecognitionEngine Engine { get; }

        public SpeechListener()
        {
            Engine = new SpeechRecognitionEngine();
            Engine.SetInputToDefaultAudioDevice();
            grammar = new DictationGrammar();
            
            Engine.LoadGrammar(grammar);
        }

        public void Listen()
        {
            Engine.RecognizeAsync();
        }

        public void Stop()
        {
            Engine.RecognizeAsyncStop();
        }
    }
}
