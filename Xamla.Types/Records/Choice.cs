using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamla.Types.Records
{
    public sealed class Choice
    {
        Schema schema;
        bool nullable;
        ChoiceSet choiceSet;
        int? value;

        public Choice(Schema schema)
            : this(schema, false)
        {
        }

        public Choice(Schema schema, bool nullable)
        {
            if (schema == null)
                throw new ArgumentNullException();

            if (schema.DataType != DataType.Choice)
                throw new ArgumentException();

            this.schema = schema;
            this.nullable = nullable;
            if (!nullable)
            {
                this.value = 0;
            }
        }

        public Choice(Schema schema, int? value, bool nullable = false)
            : this(schema, nullable)
        {
            this.Value = value;
        }

        public ChoiceSet ChoiceSet
        {
            get
            {
                if (choiceSet == null)
                    choiceSet = ChoiceSet.FromCursor(schema.DeclarationItem);

                return choiceSet;
            }
        }

        public bool Nullable
        {
            get { return nullable; }
        }

        public int? Value
        {
            get { return this.value; }
            set
            {
                if (value == null && !nullable)
                    throw new Exception("Choice field is not nullable");
                this.value = value;
            }
        }

        public void Select(string name)
        {
            this.Value = (name != null) ? (int?)this.ChoiceSet.Options.First(x => x.Name == name).Value : null;
        }

        public ChoiceOption ActiveOption
        {
            get { return this.ChoiceSet.Options.FirstOrDefault(x => x.Value == this.Value); }
        }

        public override string ToString()
        {
            var activeOption = this.ActiveOption;
            return string.Format("({0}) {1} ({2})", this.ChoiceSet.Name, (activeOption != null) ? activeOption.Name : "invalid", this.Value);
        }

        public IEditable ToEditable(IEditableFactory factory)
        {
            var choice = factory.Create(schema, nullable);
            choice.Set(this.Value);
            return choice;
        }
    }
}
