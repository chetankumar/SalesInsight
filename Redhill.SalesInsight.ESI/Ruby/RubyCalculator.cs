
using Microsoft.Scripting.Utils;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using IronRuby.Builtins;
using Utils;

namespace Redhill.SalesInsight.ESI
{
    public class RubyCalculator
    {
        public int COUNT(dynamic value)
        {
            if (value is ICollection)
            {
                var casted = (ICollection)value;
                int count = casted.Count;
                return count;
            }
            return 0;
        }

        /**
         * def average(values)
            count = values.count
            total = 0
            values.collect {|v| total += v}
            return (total.to_f/count)
          end
        */
        public double AVERAGE(dynamic value)
        {
            if (value is ICollection)
            {
                var casted = (ICollection)value;
                int count = casted.Count;
                double sum = 0;
                foreach (var val in casted)
                {
                    sum += Convert.ToDouble(val);
                }
                return sum / count;
            }
            return Convert.ToDouble(value);
        }

        /**
         * def is_blank(value)
                if (value.class == Array || value.class == Hash)
                    return value.empty?
                else
                    return value == "" || value == nil
                end
            end
         */
        public bool IS_BLANK(dynamic value)
        {
            if (value == null)
            {
                return true;
            }
            if (value is ICollection)
            {
                return value.Count == 0;
            }
            if (value is IDictionary)
            {
                var casted = (IDictionary)value;
                return casted.Keys.Count == 0;
            }
            if (value is String)
            {
                return value.Trim() == "";
            }
            if (value is DateTime)
            {
                return false;
            }
            return false;
        }

        public bool IS_NOT_BLANK(dynamic value)
        {
            if (value == null)
            {
                return false;
            }
            if (value is ICollection)
            {
                return value.Count > 0;
            }
            if (value is IDictionary)
            {
                var casted = (IDictionary)value;
                return casted.Keys.Count > 0;
            }
            if (value is String)
            {
                return value.Trim() != "";
            }
            if (value is DateTime)
            {
                return true;
            }
            return false;
        }

        public string STRINGIFY(dynamic value)
        {
            if (value is IDictionary)
            {
                List<string> output = new List<string>();
                value = (Dictionary<string, dynamic>)value;
                foreach (var val in value)
                {
                    output.Add($"{val.Key} : {val.Value}");
                }
                return "{ " + string.Join("; ", output) + " }";
            }
            else if (value is ICollection)
            {
                List<string> outputList = new List<string>();
                foreach (var val in value)
                {
                    outputList.Add(STRINGIFY(val));
                }
                return "[ " + string.Join(";| ", outputList) + " ]";
            }
            else
                return value;
        }

        // Need To Review
        public bool CONTAINS(dynamic source, dynamic value, string attribute = null)
        {
            if (source == null)
                return false;
            ICollection tmpList = null;
            if (source is IDictionary)
            {
                if (attribute != null)
                {
                    var obj = source[attribute];
                    return CONTAINS(obj, value);
                }
                var casted = (IDictionary)source;
                tmpList = casted.Values;
            }
            else if (source is ICollection)
            {
                if (attribute != null)
                {
                    tmpList = source;
                }
                else
                {
                    tmpList = source;
                }

            }
            else
            {
                return CheckEqual(source, value);
            }

            if (tmpList.Count == 0)
                return false;

            bool result = false;
            foreach (var i in tmpList)
            {
                dynamic item = i;
                if (item is IDictionary)
                {
                    if (attribute != null)
                    {
                        result = result || CheckEqual(item[attribute], value);
                    }
                    else
                    {
                        ICollection itemValues = ((IDictionary)item).Values;
                        foreach (var y in itemValues)
                        {
                            result = result || CheckEqual(y, value);
                        }
                    }
                }
                else if (item is ICollection)
                {
                    result = result || CONTAINS(item, value, attribute);
                }
                else
                {
                    result = result || CheckEqual(item, value);
                }
            }
            return result;
        }

        public List<string> SPLIT(dynamic value, char splitChar)
        {
            if (value == null)
            {
                return null;
            }
            string tmpVal = null;
            if (value is MutableString)
                tmpVal = value.ConvertToString();
            else
                tmpVal = value;

            return tmpVal.Split(new char[] { splitChar }).ToList();
        }

        public double DATE_DIFF(DateTime date1, DateTime date2)
        {
            TimeSpan t = date1 - date2;
            return t.TotalDays;
        }

        public DateTime DATE_ADD(DateTime date1, int days)
        {
            return date1.AddDays(days);
        }

        public double MONTH_DIFF(DateTime date1, DateTime date2)
        {
            TimeSpan t = date1 - date2;
            return t.TotalDays / 30.0;
        }
        // Need To Review
        public DateTime MONTH_ADD(DateTime date1, int count)
        {
            return date1.AddMonths(count);
        }

        public double MULTIPLY(dynamic values)
        {
            double product = 1;
            if (values is ICollection)
            {
                foreach (var value in values)
                {
                    double tmp = Convert.ToDouble(value);
                    product *= tmp;
                }
            }
            return product;
        }
        public double SQUARE(double value)
        {
            return value * value;
        }

        public double SUM(dynamic values)
        {
            double sum = 0;
            if (values != null)
            {
                if (values is ICollection)
                {
                    foreach (var value in values)
                    {
                        double tmp = Convert.ToDouble(value);
                        sum += tmp;
                    }
                }
            }
            return sum;
        }

        public List<dynamic> PLUCK(dynamic values, string attribute)
        {
            List<dynamic> output = new List<dynamic>();
            if (values is ICollection)
            {
                foreach (var val in values)
                {
                    if (val is IDictionary)
                    {
                        var tpm = val[attribute];
                        output.Add(tpm);
                    }
                }
            }
            return output;
        }

