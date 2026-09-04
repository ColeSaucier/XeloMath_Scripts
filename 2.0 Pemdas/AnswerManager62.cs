using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerManager62 : AnswerManagerBase
{
    public string stringEquation;
    private string randomSymbols = "x/+-(";
    private HashSet<char> usedOperators = new HashSet<char>();
    public int lengthOfEquation;
    private System.Random rnd = new System.Random(); // Created once
    public TextMeshProUGUI equationDisplayText;

    void Start()
    {
        lengthOfEquation = 0;
        stringEquation = "";
        usedOperators.Clear();
        generateEquation();
        equationDisplayText.text = stringEquation;
        answerString = SimpleMathEvaluatorSubtract.EvaluateFullEquation(stringEquation).ToString();
        Debug.LogError($"Generated equation: {stringEquation} = {answerString}");
    }

    public void generateEquation()
    {
        while (lengthOfEquation < 3)
        {
            if (lengthOfEquation >= 2)
            {
                randomSymbols = "x/+-";
            }

            List<char> availableSymbols = new List<char>();
            foreach (char c in randomSymbols)
            {
                if (!usedOperators.Contains(c))
                    availableSymbols.Add(c);
            }
            if (availableSymbols.Count == 0)
                availableSymbols = new List<char>(randomSymbols.ToCharArray());

            char symbol = availableSymbols[rnd.Next(availableSymbols.Count)];
            usedOperators.Add(symbol);

            if (symbol == '(')
            {
                randomSymbols = "x/+-";
                availableSymbols.Clear();
                foreach (char c in randomSymbols)
                {
                    if (!usedOperators.Contains(c))
                        availableSymbols.Add(c);
                }
                if (availableSymbols.Count == 0) availableSymbols = new List<char>("x/+-".ToCharArray());

                char secondSymbol = availableSymbols[rnd.Next(availableSymbols.Count)];
                usedOperators.Add(secondSymbol);
                lengthOfEquation += 2;

                string append_string = "";
                if (lengthOfEquation == 3)
                {
                    string randomnumberforthissymbol;
                    double previous_number = SimpleMathEvaluatorSubtract.EvaluateWithStrip(stringEquation, out randomnumberforthissymbol);
                    double sumWithinParenthesis_number = SimpleMathEvaluatorSubtract.GetMentalMathNumber(randomnumberforthissymbol, previous_number, stringEquation);

                    int randomNumber1 = 0;
                    int randomNumber2 = 0;
                    switch (secondSymbol)
                    {
                        case '+':
                            randomNumber1 = rnd.Next(-10, 29); // Allow negative
                            randomNumber2 = (int)sumWithinParenthesis_number - randomNumber1;
                            break;
                        case '-':
                            randomNumber1 = rnd.Next(-10, 29);
                            randomNumber2 = randomNumber1 - (int)sumWithinParenthesis_number;
                            break;
                        case 'x':
                            SimpleMathEvaluatorSubtract.GetMultiplicationPair((int)sumWithinParenthesis_number, out randomNumber1, out randomNumber2);
                            break;
                        case '/':
                            randomNumber2 = rnd.Next(1, 10);
                            randomNumber1 = (int)sumWithinParenthesis_number * randomNumber2;
                            break;
                        default:
                            throw new ArgumentException($"Unsupported symbol: {secondSymbol}");
                    }
                    append_string = $"({randomNumber1}{secondSymbol}{randomNumber2})";
                }
                else if (lengthOfEquation == 2)
                {
                    // Parenthesis first — allow negatives
                    if (secondSymbol == '/')
                    {
                        int randomNumber1 = rnd.Next(1, 12);
                        int randomNumber2 = rnd.Next(1, 10);
                        int numerator = randomNumber1 * randomNumber2;
                        int denominator = randomNumber2;
                        append_string = $"({numerator}/{denominator})";
                    }
                    else if (secondSymbol == 'x')
                    {
                        int randomNumber1 = rnd.Next(-10, 13); // Allow negative
                        int randomNumber2 = rnd.Next(1, 10);
                        append_string = $"({randomNumber1}x{randomNumber2})";
                    }
                    else if (secondSymbol == '+')
                    {
                        int randomNumber1 = rnd.Next(-10, 29);
                        int randomNumber2 = rnd.Next(-10, 29);
                        append_string = $"({randomNumber1}+{randomNumber2})";
                    }
                    else if (secondSymbol == '-')
                    {
                        int randomNumber1 = rnd.Next(-10, 29);
                        int randomNumber2 = rnd.Next(-10, randomNumber1 + 1);
                        append_string = $"({randomNumber1}-{randomNumber2})";
                    }

                    // Add operator after parenthesis
                    availableSymbols.Clear();
                    foreach (char c in "x/+-")
                    {
                        if (!usedOperators.Contains(c))
                            availableSymbols.Add(c);
                    }
                    if (availableSymbols.Count > 0)
                    {
                        char nextOp = availableSymbols[rnd.Next(availableSymbols.Count)];
                        usedOperators.Add(nextOp);
                        append_string += nextOp;
                    }
                }
                stringEquation += append_string;
            }
            else
            {
                lengthOfEquation += 1;
                string append_string = "";
                int number = 0;

                if (lengthOfEquation == 1)
                {
                    // First number — allow negative
                    bool makeNegative = rnd.NextDouble() < 0.3; // ~30% chance of negative start
                    int range = makeNegative ? -28 : 28;
                    number = makeNegative ? -rnd.Next(1, 29) : rnd.Next(1, 29);
                    append_string = $"{number}{symbol}";
                }
                else
                {
                    string dummy;
                    double previous_number = SimpleMathEvaluatorSubtract.EvaluateWithStrip(stringEquation, out dummy);
                    number = SimpleMathEvaluatorSubtract.GetMentalMathNumber(dummy, previous_number, stringEquation);
                    append_string = $"{number}{symbol}";

                    if (lengthOfEquation == 3)
                        append_string = $"{number}"; // Ends with no symbol
                }
                stringEquation += append_string;
            }
        }
        stringEquation = stringEquation.Replace("+-", "-");
    }

    public override void checkStringInput()
    {
        base.checkStringInput();
    }
}

