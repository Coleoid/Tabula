using System;

namespace Tabula
{
    public static class Formatter
    {
        public static string MethodName_to_SearchName(string methodName)
        {
            return methodName.Replace("_", "").ToLower();
        }

        public static string ClassName_to_InstanceName(string workflowName)
        {
            var lastDot = workflowName.LastIndexOf('.');
            return workflowName.Substring(lastDot + 1).Replace("Workflow", "");
        }

        public static string UseLabel_to_InstanceName(string useLabel)
        {
            return useLabel.ToLower().Replace("workflow", "").Replace(" ", "");
        }
    }
}
