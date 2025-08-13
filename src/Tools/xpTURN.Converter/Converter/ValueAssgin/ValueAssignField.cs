using System;
using System.Reflection;

namespace xpTURN.Converter.AssginValue
{
    public class ValueAssignField : IAssignField
    {
        public static ValueAssignField Default = new ();

        public bool AssignValue(FieldInfo fieldInfo, object target, object value)
        {
            try
            {
                // Set the value directly to the field
                fieldInfo.SetValue(target, value);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