public class SimpleMathEvaluatorSubtract
{
    public static double EvaluateWithStrip(string input, out string randomnumberforthissymbol)
    {
        if (string.IsNullOrEmpty(input) || input.Length < 2)
            throw new ArgumentException("Input string is too short.");

        randomnumberforthissymbol = input[^1].ToString();
        string rest = input[..^1].Trim();

        if (rest.StartsWith("(") && rest.EndsWith(")"))
        {
            string inside = rest[1..^1].Trim();
            return EvaluateFullEquation(inside); // Use full evaluator for parentheses
        }
        else
        {
            return EvaluateFullEquation(rest);
        }
    }

    public static int GetMentalMathNumber(string symbol, double answer, string currentEquation = null)
    {
        int intAnswer = (int)Math.Round(answer);
        Debug.LogError($"Answer {intAnswer.ToString()}");
        System.Random random = new System.Random();
        List<int> candidates = new List<int>();

        switch (symbol.Trim())
        {
            case "+":
                for (int i = -20; i <= 28; i++) // Allow negatives
                    if (i != 0) candidates.Add(i);
                break;
            case "-":
                for (int i = -20; i <= 28; i++)
                    if (i != 0) candidates.Add(i);
                break;
            case "x":
                // Allow negative factors when appropriate
                if (Math.Abs(intAnswer) > 10)
                {
                    for (int i = -5; i <= 5; i++)
                        if (i != 0) candidates.Add(i);
                }
                else
                {
                    for (int i = -10; i <= 10; i++)
                        if (i != 0) candidates.Add(i);
                }
                break;
            case "/":
                if (currentEquation is not null)
                {
                    string rest = currentEquation[..^1].Trim();
                    Debug.LogError($"currentEquation {currentEquation}");
                    Debug.LogError($"rest {rest}");

                    // multiplication/division/whole parenthesis case
                    bool no_subtraction_or_addition = (!currentEquation.Contains("-") && !currentEquation.Contains("+"));
                    if (rest.EndsWith(")") || no_subtraction_or_addition)
                    {
                        Debug.LogError($"Standard Division Case");
                    }
                    else
                    {
                        Debug.LogError($"UNIUWE Division Case");
                        // subtraction/ addition case
                        // Find the last whole number (from end backwards to first non-digit or operator)
                        int j = rest.Length - 1;
                        while (j >= 0 && char.IsDigit(rest[j])) j--;
                        j++;  // move back to first digit of the number

                        string lastNumberStr = rest.Substring(j).Trim();

                        int.TryParse(lastNumberStr, out int lastNumber);
                        Debug.LogError($"lastNumber {lastNumber.ToString()}");

                        // Now find divisors of lastNumber (positive only for simplicity in mental math)
                        intAnswer = Math.Abs(lastNumber);
                    }
                }
                Debug.LogError($"Division attempted, target is {intAnswer.ToString()}");
                // Only return divisors that divide evenly into the answer (positive & negative)
                for (int i = -20; i <= 20; i++)
                {
                    if (i != 0 && intAnswer % Math.Abs(i) == 0)
                    {
                        // Preserve sign of divisor
                        Debug.LogError($"i {i.ToString()}");
                        candidates.Add(i);
                    }
                }
                break;
            default:
                throw new ArgumentException($"Unsupported symbol: {symbol}");
        }

        if (candidates.Count == 0)
            throw new InvalidOperationException("No valid mental math numbers found.");

        int index = random.Next(candidates.Count);
        return candidates[index];
    }
    // Add this new static method to SimpleMathEvaluatorSubtract class
    public static void GetMultiplicationPair(int target, out int factor1, out int factor2)
    {
        if (target == 0)
        {
            // Special case for 0: common mental math pairs
            List<(int, int)> zeroPairs = new List<(int, int)>
            {
                (0, 1), (1, 0), (0, -1), (-1, 0), (0, 5), (5, 0)
            };
            var pair = zeroPairs[UnityEngine.Random.Range(0, zeroPairs.Count)];
            factor1 = pair.Item1;
            factor2 = pair.Item2;
            return;
        }

        int absTarget = Math.Abs(target);
        List<int> positiveFactors = new List<int>();

        // Find all positive factors
        for (int i = 1; i <= Math.Sqrt(absTarget); i++)
        {
            if (absTarget % i == 0)
            {
                positiveFactors.Add(i);
                if (i != absTarget / i)
                    positiveFactors.Add(absTarget / i);
            }
        }

        // Sort to prefer small factors first (mental math friendly)
        positiveFactors.Sort();

        // Prefer factors 1-10
        List<int> smallFactors = positiveFactors.FindAll(f => f <= 10);
        if (smallFactors.Count == 0) smallFactors = positiveFactors; // fallback

        // Randomly pick a small factor
        int chosenFactor = smallFactors[UnityEngine.Random.Range(0, smallFactors.Count)];

        // Second factor = target / chosen (preserves sign)
        factor1 = chosenFactor;
        factor2 = target / chosenFactor;

        // 50% chance to swap for variety
        if (UnityEngine.Random.value < 0.5f)
        {
            (factor1, factor2) = (factor2, factor1);
        }

        // Optional: allow negative factors randomly (for negative level)
        if (UnityEngine.Random.value < 0.3f) // 30% chance one is negative
        {
            factor1 = -factor1;
            factor2 = -factor2; // both negative to keep product positive if target positive
        }
    }

