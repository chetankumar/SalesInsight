using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RedHill.SalesInsight.API.Validators
{
    public class ArrayRequired : ValidationAttribute
    {
        private int _minLength;

        public ArrayRequired()
        {
            _minLength = 1;
        }

        public ArrayRequired(int minLength)
        {
            _minLength = minLength;
        }

        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return false;
            }
            else
            {
                var list = value as Array;
                if (_minLength > list.Length)
                {
                    return false;
                }
            }
            return true;
        }
    }
}