using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderProcessor.Service.Config
{
    public static class OrderConstants
    {
        public static class Types
        {
            public const string Food = "Food";
            public const string Electronics = "Electronics";
            public const string Other = "Other";
        }

        public static class State
        {
            public const string NY = "NY";
            public const string CA = "CA";
            public const string Other = "Other";
        }

        public static class Region
        {
            public const string US = "US";
            public const string EU = "EU";
        }

        public static class ValidationMessages
        {
            public static readonly string[] RequestCannotBeNull = ["Request cannot be null"];
            public static readonly string[] InvalidJson = ["Invalid JSON"];
        }
    }
}

