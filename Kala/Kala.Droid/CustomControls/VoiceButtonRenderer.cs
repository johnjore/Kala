//https://github.com/ihassantariq/VoiceRecognitionSystem
using System;
using Android.App;
using Android.Content;
using Android.Speech;
using Kala;
using Kala.Droid;
using Plugin.Logger;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(VoiceButton), typeof(VoiceButtonRenderer))]
namespace Kala.Droid
{
    public class VoiceButtonRenderer : ButtonRenderer, Android.Views.View.IOnClickListener
    {
        private readonly int VOICE = 10;
        private MainActivity activity;
        private global::Android.Widget.Button nativeButton;
        private VoiceButton sharedButton;

        public VoiceButtonRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);
            activity = this.Context as MainActivity;
            nativeButton = new global::Android.Widget.Button(Context);

            if (e.OldElement == null)
            {
                SetNativeControl(nativeButton);
                nativeButton.Clickable = true;
                nativeButton.Focusable = true;
                nativeButton.SetOnClickListener(this);
                nativeButton.Alpha = 0.00f;
            } else {
                activity.ActivityResult -= HandleActivityResult;
            }

            if (e.NewElement != null)
            {
                activity.ActivityResult += HandleActivityResult;
                sharedButton = e.NewElement as VoiceButton;
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (Control == null)
            {
                return;
            }
        }

        public void OnClick(Android.Views.View view)
        {
            try
            {
                App.config.LastActivity = DateTime.Now; //Update lastActivity to reset Screensaver timer

                string rec = Android.Content.PM.PackageManager.FeatureMicrophone;
                if (rec != "android.hardware.microphone")
                {
                    // no microphone, no recording. Disable the button and output an alert
                    var alert = new AlertDialog.Builder(Context);
                    alert.SetTitle("You don't seem to have a microphone to record with");
                    alert.SetPositiveButton("OK", (sender, e) => {
                        return;
                    });

                    alert.Show();
                }
                else
                {
                    // create the intent and start the activity
                    var voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                    voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);

                    // put a message on the modal dialog
                    voiceIntent.PutExtra(RecognizerIntent.ExtraPrompt, "Speak now!");

                    // if there is more then 1.5s of silence, consider the speech over
                    voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
                    voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
                    voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
                    voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);

                    // you can specify other languages recognised here, for example
                    // voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.German);
                    // if you wish it to recognise the default Locale language and German
                    // if you do use another locale, regional dialects may not be recognised very well
                    voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
                    activity.StartActivityForResult(voiceIntent, VOICE);
                }
            }
            catch (Exception ex)
            {
                CrossLogger.Current.Debug("Voice", "Exception: " + ex.ToString());
            }
        }

        private void HandleActivityResult(object sender, ActivityResultEventArgs e)
        {
            if (e.RequestCode == VOICE)
            {
                if (e.ResultCode == Result.Ok)
                {
                    var matches = e.Data.GetStringArrayListExtra(RecognizerIntent.ExtraResults);
                    if (matches.Count != 0)
                    {
                        string textInput = matches[0];
                        if (textInput.Length > 500)
                            textInput = textInput.Substring(0, 500);
                        sharedButton.OnTextChanged?.Invoke(textInput);
                    }
                    else
                        sharedButton.OnTextChanged?.Invoke("No speech was recognised");
                }
            }
        }
    }
}