    public static int EvaluateFullEquation(string expression)
    {
        if (string.IsNullOrEmpty(expression))
            throw new ArgumentException("Expression cannot be empty.");

        expression = expression.Replace("x", "*").Trim();

        Stack<double> values = new Stack<double>();
        Stack<char> operators = new Stack<char>();
        int i = 0;

        while (i < expression.Length)
        {
            char c = expression[i];

            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            // Handle negative numbers (unary minus)
            if (c == '-' && (i == 0 || !char.IsDigit(expression[i - 1]) && expression[i - 1] != ')'))
            {
                i++;
                double val = 0;
                bool hasDigit = false;
                while (i < expression.Length && char.IsDigit(expression[i]))
                {
                    val = val * 10 + (expression[i] - '0');
                    i++;
                    hasDigit = true;
                }
                if (hasDigit)
                    values.Push(-val);
                else
                    throw new ArgumentException("Invalid negative number");
                continue;
            }

            if (char.IsDigit(c))
            {
                double val = 0;
                while (i < expression.Length && char.IsDigit(expression[i]))
                {
                    val = val * 10 + (expression[i] - '0');
                    i++;
                }
                values.Push(val);
                continue;
            }

            if (c == '(')
            {
                operators.Push(c);
                i++;
                continue;
            }

            if (c == ')')
            {
                while (operators.Count > 0 && operators.Peek() != '(')
                {
                    values.Push(ApplyOperator(values, operators.Pop()));
                }
                if (operators.Count == 0 || operators.Peek() != '(')
                    throw new ArgumentException("Mismatched parentheses.");
                operators.Pop();
                i++;
                continue;
            }

            if (IsOperator(c))
            {
                while (operators.Count > 0 &&
                       operators.Peek() != '(' &&
                       GetPriority(operators.Peek()) >= GetPriority(c))
                {
                    values.Push(ApplyOperator(values, operators.Pop()));
                }
                operators.Push(c);
                i++;
                continue;
            }

            throw new ArgumentException($"Invalid character: {c}");
        }

        while (operators.Count > 0)
        {
            if (operators.Peek() == '(')
                throw new ArgumentException("Mismatched parentheses.");
            values.Push(ApplyOperator(values, operators.Pop()));
        }

        if (values.Count != 1)
            throw new InvalidOperationException("Evaluation error.");

        return (int)Math.Round(values.Pop());
    }

    private static bool IsOperator(char c) => c == '+' || c == '-' || c == '*' || c == '/';

    private static int GetPriority(char op) => (op == '*' || op == '/') ? 2 : 1;

    private static double ApplyOperator(Stack<double> values, char op)
    {
        if (values.Count < 2)
            throw new InvalidOperationException("Not enough values.");

        double b = values.Pop();
        double a = values.Pop();

        return op switch
        {
            '+' => a + b,
            '-' => a - b,
            '*' => a * b,
            '/' => a / b,
            _ => throw new ArgumentException("Invalid operator")
        };
    }
}
