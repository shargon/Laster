using Laster.Core.Designer;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;

namespace Laster.Core.Classes.Collections
{
    public class DataVariableCollection : List<Variable>
    {
        public class DesignClassLink
        {
            public DataVariableCollection Parent;
            public override string ToString() { return Parent.Count.ToString(); }
        }
        public class DesignClass
        {
            [Editor(typeof(DataVariableCollectionEditor), typeof(UITypeEditor))]
            public DesignClassLink Variables { get; set; }

            //public VariableEdit() {  }
            public DesignClass(DataVariableCollection dataVariableCollection)
            {
                Variables = new DesignClassLink()
                {
                    Parent = dataVariableCollection,
                };
            }
        }
        public DesignClass Designer { get { return new DesignClass(this); } }

        public void Add(string obj, string property, string value) { Add(new Variable(obj, property, value)); }
    }
}