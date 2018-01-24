using System;

namespace Tabula
{
    public static class Formatter
    {
        public static string SearchName_from_MethodName(string methodName)
        {
            return methodName.Replace("_", "").ToLower();
        }

        public static string InstanceName_from_TypeName(string typeName)
        {
            var lastDot = typeName.LastIndexOf('.');
            return typeName.Substring(lastDot + 1).Replace("Workflow", "");
        }

        public static string SearchName_from_Use_label(string useLabel)
        {
            return useLabel.ToLower().Replace("workflow", "").Replace(" ", "");
        }

        internal static string SearchName_from_TypeName(string typeName)
        {
            var lastDot = typeName.LastIndexOf('.');
            return typeName.Substring(lastDot + 1).ToLower().Replace("workflow", "");
        }
    }
}
