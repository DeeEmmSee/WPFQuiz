using System;
using System.IO;
using System.Media;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFApp.Entities;
using WPFApp.Services;
using WPFApp.Utilities;

namespace WPFApp
{
    // TC: There is a bug when updating text in the attribute vs using inner text. When removing the last character of inner text it removes all text from preview even if text attribute has a value. Need to refresh to show text in preview again

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        OTDQuestion[] Questions;
        int QuestionIndex = -1;
        int Correct = 0;
        int msTimePerLetter = 50;

        public MainWindow()
        {
            InitializeComponent();

            Questions = [];
            bdrQuestionPanel.Visibility = Visibility.Collapsed;

        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            btnStart.Visibility = Visibility.Collapsed;
            pnlEndGame.Visibility = Visibility.Collapsed;
            Correct = 0;

            try
            {
                pnlLoading.Visibility = Visibility.Visible;
                bdrQuestionPanel.Visibility = Visibility.Collapsed;

                OTDService otdService = new OTDService();
                Questions = await otdService.GetQuestions();

                pnlLoading.Visibility = Visibility.Collapsed;
                bdrQuestionPanel.Visibility = Visibility.Visible;

                QuestionIndex = -1;
                SetNextQuestion();
            }
            catch (Exception ex)
            {
                lblError.Text = "[btnStart] EXCEPTION: " + ex.Message;
                lblError.Visibility = Visibility.Visible;
            }
        }

        private void btnSubmitAnswer_Click(object sender, RoutedEventArgs e)
        {
            btnOption1.IsEnabled = false;
            btnOption2.IsEnabled = false;
            btnOption3.IsEnabled = false;
            btnOption4.IsEnabled = false;

            // Show correct answer
            bool correct = (e.Source as Button).Content == Questions[QuestionIndex].CorrectAnswer;

            (e.Source as Button).Background = correct ? Brushes.ForestGreen : Brushes.IndianRed;

            if (correct)
            {
                Correct++;
                using (MemoryStream stream = new MemoryStream(Properties.Resources.correct))
                {
                    correctSound = new SoundPlayer(stream);
                    correctSound.Play();
                }
            }
            else
            {
                using (MemoryStream stream = new MemoryStream(Properties.Resources.incorrect))
                {
                    incorrectSound = new SoundPlayer(stream);
                    incorrectSound.Play();
                }
            }

            if (btnOption1.Background != Brushes.IndianRed)
                btnOption1.Background = (btnOption1.Content.ToString() == Questions[QuestionIndex].CorrectAnswer ? Brushes.ForestGreen : Brushes.FloralWhite);
            
            if (btnOption2.Background != Brushes.IndianRed)
                btnOption2.Background = (btnOption2.Content.ToString() == Questions[QuestionIndex].CorrectAnswer ? Brushes.ForestGreen : Brushes.FloralWhite);
            
            if (btnOption3.Background != Brushes.IndianRed)
                btnOption3.Background = (btnOption3.Content.ToString() == Questions[QuestionIndex].CorrectAnswer ? Brushes.ForestGreen : Brushes.FloralWhite);
            
            if (btnOption4.Background != Brushes.IndianRed)
                btnOption4.Background = (btnOption4.Content.ToString() == Questions[QuestionIndex].CorrectAnswer ? Brushes.ForestGreen : Brushes.FloralWhite);

            btnNextQuestion.Visibility = Visibility.Visible;
        }

        private void SetNextQuestion()
        {
            btnNextQuestion.Visibility = Visibility.Collapsed; 

            QuestionIndex++;

            if (QuestionIndex >= Questions.Length)
            {
                // End of game
                bdrQuestionPanel.Visibility = Visibility.Collapsed;
                pnlEndGame.Visibility = Visibility.Visible;
                txtScore.Text = Correct.ToString();
            }
            else
            {
                btnOption1.IsEnabled = true;
                btnOption2.IsEnabled = true;
                btnOption3.IsEnabled = true;
                btnOption4.IsEnabled = true;

                // Next question
                // NOTE: Text from the API can come out HTML encoded
                lblQuestionNumber.Text = $"Question {QuestionIndex + 1}";
                lblQuestionCategory.Text = HttpUtility.HtmlDecode(Questions[QuestionIndex].Category);
                //lblQuestion.Text = HttpUtility.HtmlDecode(Questions[QuestionIndex].Question); 

                string? qText = HttpUtility.HtmlDecode(Questions[QuestionIndex].Question);

                // Animation
                StringAnimationUsingKeyFrames saukf = new StringAnimationUsingKeyFrames();
                TimeSpan ts = new TimeSpan(0, 0, 0, 0, msTimePerLetter);
                saukf.Duration = new TimeSpan(0, 0, 0, 0, qText.Length * msTimePerLetter);
                saukf.FillBehavior = FillBehavior.HoldEnd;
                
                string tmp = "";
                foreach (char c in qText)
                {
                    tmp += c;
                    ts = ts.Add(TimeSpan.FromMilliseconds(msTimePerLetter));
                    saukf.KeyFrames.Add(new DiscreteStringKeyFrame(tmp, KeyTime.FromTimeSpan(ts)));
                }

                lblQuestion.BeginAnimation(TextBlock.TextProperty, saukf);

                List<string> answers = Questions[QuestionIndex].IncorrectAnswers.ToList();
                answers.Add(Questions[QuestionIndex].CorrectAnswer);

                GeneralUtilities.Shuffle(ref answers);

                // TODO: make more robust
                if (answers.Count == 4)
                {
                    btnOption1.Content = HttpUtility.HtmlDecode(answers[0]);
                    btnOption2.Content = HttpUtility.HtmlDecode(answers[1]);
                    btnOption3.Content = HttpUtility.HtmlDecode(answers[2]);
                    btnOption4.Content = HttpUtility.HtmlDecode(answers[3]);

                    btnOption3.Visibility = Visibility.Visible;
                    btnOption4.Visibility = Visibility.Visible;
                }
                else if (answers.Count == 2)
                {
                    btnOption1.Content = HttpUtility.HtmlDecode(answers[0]);
                    btnOption2.Content = HttpUtility.HtmlDecode(answers[1]);

                    btnOption3.Visibility = Visibility.Collapsed;
                    btnOption4.Visibility = Visibility.Collapsed;
                }

                // Reset background
                btnOption1.Background = Brushes.FloralWhite;
                btnOption2.Background = Brushes.FloralWhite;
                btnOption3.Background = Brushes.FloralWhite;
                btnOption4.Background = Brushes.FloralWhite;

            }
        }

        private void btnNextQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (QuestionIndex + 1 >= Questions.Length)
                btnNextQuestion.Content = "RESULTS";
            else
                btnNextQuestion.Content = "NEXT QUESTION";

            SetNextQuestion();
        }
    }
}