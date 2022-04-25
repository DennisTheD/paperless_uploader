using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace PaperlessClient.Mobile.Behaviors
{
    public class QuitSearchCommandBehavior : BehaviorBase<SearchBar>
    {
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(QuitSearchCommandBehavior), null);
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        protected override void OnAttachedTo(SearchBar bindable)
        {
            base.OnAttachedTo(bindable);
            bindable.TextChanged += OnTextChanged;
        }        

        protected override void OnDetachingFrom(SearchBar bindable)
        {
            bindable.TextChanged -= OnTextChanged;
            base.OnDetachingFrom(bindable);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (e != null
                && string.IsNullOrEmpty(e.NewTextValue)) { 

                Command.Execute(sender);
            }
        }
    }
}
