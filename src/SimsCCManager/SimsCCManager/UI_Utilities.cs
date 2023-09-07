using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace SimsCCManager.UI.Utilities
{
    public class DelegateCommand : ICommand  
    {  
        public delegate void SimpleEventHandler();  
        private SimpleEventHandler handler;  
        private bool isEnabled = true;  
 
        public event EventHandler CanExecuteChanged;  
 
        public DelegateCommand(SimpleEventHandler handler)  
        {  
            this.handler = handler;  
        }  
 
        private void OnCanExecuteChanged()  
        {  
            if (this.CanExecuteChanged != null)  
            {  
                this.CanExecuteChanged(this, EventArgs.Empty);  
            }  
        }  
 
        bool ICommand.CanExecute(object arg)  
        {  
            return this.IsEnabled;  
        }  
 
        void ICommand.Execute(object arg)  
        {  
            this.handler();  
        }  
 
        public bool IsEnabled  
        {  
            get { return this.isEnabled; }  
 
            set 
            {  
                this.isEnabled = value;  
                this.OnCanExecuteChanged();  
            }  
        }       
    }  
    
    public class RelayCommand : ICommand
    {
        #region Fields
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        #endregion

        #region Constructors
        public RelayCommand(Action<object> execute) : this(execute, null) { }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion

        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
        #endregion
    }

    public class MTObservableCollection<T> : ObservableCollection<T>
    {
        public override event NotifyCollectionChangedEventHandler CollectionChanged;
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            NotifyCollectionChangedEventHandler CollectionChanged = this.CollectionChanged;
            if (CollectionChanged != null)
                foreach (NotifyCollectionChangedEventHandler nh in CollectionChanged.GetInvocationList())
                {
                    DispatcherObject dispObj = nh.Target as DispatcherObject;
                    if (dispObj != null)
                    {
                        Dispatcher dispatcher = dispObj.Dispatcher;
                        if (dispatcher != null && !dispatcher.CheckAccess())
                        {
                            dispatcher.BeginInvoke(
                                (Action)(() => nh.Invoke(this,
                                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset))),
                                DispatcherPriority.DataBind);
                            continue;
                        }
                    }
                    nh.Invoke(this, e);
                }
        }
    }


}

namespace SimsCCManager.CustomUIElements {
    public class CustomDataGrid : DataGrid
    {
        public CustomDataGrid ()
        {
            this.SelectionChanged += CustomDataGrid_SelectionChanged;
        }

        void CustomDataGrid_SelectionChanged (object sender, SelectionChangedEventArgs e)
        {
            this.SelectedItemsList = this.SelectedItems;
        }
        #region SelectedItemsList

        public IList SelectedItemsList
        {
            get { return (IList)GetValue (SelectedItemsListProperty); }
            set { SetValue (SelectedItemsListProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemsListProperty =
                DependencyProperty.Register ("SelectedItemsList", typeof (IList), typeof (CustomDataGrid), new PropertyMetadata (null));

        #endregion
    }

    class StringMatchConverter : IMultiValueConverter
    {
        public object Convert(object [] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {

            if(values.Length < 2)
            {
                return false;
            }

            for (int i = 1; i < values.Length; i++)
            {
                string one = values[0] as string;
                string two = values[i] as string;
                    if (!one.Equals(two))
                    {
                        return false;
                    }
            }

            return true;

        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
    
}