        public dynamic UPDATE_ITEM_AT(dynamic list, int index, dynamic item, string attribute = null)
        {
            if (list == null)
                return null;


            if (item != null && item is MutableString)
                item = item.ConvertToString();

            if (list is ICollection)
            {
                if (index < list.Count)
                {
                    if (attribute != null)
                    {
                        list[index][attribute] = item;
                    }
                    else
                    {
                        list[index] = item;
                    }
                }
            }
            return list;
        }

        private bool CheckEqual(dynamic value1, dynamic value2)
        {
            if (value1 is string && value2 is MutableString)
                return value1.Contains(value2.ConvertToString());
            else if (value2 is MutableString)
                return false;
            else
                return value1.Contains(value2);
        }

        public dynamic ITEM_NUM(dynamic source, int index, string attribute = null)
        {
            if (source == null)
                return null;

            if (source is ICollection)
            {
                if (index >= 0 && index < source.Count)
                {
                    if (attribute != null)
                    {
                        return source[index][attribute];
                    }
                    else
                    {
                        return source[index];
                    }
                }
            }
            return null;
        }

        public dynamic NUM_OF(dynamic source, int index)
        {
            if (source == null)
                return null;

            if (source is ICollection)
            {
                return source[index];
            }
            return null;
        }

        public dynamic APPEND(dynamic list, dynamic item)
        {
            if (list == null)
                return null;

            if (list != null && item is MutableString)
                item = item.ConvertToString();

            if (list is ICollection)
            {
                list.Add(item);
            }
            return list;
        }

        public dynamic ROUND(double value)
        {
            return Math.Round(value);
        }

        public dynamic ABS(double value)
        {
            return Math.Abs(value);
        }

        public dynamic PMT(double rate, double nper, double pv, double fv, double type)
        {
            return Microsoft.VisualBasic.Financial.Pmt(rate, nper, pv, fv, (Microsoft.VisualBasic.DueDate)type);
        }
        public dynamic IPMT(double rate, double per, double nper, double pv, double fv, double type)
        {
            return Microsoft.VisualBasic.Financial.IPmt(rate, per, nper, pv, fv, (Microsoft.VisualBasic.DueDate)type);
        }
        public dynamic PPMT(double rate, double per, double nper, double pv, double fv, double type)
        {
            return Microsoft.VisualBasic.Financial.PPmt(rate, per, nper, pv, fv, (Microsoft.VisualBasic.DueDate)type);
        }

        public dynamic SORT(dynamic source, string field = null)
        {
            if (source is List<Dictionary<string, dynamic>>)
            {
                return ((List<dynamic>)source).OrderBy(d => d[field]).ToList();
            }
            else if (source is List<List<Dictionary<string, dynamic>>>)
            {
                return source;
            }
            else if (source is List<double>)
            {
                return ((List<double>)source).OrderBy(d => d).ToList();
            }
            else if (source is List<string>)
            {
                return ((List<string>)source).OrderBy(d => d).ToList();
            }
            return source;
        }

        public dynamic SORT_DESC(dynamic source, string field = null)
        {
            if (source is List<Dictionary<string, dynamic>>)
            {
                return ((List<dynamic>)source).OrderByDescending(d => d[field]).ToList();
            }
            else if (source is List<List<Dictionary<string, dynamic>>>)
            {
                return source;
            }
            else if (source is List<double>)
            {
                return ((List<double>)source).OrderByDescending(d => d).ToList();
            }
            else if (source is List<string>)
            {
                return ((List<string>)source).OrderByDescending(d => d).ToList();
            }
            return source;
        }

        public double MIN(dynamic values)
        {
            double min = 0;
            if (values is ICollection)
            {
                List<dynamic> items = SORT(values);
                min = Convert.ToDouble(items[0]);
            }
            return min;
        }

        public double MAX(dynamic values)
        {
            double min = 0;
            if (values is ICollection)
            {
                List<dynamic> items = SORT_DESC(values);
                min = Convert.ToDouble(items[0]);
            }
            return min;
        }

        public dynamic MERGE_HASH(dynamic oldHash, dynamic newHash)
        {
            if (oldHash is IDictionary && newHash is IDictionary)
            {
                foreach (var item in (Dictionary<string, dynamic>)newHash)
                {
                    if (item.Value != null)
                    {
                        oldHash[item.Key] = newHash[item.Key];
                    }
                }
            }
            return oldHash;
        }
        public dynamic MERGE_ITEM_AT(dynamic list, int index, dynamic item)
        {
            if (list == null)
                list = new List<dynamic>();

            dynamic oldItem = list[index];
            if (oldItem == null)
                list[index] = item;
            return list;
        }
        public dynamic MERGE_ALL_ITEMS_AT(dynamic list, int index, dynamic item, string attribute = null)
        {
            if (list == null)
                list = new List<dynamic>();

            dynamic oldItem = list[index];
            if (oldItem == null)
                list[index] = item;
            else
            {
                if (oldItem is ICollection && item is ICollection)
                {
                    for (var i = 0; i < oldItem.Count(); i++)
                    {
                        if (attribute != null)
                        {

                        }
                        else
                        {
                            oldItem[i] = MERGE_HASH(oldItem[i], item[i]);
                        }
                    }
                }
                else if (oldItem is IDictionary)
                {
                    oldItem = MERGE_HASH(oldItem, item);
                }
                list[index] = oldItem;
            }
            return list;
        }

        //NUM_OF
        //UPDATE

        public DateTime NEW_DATE(string value)
        {
            return DateUtils.ParseSafe(value).Value;
        }
    }
}
