using calculator_WinUI.Pages;
using NCalc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.Appointments.AppointmentsProvider;

namespace calculator_WinUI
{
    /// <summary>
    /// Represent a operator in expression
    /// </summary>
    public class Operator
    {
        private readonly char Notation;
        private static readonly List<char> AcceptNotations = new List<char>() { '+', '-', '*', '/' };

        public Operator(char notation)
        {
            if (AcceptNotations.Contains(notation))
                Notation = notation;
        }

        /// <summary>
        /// Calculate expression between two numbers.
        /// </summary>
        /// <param name="firstNumber">Number on the left of operator</param>
        /// <param name="secondNumber">Number on the left of operator</param>
        /// <returns>Calculated answer.</returns>
        public dynamic Calculate(dynamic firstNumber, dynamic secondNumber)
        {
            switch (Notation)
            {
                case '+':
                    return firstNumber + secondNumber;
                case '-':
                    return firstNumber - secondNumber;
                case '*':
                    return firstNumber * secondNumber;
            }
            return firstNumber / secondNumber;
        }

        /// <summary>
        /// Convert opeartor to string.
        /// </summary>
        /// <returns>Converted string.</returns>
        public override string ToString()
        {
            const string Format = " {0} ";
            return string.Format(Format, Notation.ToString());
        }
    }

    /// <summary>
    /// Represents a number or operator in expression
    /// </summary>
    public class ExpressionElement
    {
        public readonly Type ElementType;
        public readonly dynamic ElementValue;

        public ExpressionElement(decimal elementValue)
        {
            ElementType = typeof(decimal);
            ElementValue = elementValue;
        }

        public ExpressionElement(Operator elementValue)
        {
            ElementType = typeof(Operator);
            ElementValue = elementValue;
        }

        public override string ToString()
        {
            return ElementValue?.ToString();
        }
    }

    /// <summary>
    /// Collection to store expression elements
    /// </summary>
    public class ExpressionElementCollection : List<ExpressionElement>
    {
        public override string ToString()
        {
            string stringValue = "";

            foreach (var expressionElement in this)
                stringValue += expressionElement.ToString();

            return stringValue;
        }
    }

    /// <summary>
    /// A calculator to calculate and edit expression.
    /// </summary>
    public class Calculator
    {
        private readonly DataContext ParentDataContext;
        private readonly Type ParentDataContextType;
        public bool Calculated { get; private set; } = false;
        private ExpressionElementCollection ExpressionElements = new();

        /// <summary>
        /// Calculator class's generator
        /// </summary>
        /// <param name="parentDataContext">DataContext of the Page which initialize this Calculator</param>
        public Calculator(DataContext parentDataContext)
        {
            ParentDataContext = parentDataContext;
            ParentDataContextType = parentDataContext.GetType();
        }

        /// <summary>
        /// Check a number string is a valid decimal number or not.
        /// </summary>
        /// <param name="number">String to check.</param>
        /// <returns></returns>
        public bool IsValidNumber(string number)
        {
            string pattern = @"^-?[.]?$";
            return !Regex.Match(number, pattern).Success;
        }

        /// <summary>
        /// Check a number string is negative and valid decimal or not.
        /// </summary>
        /// <param name="number">Number string to check.</param>
        /// <returns></returns>
        public bool IsNegativeNumber(string number)
        {
            if (!IsValidNumber(number))
                return false;
            string pattern = @"^-";
            return Regex.Match(number, pattern).Success;
        }

        /// <summary>
        /// Add a number or operator to last position.
        /// </summary>
        /// <param name="element">Number or operator to add to expression element list</param>
        public void AddExpressionElement(ExpressionElement element)
        {
            if (Calculated)
            {
                ClearExpressionElement();
                return;
            }
            ExpressionElements.Add(element);
            (ParentDataContext as MainPageDataContext).RaisePropertyChange("ExpressionString");
        }

        /// <summary>
        /// Add a string represent a decimal number and return a status code.
        /// </summary>
        /// <param name="number">string represents decimal number.</param>
        /// <returns></returns>
        public int AddNumberExpressionElement(string number)
        {
            if (!IsValidNumber(number))
                return 1;
            AddExpressionElement(new ExpressionElement(decimal.Parse(number)));
            return 0;
        }

        /// <summary>
        /// Make expression empty.
        /// </summary>
        public void ClearExpressionElement()
        {
            Calculated = false;

            ExpressionElements.Clear();
            (ParentDataContext as MainPageDataContext).RaisePropertyChange("ExpressionString");
        }

        /// <summary>
        /// Get last expression element in the expression.
        /// </summary>
        /// <returns>Last expression element in the expression.</returns>
        public ExpressionElement GetLastExpressionElement()
        {
            if (ExpressionElements.Any())
                return ExpressionElements[ExpressionElements.Count - 1];
            return null;
        }

        /// <summary>
        /// Delete last number or operator in expression.
        /// </summary>
        public void DeleteLastExpressionElement()
        {
            if (ExpressionElements.Any())
                ExpressionElements.RemoveAt(ExpressionElements.Count - 1);
            (ParentDataContext as MainPageDataContext).RaisePropertyChange("ExpressionString");
        }

        /// <summary>
        /// Convert expression list to string.
        /// </summary>
        /// <returns>String represents expression.</returns>
        public string ExpressionElementToString()
        {
            return ExpressionElements.ToString();
        }

        public string Calculate()
        {
            Calculated = true;
            return new Expression(ExpressionElementToString()).Evaluate().ToString();
        }
    }
}