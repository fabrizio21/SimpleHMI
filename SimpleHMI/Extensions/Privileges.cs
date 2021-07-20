using Prism;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SimpleHMI.Privileges
{
    public enum EnumPermission : short
    {
        ADMIN = 1,  //Administrator
        EDITOR = 2, //Editor
        USER = 99 //normal user
    }

    class Privilege
    {
        #region private methods

        private static void RecalculateControlVisibility(UIElement control, bool hasPermission)
        {
            if (hasPermission)
                control.Visibility = Visibility.Visible;
            else
                control.Visibility = Visibility.Collapsed;
        }

        private static void RecalculateControlIsEnabled(Control control, bool hasPermission)
        {
            control.IsEnabled = hasPermission;
        }

        #endregion

        #region Visibility

        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.RegisterAttached("Visibility", typeof(string), typeof(Privilege), new PropertyMetadata(Visibility_Callback));

        private static void Visibility_Callback(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            bool hasPermission = false;

            var uiElement = (UIElement)source;
            var permissions = GetVisibility(uiElement).Split(',');
            EnumPermission permission = new EnumPermission();

            //if using MVVM-light toolkit
            //if (ViewModelBase.IsInDesignModeStatic)
            //{
            //  hasPermission = true;
            //  goto END;
            //}

            //loop trough enumerator
            foreach (var fieldInfo in permission.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                //loop trough parameters
                foreach (var item in permissions.Where(m => m.ToUpper() == fieldInfo.Name.ToUpper()))
                {
                    permission = (EnumPermission)Enum.Parse(permission.GetType(), fieldInfo.Name, false);

                    //hasPermission = HasUserPermission(permission); //check if this permission is in users permission list
                    hasPermission = true; //TODO: place here your code to check permission of user
                    if (hasPermission) goto END;
                }
            }

            goto END;

        END:
            RecalculateControlVisibility(uiElement, hasPermission);
        }

        public static void SetVisibility(UIElement element, string value)
        {
            element.SetValue(VisibilityProperty, value);
        }

        public static string GetVisibility(UIElement element)
        {
            return (string)element.GetValue(VisibilityProperty);
        }

        #endregion

        #region IsEnabled

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached("IsEnabled", typeof(string), typeof(Privilege), new PropertyMetadata(IsEnabled_Callback));

        private static void IsEnabled_Callback(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            bool hasPermission = false;

            var uiElement = (Control)source;
            var permissions = GetIsEnabled(uiElement).Split(',');
            EnumPermission permission = new EnumPermission();

            //if using MVVM-light toolkit
            //if (ViewModelBase.IsInDesignModeStatic)
            //{
            //  hasPermission = true;
            //  goto END;
            //}

            //loop trough enumerator
            foreach (var fieldInfo in permission.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                //loop trough parameters
                foreach (var item in permissions.Where(m => m.ToUpper() == fieldInfo.Name.ToUpper()))
                {
                    permission = (EnumPermission)Enum.Parse(permission.GetType(), fieldInfo.Name, false);

                    Prism.Ioc.IContainerRegistry containerRegistry;

                    //hasPermission = HasUserPermission(permission); //check if this permission is in users permission list
                    hasPermission = true; //TODO: place here your code to check permission of user
                    if (hasPermission) goto END;
                }
            }

            goto END;

        END:
            RecalculateControlIsEnabled(uiElement, hasPermission);
        }

        public static void SetIsEnabled(UIElement element, string value)
        {
            element.SetValue(IsEnabledProperty, value);
        }

        public static string GetIsEnabled(UIElement element)
        {
            return (string)element.GetValue(IsEnabledProperty);
        }

        #endregion
    }
}
