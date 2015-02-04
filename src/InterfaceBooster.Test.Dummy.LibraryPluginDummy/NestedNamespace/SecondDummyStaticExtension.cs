using InterfaceBooster.LibraryPluginApi.Static;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.Test.Dummy.LibraryPluginDummy.NestedNamespace
{
    public class SecondDummyStaticExtension: IStaticExtension
    {
        public string Namespace { get { return null; } }

        [StaticVariable]
        public string StringValue { get; set; }

        [StaticVariable]
        public bool BoolValue { get; set; }

        [StaticVariable]
        public int IntValue { get; set; }

        [StaticVariable]
        public decimal DecimalValue { get; set; }

        [StaticVariable]
        public double DoubleValue { get; set; }

        [StaticVariable]
        public char CharValue { get; set; }

        [StaticVariable]
        public DateTime DateTimeValue { get; set; }

        public SecondDummyStaticExtension()
        {
            StringValue = "String Value";
            BoolValue = true;
            IntValue = 15;
            DecimalValue = 12.756M;
            DoubleValue = 12.5987654321;
            CharValue = 'R';
            DateTimeValue = new DateTime(1987, 12, 15, 22, 30, 0);
        }
    }
}
