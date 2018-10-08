// using Microsoft.SqlServer.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Xamla.Types.Records
{
    public class ChoiceOption
    {
        public int Value { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public bool Hidden { get; set; }
        public string Caption { get; set; }
        public string Description { get; set; }

        public IEditable ToEditable(IEditableFactory factory)
        {
            var option = (IEditableObject)factory.Create(BuiltInSchema.ChoiceOption);
            option.GetField("Value").Set(this.Value);
            option.GetField("Order").Set(this.Order);
            option.GetField("Hidden").Set(this.Hidden);
            option.GetField("Name").Set(this.Name);
            option.GetField("Caption").Set(this.Caption);
            option.GetField("Description").Set(this.Description);
            return option;
        }

        public static ChoiceOption FromCursor(ICursor cursor)
        {
            return new ChoiceOption()
            {
                Value = cursor.GoTo("Value").Get<int>(),
                Order = cursor.GoTo("Order").Get<int>(),
                Hidden = cursor.GoTo("Hidden").Get<bool>(),
                Name = cursor.GoTo("Name").Get<string>(),
                Caption = cursor.GoTo("Caption").Get<string>(),
                Description = cursor.GoTo("Description").Get<string>()
            };
        }

        public override bool Equals(object obj)
        {
            var other = obj as ChoiceOption;
            return other != null
                && this.Value == other.Value
                && this.Name == other.Name
                && this.Order == other.Order
                && this.Hidden == other.Hidden
                && this.Caption == other.Caption
                && this.Description == other.Description;
        }

        public override int GetHashCode()
        {
            return this.Value;
        }
    }

    public class ChoiceSet
        : Item
        , IEditableConvertible
    {
        public string Caption { get; set; }
        public string Description { get; set; }
        public List<int> Defaults { get; set; }
        public List<ChoiceOption> Options { get; set; }

        public ChoiceSet()
        {
            this.Defaults = new List<int>();
            this.Options = new List<ChoiceOption>();
        }

        public void AddOption(string name, int value, int order = 0, string caption = null, string description = null, bool hidden = false)
        {
            this.Options.Add(new ChoiceOption() { Name = name, Value = value, Order = order, Caption = caption, Description = description, Hidden = hidden });
        }

        public static ChoiceSet FromEnum(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException();
            if (!enumType.GetTypeInfo().IsEnum)
                throw new ArgumentException("Passed type is not an enumeration.", "enumType");

            var names = enumType.GetTypeInfo().GetEnumNames();
            var values = enumType.GetTypeInfo().GetEnumValues().Cast<int>();

            return new ChoiceSet()
            {
                Name = enumType.FullName,
                Options = names.Zip(values, (n, v) => new ChoiceOption { Name = n, Value = v }).ToList()
            };
        }

        public IEditableObject ToEditable(IEditableFactory factory)
        {
            var choiceSet = factory.CreateItem(Schema.BuiltIn[BuiltInSchema.ChoiceSet]);
            choiceSet.GetField((int)ItemLayout.Id).Set(this.Id);
            choiceSet.GetField((int)ItemLayout.Name).Set(this.Name);
            choiceSet.GetField((int)ItemLayout.Revision).Set(this.Revision);

            IEditableObject dataField = choiceSet.GetField((int)ItemLayout.Data).Get<IEditableObject>();
            dataField.GetField("Description").Set(this.Description);
            dataField.GetField("Caption").Set(this.Caption);

            var defaultsList = (IEditableList)dataField.GetField("Defaults");
            this.Defaults.ForEach(x => defaultsList.Add().Set(x));

            var optionList = (IEditableList)dataField.GetField("Options");
            this.Options.ForEach(x => optionList.Add().Set(x.ToEditable(factory)));

            return choiceSet;
        }

        IEditable IEditableConvertible.ToEditable(IEditableFactory factory)
        {
            return ToEditable(factory);
        }

        public static ChoiceSet FromCursor(ICursor cursor)
        {
            var dataField = cursor.GoTo((int)ItemLayout.Data, true);

            var result = new ChoiceSet();
            if (!cursor.GoTo((int)ItemLayout.Id).IsNull)
                result.Id = cursor.GoTo((int)ItemLayout.Id).Get<ItemId>();
            result.Name = cursor.GoTo((int)ItemLayout.Name).Get<string>();
            result.Revision = cursor.GoTo((int)ItemLayout.Revision).Get<int>();
            result.Description = dataField.GoTo("Description").Get<string>();
            result.Caption = dataField.GoTo("Caption").Get<string>();
            result.Defaults = dataField.GoTo("Defaults").Children.Select(x => x.Get<int>()).ToList();
            result.Options = dataField.GoTo("Options").Children.Select(ChoiceOption.FromCursor).ToList();
            return result;
        }
    }
}
