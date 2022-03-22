using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Система_проверки_знаний {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private Test test = null;
        private DispatcherTimer dispatcherTimer;
        private DateTime startTime;
        private DateTime stopTime;
        public MainWindow() {
            InitializeComponent();
            questionChoicePanel.IsEnabled = false;
            answerPanel.IsEnabled = false;
            btnStartTest.IsEnabled = false;
            btnStopTest.IsEnabled = false;
            dispatcherTimer = new DispatcherTimer();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog {
                Multiselect = false,
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (openFileDialog.ShowDialog() == true) {
                try {
                    test = TestFactory.CreateTest(File.ReadAllLines(openFileDialog.FileNames[0]));
                    btnStartTest.IsEnabled = true;
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            } else {

            }
        }

        private void questionRadioButton_Click(object sender, RoutedEventArgs e) {
            int index = int.Parse(((RadioButton)sender).Tag.ToString());
            Question question = test.Questions[index];
            questionTextBox.Text = question.QuestionText;
            questionImage.Source = question.Picture;
            answerPanel.Children.Clear();
            switch (question.Type) {
            case QuestionType.CHOOSE_ONE: {
                foreach (string possibleAnswer in question.PossibleAnswers) {
                    RadioButton radioButton = new RadioButton();
                    radioButton.Content = possibleAnswer;
                    radioButton.Tag = index;
                    radioButton.GroupName = "answerRadioButton";
                    radioButton.Click += answerRadioButton_Click;
                    answerPanel.Children.Add(radioButton);
                }
                break;
            }
            case QuestionType.CHOOSE_MULTIPLE: {
                foreach (string possibleAnswer in question.PossibleAnswers) {
                    CheckBox checkBox = new CheckBox();
                    checkBox.Content = possibleAnswer;
                    checkBox.Tag = index;
                    checkBox.Click += answerCheckBox_Click;
                    answerPanel.Children.Add(checkBox);
                }
                break;
            }
            case QuestionType.GIVE_ONE: {
                TextBox textBox = new TextBox();
                textBox.Tag = index;
                textBox.TextChanged += answerTextBox_Click;
                answerPanel.Children.Add(textBox);
                break;
            }
            }
        }

        private void answerRadioButton_Click(object sender, RoutedEventArgs e) {
            int index = int.Parse(((RadioButton)sender).Tag.ToString());
            test.Questions[index].GivenAnswers.Clear();
            test.Questions[index].GivenAnswers.Add(((RadioButton)sender).Content.ToString());
        }

        private void answerTextBox_Click(object sender, RoutedEventArgs e) {
            int index = int.Parse(((TextBox)sender).Tag.ToString());
            test.Questions[index].GivenAnswers.Clear();
            test.Questions[index].GivenAnswers.Add(((TextBox)sender).Text);
        }

        private void answerCheckBox_Click(object sender, RoutedEventArgs e) {
            CheckBox checkBox = (CheckBox)sender;
            int index = int.Parse(checkBox.Tag.ToString());
            if ((bool)checkBox.IsChecked) {
                test.Questions[index].GivenAnswers.Add(checkBox.Content.ToString());
            } else {
                test.Questions[index].GivenAnswers.Remove(checkBox.Content.ToString());
            }
        }

        private void btnStartTest_Click(object sender, RoutedEventArgs e) {
            if (test != null) {
                questionChoicePanel.IsEnabled = true;
                questionChoicePanel.Children.Clear();
                questionTextBox.Clear();
                answerPanel.Children.Clear();
                answerPanel.IsEnabled = true;
                for (int i = 0; i < test.Questions.Count; i++) {
                    RadioButton radioButton = new RadioButton();
                    radioButton.Content = "Вопрос " + (i + 1);
                    radioButton.GroupName = "questionRadioButton";
                    radioButton.Click += questionRadioButton_Click;
                    radioButton.Tag = i.ToString();
                    questionChoicePanel.Children.Add(radioButton);
                }
                btnStartTest.IsEnabled = false;
                btnStopTest.IsEnabled = true;

                scoreTextBlock.Text = "";

                startTime = DateTime.Now;
                stopTime = startTime.AddSeconds(test.Seconds).AddMinutes(test.Minutes);
                dispatcherTimer.Tick += new EventHandler(timer_Tick);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
                dispatcherTimer.Start();
            }
        }

        private void timer_Tick(object sender, EventArgs e) {
            if (stopTime <= DateTime.Now) {
                btnStopTest_Click(null, null);
                dispatcherTimer.Stop();
            }
            timerTextBlock.Text = (stopTime - DateTime.Now).ToString(@"mm\:ss");
            CommandManager.InvalidateRequerySuggested();
        }

        private void btnStopTest_Click(object sender, RoutedEventArgs e) {
            questionChoicePanel.IsEnabled = false;
            answerPanel.IsEnabled = false;
            btnStopTest.IsEnabled = false;
            double score = 0.0;
            for (int i = 0; i < test.Questions.Count; i++) {
                test.Questions[i].computeScore();
                score += test.Questions[i].Score;
            }
            scoreTextBlock.Text = ((int)(score / test.Questions.Count * 100.0)) + "%";
        }
    }
}
