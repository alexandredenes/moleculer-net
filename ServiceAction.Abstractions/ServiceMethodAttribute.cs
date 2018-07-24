using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceAction.Abstractions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ServiceMethodAttribute : Attribute
    {
        private string _name;

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
