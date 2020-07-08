using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EthCanConfig.Models
{
    class NumberSetting : TypedSetting<int>
    {
        public NumberSetting(string name, int value) : base(name, value)
        {
        }
    }
    class UnsignedNumberSetting : TypedSetting<uint>
    {
        public UnsignedNumberSetting(string name, uint value) : base(name, value) { }
    }
    class HexadecimalSetting : UnsignedNumberSetting
    {
        public int Length;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="length">Count of hexadecimal numbers displayed</param>
        public HexadecimalSetting(string name, uint value, int length) : base(name, value) {
            Length = length;
        }
        [HexValidation]
        public override string StringValue
        {
            get
            {
                return string.Format("{0:X"+Length.ToString()+"}", TypedValue);
            }
            set
            {
                TypedValue = Convert.ToUInt32(value, 16);
            }
        }
    }

    public class HexValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var str = value as string;
            Regex regex = new Regex("[0-9a-fA-F]+");
            if (regex.IsMatch(str))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("Text is not hex formatted");
        }
    }
}