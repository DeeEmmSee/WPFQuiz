using System;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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
        int QuestionIndex= -1;

        public MainWindow()
        {
            InitializeComponent();

            Questions = [];
            bdrQuestionPanel.Visibility = Visibility.Collapsed;
        }

        private async void btnStart_Click(object sender, RoutedEventArgs e)
        {
            btnStart.Visibility = Visibility.Collapsed;

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

            // Show correct answer
            bool correct = "" == Questions[QuestionIndex].CorrectAnswer;

            //Brushes.ForestGreen
            //Brushes.IndianRed
        }

        private void SetNextQuestion()
        {
            btnNextQuestion.Visibility = Visibility.Collapsed; 

            QuestionIndex++;

            if (QuestionIndex >= Questions.Length)
            {
                // End of game
            }
            else
            {
                // Next question
                // NOTE: Text from the API can come out HTML encoded
                lblQuestionNumber.Text = $"Question {QuestionIndex + 1}";
                lblQuestionCategory.Text = HttpUtility.HtmlDecode(Questions[QuestionIndex].Category);
                lblQuestion.Text = HttpUtility.HtmlDecode(Questions[QuestionIndex].Question); 
                                
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