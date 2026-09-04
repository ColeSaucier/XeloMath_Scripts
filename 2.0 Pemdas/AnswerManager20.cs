using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
//using System.Diagnostics;
//using System.Diagnostics;

public class AnswerManager20 : AnswerManagerBase
{
    public string stringEquation;
    private string randomSymbols = "x/+-(";
    private HashSet<char> usedOperators = new HashSet<char>();
    public int lengthOfEquation;
    private System.Random rnd;  // ← Add this
    //public static AnswerHolder AnswerHolder;
    public TextMeshProUGUI equationDisplayText;
    public bool subtraction_first_handle_case = false;

    public void Start()
    {
        lengthOfEquation = 0;
        stringEquation = "";
        usedOperators.Clear();     // Reset the set
        rnd = new System.Random(); // ← Create ONCE per generation
        generateEquation();
        equationDisplayText.text = stringEquation;
        answerString = SimpleMathEvaluator.EvaluateFullEquation(stringEquation).ToString();
        Debug.Log($"Generated equation: {stringEquation} = {answerString}");
    }

    /*public void evaluateAnswerViaPemdas_CheckForRulesOfEquation()
    {
    }*/

    public void generateEquation()
    {
        /* Rules for this equation
        - end answer must be a whole number
            - division must calculate a whole number
                - downstream division must be divided by a factor
            - if end answer is not a whole number, re run
        - multiplication must be doable with mental math
            - within 10 times 10
            - or only 234\
            - downstream multiplcation must be within 10 times 10
        - symbol must flip after adding a new digit
        - signs allowable () X / + -
        -addition and subtraction can use numbers bigger than 10
        */

        while (lengthOfEquation < 3)
        {
            if (lengthOfEquation >= 2)
            {
                randomSymbols = "x/+-";
            }

            // Build list of currently allowed symbols (excluding already used ones)
            List<char> availableSymbols = new List<char>();
            foreach (char c in randomSymbols)
            { 
                if (!usedOperators.Contains(c))
                {
                    availableSymbols.Add(c);
                }
            }
            // If somehow no symbols left (shouldn't happen), fall back to all
            if (availableSymbols.Count == 0)
                availableSymbols = new List<char>(randomSymbols.ToCharArray());

            foreach (var item in availableSymbols)
            {
                //Debug.LogError($"{item}");
            }

            //System.Random rnd = new System.Random();
            char symbol = availableSymbols[rnd.Next(availableSymbols.Count)];
            /*
            if (lengthOfEquation == 0)
            {
                symbol = '-';
            }
            */
            // Add used operator to blacklist
            usedOperators.Add(symbol);

            if (symbol == '(')
            {
                // We must handle parenthesis uniquely as it requires a sub equation
                randomSymbols = "x/+-";
                
                // Re-build available list for inside parentheses (respect already used operators)
                availableSymbols.Clear();
                foreach (char c in randomSymbols)
                {
                    if (!usedOperators.Contains(c))
                        availableSymbols.Add(c);
                }
                if (availableSymbols.Count == 0) availableSymbols = new List<char>("x/+-".ToCharArray());

                char secondSymbol = availableSymbols[rnd.Next(availableSymbols.Count)];
                usedOperators.Add(secondSymbol); // Also ban the inner operator
                lengthOfEquation += 2;

                // Add used operator to blacklist
                usedOperators.Add(secondSymbol);

                string append_string = "";

                if (lengthOfEquation == 3)
                {
                    string randomnumberforthissymbol;
                    double previous_number = SimpleMathEvaluator.EvaluateWithStrip(stringEquation, out randomnumberforthissymbol);
                    //Debug.LogError($"previous_number, {previous_number}");
                    subtraction_first_handle_case = false;
                    double sumWithinParenthesis_number = SimpleMathEvaluator.GetMentalMathNumber(randomnumberforthissymbol, previous_number, subtraction_first_handle_case);
                    //Debug.LogError($"sumWithinParenthesis_number, {sumWithinParenthesis_number}");

                    int randomNumber1 = 0;
                    int randomNumber2 = 0;

                    switch (secondSymbol)
                    {
                        case '+':
                            // Addition: numbers 1 to 28
                            randomNumber1 = rnd.Next(1, (int)sumWithinParenthesis_number);
                            randomNumber2 = (int)sumWithinParenthesis_number - randomNumber1;
                            break;
                        case '-':
                            // Subtraction: second number must be smaller than answer
                            randomNumber1 = rnd.Next(1, (int)sumWithinParenthesis_number + 28);
                            randomNumber2 =  randomNumber1 - (int)sumWithinParenthesis_number;
                            break;
                        case 'x':
                            // Small result → allow up to 10
                            randomNumber1 = SimpleMathEvaluator.GetMentalMathNumber("/", sumWithinParenthesis_number, subtraction_first_handle_case);
                            randomNumber2 = (int)sumWithinParenthesis_number / randomNumber1;
                            break;
                        case '/':
                            // Division: only return factors of the answer #NOT A TYPO
                            randomNumber2 = rnd.Next(1, 10);
                            randomNumber1 = (int)sumWithinParenthesis_number * randomNumber2;
                            break;
                        default:
                            throw new ArgumentException($"Unsupported symbol: {secondSymbol}");
                    }
                    append_string = $"({randomNumber1}{secondSymbol}{randomNumber2})"; //Ends with no symbol
                }

                if (lengthOfEquation == 2)
                {
                    // Parenthesis is first
                    if (secondSymbol == '/')
                    {
                        int randomNumber1 = rnd.Next(1, 12);
                        int randomNumber2 = rnd.Next(1, 10);
                        int numerator = randomNumber1 * randomNumber2;
                        int denominator = randomNumber2;
                        append_string = $"({numerator}/{denominator})";
                    }
                    if (secondSymbol == 'x')
                    {
                        int randomNumber1 = rnd.Next(1, 12);
                        int randomNumber2 = rnd.Next(1, 10);
                        append_string = $"({randomNumber1}x{randomNumber2})";
                    }
                    if (secondSymbol == '+')
                    {
                        int randomNumber1 = rnd.Next(1, 28);
                        int randomNumber2 = rnd.Next(1, 28);
                        append_string = $"({randomNumber1}+{randomNumber2})";
                    }
                    if (secondSymbol == '-')
                    {
                        int randomNumber1 = rnd.Next(2, 28);
                        int randomNumber2 = rnd.Next(1, randomNumber1);
                        append_string = $"({randomNumber1}-{randomNumber2})";
                    }

                    // Add the operator AFTER the parenthesis — also ban it
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
                //randomSymbols = "x/+-";
                lengthOfEquation += 1;

                List<int> candidates = new List<int>();
                string append_string = "";
                int number = 0;

                // length of equation rn is technically empty
                if (lengthOfEquation == 1)
                {
                    switch (symbol)
                    {
                        case '+':
                            // Addition: numbers 1 to 28
                            for (int i = 1; i <= 28; i++)
                                candidates.Add(i);
                            break;
                        case '-':
                            // Subtractuib (after): numbers 1 to 28
                            for (int i = 12; i <= 42; i++)
                                candidates.Add(i);
                            if (lengthOfEquation == 1)
                                subtraction_first_handle_case = true;
                            break;
                        case 'x':
                            // Small result → allow up to 10
                            for (int i = 1; i <= 10; i++)
                                candidates.Add(i);
                            break;
                        case '/':
                            // Division: only return factors of the answer
                            int randomNumber1 = rnd.Next(2, 12);
                            int randomNumber2 = rnd.Next(1, 10);
                            int numerator = randomNumber1 * randomNumber2;
                            candidates.Add(numerator);
                            // Always include 1 and the number itself
                            break;
                        default:
                            throw new ArgumentException($"Unsupported symbol: {symbol}");
                    }

                    if (candidates.Count == 0)
                        throw new InvalidOperationException("No valid mental math numbers found.");

                    // Return one random number from the list
                    int index = rnd.Next(candidates.Count);
                    number = candidates[index];
                    append_string = $"{number}{symbol}";
                }
                else
                {
                    string dummy;
                    double previous_number = SimpleMathEvaluator.EvaluateWithStrip(stringEquation, out dummy);
                    number = SimpleMathEvaluator.GetMentalMathNumber(dummy, previous_number, subtraction_first_handle_case,  stringEquation); //Probl;em
                    append_string = $"{number}{symbol}";   

                    if (lengthOfEquation == 3)
                        append_string = $"{number}"; //Ends with no symbol
                }
                stringEquation += append_string;
                //Debug.LogError($"stringEquation {stringEquation}");
            }
        }
    }

    public static string RemoveSubstring(string original, string toRemove)
    {
        if (string.IsNullOrEmpty(original) || string.IsNullOrEmpty(toRemove))
            return original;
        return original.Replace(toRemove, "");
    }

    public override void checkStringInput()
    {
        // Set the answer string using the specific logic for this class
        //answerString = AnswerHolder.answer.ToString();
        // Call the base class's checkStringInput method to handle the rest of the logic
        base.checkStringInput();
    }
}

public class SimpleMathEvaluator
{
    private static double EvaluateSimple(string expr, bool not_parenthesis)
    {
        expr = expr.Trim();
        // Find the operator position
        int opPos = -1;
        char op = '\0';
        for (int i = 0; i < expr.Length; i++)
        {
            char c = expr[i];
            if (c == '+' || c == '-' || c == 'x' || c == '/')
            {
                if (opPos != -1)
                    throw new ArgumentException("Expression has more than one operator.");
                opPos = i;
                op = c;
            }
        }
        if (opPos == -1)
        {
            // No operator, just a number
            return double.Parse(expr);
        }
        // Split into left and right operands
        string leftStr = expr.Substring(0, opPos).Trim();
        string rightStr = expr.Substring(opPos + 1).Trim();
        double left = double.Parse(leftStr);
        double right = double.Parse(rightStr);

        if (not_parenthesis)
        {
            return op switch
            {
                '+' => right,
                '-' => right,
                'x' => left * right,
                '/' => left / right,
                _ => throw new ArgumentException("Invalid operator.")
            };
        }
        else
        {
            return op switch
            {
                '+' => left + right,
                '-' => left - right,
                'x' => left * right,
                '/' => left / right,
                _ => throw new ArgumentException("Invalid operator.")
            };
        }
    }

