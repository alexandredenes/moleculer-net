using System;

namespace ServiceAction.Abstractions
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ServiceMethodAttribute : Attribute
    {
        private string _name;

        public ServiceMethodAttribute(string name = null)
        {
            Name = name;
        }

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}
