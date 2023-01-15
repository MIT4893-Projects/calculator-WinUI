// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System.ComponentModel;
using System.Linq;
using Windows.UI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace calculator_WinUI.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            DataContext = new MainPageDataContext(this);
        }

        public MainPageDataContext GetDataContext()
        {
            return DataContext as MainPageDataContext;
        }

        private void ClearAll()
        {
            GetDataContext().AnswerString = "";
            GetDataContext().GetCalculator().ClearExpressionElement();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (GetDataContext().GetCalculator().Calculated)
                GetDataContext().GetCalculator().ClearExpressionElement();

            if (GetDataContext().AnswerString != "0")
            {
                if (GetDataContext().AnswerString.Any())
                    GetDataContext().AnswerString = GetDataContext().AnswerString.Substring(0, GetDataContext().AnswerString.Length - 1);
                else
                {
                    GetDataContext().AnswerString = "";
                    GetDataContext().GetCalculator().DeleteLastExpressionElement();
                }
            }
            else
                GetDataContext().GetCalculator().DeleteLastExpressionElement();

            if (GetDataContext().GetCalculator().GetLastExpressionElement() != null)
            {
                if (GetDataContext().GetCalculator().GetLastExpressionElement().ElementType == typeof(decimal))
                {
                    GetDataContext().AnswerString = GetDataContext().GetCalculator().GetLastExpressionElement().ToString();
                    GetDataContext().GetCalculator().DeleteLastExpressionElement();
                }
            }
        }

        private void NumberButton_Click(object sender, RoutedEventArgs e)
        {
            // If AnswerString is calculated answer.
            if (GetDataContext().GetCalculator().Calculated)
                ClearAll();

            // Max number limit is 12.
            if (GetDataContext().AnswerString.Length >= 12)
                return;
            Button senderButton = sender as Button;
            int clickedNumber = 0;
            if (senderButton == Number1Button)
                clickedNumber = 1;
            else if (senderButton == Number2Button)
                clickedNumber = 2;
            else if (senderButton == Number3Button)
                clickedNumber = 3;
            else if (senderButton == Number4Button)
                clickedNumber = 4;
            else if (senderButton == Number5Button)
                clickedNumber = 5;
            else if (senderButton == Number6Button)
                clickedNumber = 6;
            else if (senderButton == Number7Button)
                clickedNumber = 7;
            else if (senderButton == Number8Button)
                clickedNumber = 8;
            else if (senderButton == Number9Button)
                clickedNumber = 9;

            if (GetDataContext().AnswerString == "0")
            {
                GetDataContext().AnswerString = clickedNumber.ToString();
                return;
            }
            GetDataContext().AnswerString = GetDataContext().AnswerString + clickedNumber.ToString();
        }

        private void OperatorButton_Click(object sender, RoutedEventArgs e)
        {
            // If answerString is empty, don't add an operator.
            if (!GetDataContext().answerString.Any())
                return;

            // If number is not valid, don't add an operator and number
            if (!GetDataContext().GetCalculator().IsValidNumber(GetDataContext().AnswerString))
                return;

            // If AnswerString is caculate answer then add current answer to expression and add an operator
            if (GetDataContext().GetCalculator().Calculated)
                GetDataContext().GetCalculator().ClearExpressionElement();

            // Add number in AnswerString to expression element collection.
            GetDataContext().GetCalculator()
                            .AddNumberExpressionElement(GetDataContext().AnswerString);

            // If expression element collection is not empty and last element is not Operator.
            if (GetDataContext().GetCalculator()
                                .GetLastExpressionElement() != null)
            {
                if (GetDataContext().GetCalculator()
                                    .GetLastExpressionElement().ElementType == typeof(Operator))
                {
                    return;
                }
            }

            // Find which button was clicked
            Button senderButton = sender as Button;
            char clickedOperator = new();

            if (senderButton == OperatorPlusButton)
                clickedOperator = '+';
            else if (senderButton == OperatorMinusButton)
                clickedOperator = '-';
            else if (senderButton == OperatorMultiplyButton)
                clickedOperator = '*';
            else if (senderButton == OperatorDivisionButton)
                clickedOperator = '/';

            // Add Operator to expression element collection and reset answer string 
            GetDataContext().GetCalculator()
                            .AddExpressionElement(new ExpressionElement(new Operator(clickedOperator)));

            GetDataContext().AnswerString = "";
        }

        private void DotButton_Click(object sender, RoutedEventArgs e)
        {
            // If there's a dot in number, don't add a new dot twice.
            if (GetDataContext().AnswerString.IndexOf('.') != -1)
                return;
            GetDataContext().AnswerString = GetDataContext().AnswerString + ".";
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            GetDataContext().GetCalculator()
                            .AddNumberExpressionElement(GetDataContext().AnswerString);
            GetDataContext().AnswerString = GetDataContext().GetCalculator().Calculate();
        }

        private void InvertButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear expression when AnswerString is calculated string
            if (GetDataContext().GetCalculator().Calculated)
                GetDataContext().GetCalculator().ClearExpressionElement();

            // Don't do anything if AnswerString is zero.
            if (GetDataContext().AnswerString == "0")
                return;

            if (GetDataContext().GetCalculator().IsNegativeNumber(GetDataContext().AnswerString))
            {
                GetDataContext().AnswerString = GetDataContext().AnswerString.TrimStart('-');
                return;
            }
            GetDataContext().AnswerString = "-" + GetDataContext().AnswerString;
        }

        private void ClearEntryButton_Click(object sender, RoutedEventArgs e)
        {
            GetDataContext().AnswerString = "";
        }
    }

    /// <summary>
    /// Data Context for MainPage
    /// </summary>
    public partial class MainPageDataContext : DataContext, INotifyPropertyChanged
    {
        private readonly Calculator calculator;


        #region Binding Properties
        public string answerString { get; private set; } = "";
        public string AnswerString
        {
            get
            {
                if (!answerString.Any())
                    return "0";
                return answerString;
            }
            set
            {
                answerString = value;
                RaisePropertyChange("AnswerString");
            }
        }
        public string ExpressionString { get => GetCalculator().ExpressionElementToString(); }
        #endregion


        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        public MainPageDataContext(Page parentPage) : base(parentPage)
        {
            calculator = new Calculator(this);
        }

        /// <summary>
        /// Returns a reference to calculator object of this data context.
        /// </summary>
        /// <returns>Calculator's reference.</returns>
        public Calculator GetCalculator()
        {
            return calculator;
        }

        public SolidColorBrush TextForegroundOnAccentColor
        {
            get
            {
                Color accentColor = (Color)ParentPage.Resources["SystemAccentColor"];
                return ColorPicker.BlackOrWhiteTextColorBasedOnBackground(accentColor);
            }
        }
    }
}