    /// <summary>
    /// Main function: Strips the last character from the input string, stores it as 'randomnumberforthissymbol',
    /// then evaluates the remaining string as a math term according to the rules:
    /// - If wrapped in parentheses, evaluate the inside (any operator: +, -, *, /).
    /// - Otherwise, if it contains multiplication or division, evaluate it.
    /// - Otherwise, treat as a single number.
    /// Assumes valid input with at most two numbers and one symbol, no nested parentheses, no recursion.
    /// Returns the evaluated result of the remaining string after stripping the last character.
    /// </summary>
    public static double EvaluateWithStrip(string input, out string randomnumberforthissymbol)
    {
        if (string.IsNullOrEmpty(input) || input.Length < 2)
            throw new ArgumentException("Input string is too short.");
        // Strip the last character and store it
        randomnumberforthissymbol = input[input.Length - 1].ToString();
        // Get the rest
        string rest = input.Substring(0, input.Length - 1).Trim();
        // First, check if it's wrapped in parentheses
        if (rest.StartsWith("(") && rest.EndsWith(")"))
        {
            string inside = rest.Substring(1, rest.Length - 2).Trim();
            return EvaluateSimple(inside, false);
        }
        else
        {
            return EvaluateSimple(rest, true);
        }
    }

    /// <summary>
    /// Given the operator symbol and the correct answer from EvaluateWithStrip,
    /// returns ONE random, mentally reasonable number to use in a question.
    /// </summary>
    public static int GetMentalMathNumber(string symbol, double answer, bool subtraction_handle_case, string equation_forSubtractionCase = "")
    {
        int intAnswer = (int)Math.Round(answer);
        //Debug.LogError($"intAnswer {intAnswer}");

        System.Random random = new System.Random(); // Explicit System.Random - no ambiguity
        List<int> candidates = new List<int>();
        //Debug.LogError($"symbol.Trim() {symbol.Trim()}");

        switch (symbol.Trim())
        {
            case "+":
                // Addition: numbers 1 to 28
                for (int i = 1; i <= 28; i++)
                    candidates.Add(i);
                break;
            case "-":
                // Subtraction: second number must be smaller than answer
                for (int i = 1; i <= intAnswer; i++)
                    candidates.Add(i);
                break;
            case "x":
                if (subtraction_handle_case)
                {    
                    // Expected format: e.g. "10-3*" or "5-2*"
                    string cleaned = equation_forSubtractionCase.TrimEnd('x').Trim(); // remove trailing *
                    int minusIndex = cleaned.IndexOf('-');

                    string firstStr = cleaned.Substring(0, minusIndex).Trim();
                    string secondStr = cleaned.Substring(minusIndex + 1).Trim();

                    // Max multiplier to keep (first - second * mult) >= 0
                    // mult <= first / second
                    if (int.TryParse(firstStr, out int firstInt) && 
                        int.TryParse(secondStr, out int secondInt) && 
                        secondInt > 0)
                    {
                        // Max multiplier to keep (first - second * mult) >= 0
                        // mult <= floor(first / second)
                        int maxMultiplier = Mathf.FloorToInt(firstInt / (float)secondInt);

                        for (int i = 1; i <= maxMultiplier; i++)
                        {
                            candidates.Add(i);
                        }
                    }
                }
                else
                {
                    // Multiplication: small factors only
                    if (intAnswer > 10)
                    {
                        // Big result → use tiny multipliers: 1–4
                        for (int i = 1; i <= 4; i++)
                            candidates.Add(i);
                    }
                    else
                    {
                        // Small result → allow up to 10
                        for (int i = 1; i <= 10; i++)
                            candidates.Add(i);
                    }                    
                }
                break;
            case "/":
                // Division: only return factors of the answer
                for (int i = 2; i <= intAnswer; i++)
                {
                    if (intAnswer % i == 0)
                        candidates.Add(i);
                }
                break;
            default:
                throw new ArgumentException($"Unsupported symbol: {symbol}");
        }

        if (candidates.Count == 0)
            throw new InvalidOperationException("No valid mental math numbers found.");
        // Return one random number from the list
        int index = random.Next(candidates.Count);
        foreach (var item in candidates)
        {
            //Debug.LogError($"{item}");
        }
        return candidates[index];
    }
    public static int EvaluateFullEquation(string expression)
    {
        if (string.IsNullOrEmpty(expression))
            throw new ArgumentException("Expression cannot be empty.");

        // Replace 'x' with '*' so we can use standard operator handling
        expression = expression.Replace("x", "*");

        Stack<double> values = new Stack<double>();
        Stack<char> operators = new Stack<char>();

        int i = 0;
        while (i < expression.Length)
        {
            char c = expression[i];

            // Skip whitespace (your generator doesn't add any, but safe)
            if (char.IsWhiteSpace(c))
            {
                i++;
                continue;
            }

            // Parse numbers (supports multi-digit)
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

            // Opening parenthesis
            if (c == '(')
            {
                operators.Push(c);
                i++;
                continue;
            }

            // Closing parenthesis
            if (c == ')')
            {
                while (operators.Count > 0 && operators.Peek() != '(')
                {
                    double result = ApplyOperator(values, operators.Pop());
                    values.Push(result);
                }
                if (operators.Count == 0 || operators.Peek() != '(')
                    throw new ArgumentException("Mismatched parentheses.");
                operators.Pop(); // remove '('
                i++;
                continue;
            }

            // Operator (+, -, *, /)
            if (IsOperator(c))
            {
                while (operators.Count > 0 &&
                       operators.Peek() != '(' &&
                       GetPriority(operators.Peek()) >= GetPriority(c))
                {
                    double result = ApplyOperator(values, operators.Pop());
                    values.Push(result);
                }
                operators.Push(c);
                i++;
                continue;
            }

            throw new ArgumentException($"Invalid character in expression: {c}");
        }

        // Apply any remaining operators
        while (operators.Count > 0)
        {
            if (operators.Peek() == '(')
                throw new ArgumentException("Mismatched parentheses.");
            double result = ApplyOperator(values, operators.Pop());
            values.Push(result);
        }

        if (values.Count != 1)
            throw new InvalidOperationException("Evaluation error: incorrect number of values.");

        // Your rules guarantee a whole number result
        return (int)Math.Round(values.Pop());
    }

    private static bool IsOperator(char c)
    {
        return c == '+' || c == '-' || c == '*' || c == '/';
    }

    private static int GetPriority(char op)
    {
        return (op == '*' || op == '/') ? 2 : 1;
    }

    private static double ApplyOperator(Stack<double> values, char op)
    {
        if (values.Count < 2)
            throw new InvalidOperationException("Not enough values for operator.");

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