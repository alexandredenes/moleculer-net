using System;

namespace ServiceAction.Abstractions
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceActionAttribute : Attribute
    {
        private string _name;

        public ServiceActionAttribute(string name = null)
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
