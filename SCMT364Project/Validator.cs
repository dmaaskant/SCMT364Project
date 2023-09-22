using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace SCMT364Project
{
    internal class Validator
    {
        // Only has functions
        SolidColorBrush highlightColor = Brushes.PaleVioletRed;

        public bool isValidRow(TextBox tn, TextBox tt, TextBox p)
        {
            if (!isValidTN(tn))
            {
                //MessageBox.Show($"{tn.Text} is invalid, please change input");
                //tn.Focus();
                return false;
            }
            if (!isValidTT(tt))
            {
                //MessageBox.Show($kk"{tt.Text} is invalid, please change input");
                //tt.Focus();
                return false;
            }

            if (!isValidP(p))
            {
                //MessageBox.Show($"{p.Text} is invalid, please change input");
                //p.Focus();
                return false;
            }
            if (!hasTNandTT(tn, tt))
            {
                MessageBox.Show($"Task Name and Time have to both be present, please change input");
                tn.Background = highlightColor;
                tt.Background = highlightColor;
                tn.Focus();
                return false;
            }
            return true;
        }
        public bool isValidInputCell(TextBox box)
        {
            if (!isLength(10, box))
            {
                MessageBox.Show("Please decrease length of input");
                box.Background = highlightColor;
                box.Focus();
                return false;
            }
            if (!isNumeric(box))
            {
                MessageBox.Show("Please only input numeric data");
                box.Background = highlightColor;
                box.Focus();
                return false;
            }
            if (box.Text.Trim().Length == 0)
            {
                MessageBox.Show("Input is required");
                box.Background = highlightColor;
                box.Focus();
                return false;
            }
            return true;
        }
        public bool isValidTN(TextBox box)
        {
            if (!isLength(5, box))
            {
                MessageBox.Show("Please input max 5 characters");
                box.Background = highlightColor;
                box.Focus();
                return false;
            }
            if (!isAlphaNumeric(box))
            {
                MessageBox.Show("Please input only A-O or 1-15");
                box.Background = highlightColor;
                box.Focus();
                return false;
            }
            return true;
        }
        public bool isValidTT(TextBox box)
        {
            if (!isLength(10, box))
            {
                MessageBox.Show("Please inupt max 10 characters");
                box.Background = highlightColor;
                box.Focus();
                return false;
            }
            if (!isNumeric(box))
            {
                MessageBox.Show("Please input only numeric characters");
                box.Background = highlightColor;
                box.Focus();
                return false;
            }
            return true;
        }
        public bool isValidP(TextBox box)
        {
            if (!isLength(30, box))
            {
                MessageBox.Show("Please input only 30 characters");
                box.Background = highlightColor;
                box.Focus();
                return false;
            }
            if (!isValidParentText(box))
            {
                MessageBox.Show("Please input only A-O or 1-15");
                box.Background = highlightColor;
                box.Focus();
                return false;
            }
            return true;
        }
        public bool isLength(int char_length, TextBox box)
        {
            string text = box.Text.Trim();
            return (text.Length <= char_length);
        }

        /// <summary>
        /// Function does not accept decimals or floats because period is not letter or digit
        /// Is good because its only validated for task names
        /// Should only accept "A"-"Z" 
        /// </summary>
        /// <param name="box"></param>
        /// <returns> false for 12.3 because of the period.</returns>
        public bool isAlphaNumeric(TextBox box)
        {

            string text = box.Text.Trim();
            if (text.Length == 0) return true;
            HashSet<string> valid_strings = new HashSet<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" };
            if (valid_strings.Contains(box.Text.Trim())) return true;
            return false;
        }
        public bool isAlphaNumeric(string text)
        {
            if (text.Length == 0) return true;
            HashSet<string> valid_strings = new HashSet<string> { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" };
            if (valid_strings.Contains(text.Trim())) return true;
            return false;
        }
        public bool isValidParentText(TextBox box)
        {
            string[] inputs = box.Text.Trim().Split(' ');
            foreach (string input in inputs)
            {
                if (!isAlphaNumeric(input))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="box"></param>
        /// <returns></returns>
        public bool isNumeric(TextBox box)
        {
            string numString = box.Text.Trim(); //"1287543.0" will return false for a long
            if (numString.Length == 0) return true;
            decimal number1 = 0;
            return decimal.TryParse(numString, out number1);
        }
        public bool hasTNandTT(TextBox name, TextBox time)
        {
            string tName = name.Text.Trim();
            string tTime = time.Text.Trim();
            if (tName == String.Empty && tTime == String.Empty) { return true; } // Both empty is also valid
            return (tName != String.Empty && tTime != String.Empty); // One empty and other isn't, not valid
        }

    }
}
