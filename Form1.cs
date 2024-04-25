using Microsoft.CognitiveServices.Speech;
using System;
using System.Windows.Forms;

namespace TextToSpeechApp
{
    public partial class Form1 : Form
    {
        private const string API_KEY = "4c371481854744209c6063ddcbab1ad6";
        private const string API_REGION = "eastus";
        private const string AVAILABLE_LANGUAGES = "English,Deutsch,Ukrainian";
        public Form1()
        {
            InitializeComponent();

            LanguagesComboBox.Items.AddRange(AVAILABLE_LANGUAGES.Split(','));
        }

        private async void SendRequestButton_Click(object sender, EventArgs e)
        {
            string convertingText = InputTextBox.Text;

            try
            {
                if (string.IsNullOrEmpty(convertingText))
                {
                    throw new ArgumentNullException("Text for converting is null or empty!");
                }
                else if (LanguagesComboBox.SelectedItem == null)
                {
                    throw new Exception("Language is not selected!");
                }

                SpeechConfig config = SpeechConfig.FromSubscription(API_KEY, API_REGION);

                switch(LanguagesComboBox.SelectedItem.ToString())
                {
                    case "English":
                        config.SpeechSynthesisVoiceName = "en-GB-RyanNeural";
                        break;
                    case "Ukrainian":
                        config.SpeechSynthesisVoiceName = "uk-UA-PolinaNeural";
                        break;
                    case "Deutsch":
                        config.SpeechSynthesisVoiceName = "de-DE-ConradNeural";
                        break;
                }

                using (SpeechSynthesizer synth = new SpeechSynthesizer(config))
                {
                    using (SpeechSynthesisResult result = await synth.SpeakTextAsync(convertingText))
                    {
                        OutputSpeechSynthesisResult(result, convertingText);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        static private void OutputSpeechSynthesisResult(SpeechSynthesisResult result, string text)
        {
            switch (result.Reason)
            {
                case ResultReason.SynthesizingAudioCompleted:
                    Console.WriteLine($"Speech synthesized for text: [{text}]");
                    break;
                case ResultReason.Canceled:
                    SpeechSynthesisCancellationDetails cancellation = SpeechSynthesisCancellationDetails.FromResult(result);
                    MessageBox.Show($"CANCELED: Reason={cancellation.Reason}", "Canceled", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        MessageBox.Show($"CANCELED:\nErrorCode={cancellation.ErrorCode}\nErrorDetails=[{cancellation.ErrorDetails}]\nDid you set the speech resource key and region values?", 
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
