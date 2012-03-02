/*
 * This file is part of GeoTransformer project (http://geotransformer.codeplex.com/).
 * It is licensed under Microsoft Reciprocal License (Ms-RL).
 */
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace GeoTransformer
{
    /// <summary>
    /// Class containing extension methods for <see cref="Control"/> class.
    /// </summary>
    public static class ControlExtensions
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);
        
        #region [ Textbox extensions ]

        /// <summary>
        /// Sets the watermark text on a <see cref="TextBox"/> control.
        /// </summary>
        /// <param name="textBox">The text box.</param>
        /// <param name="watermarkText">The watermark text.</param>
        public static void SetWatermark(this TextBox textBox, string watermarkText)
        {
            uint ECM_FIRST = 0x1500;
            uint EM_SETCUEBANNER = ECM_FIRST + 1;
            SendMessage(textBox.Handle, EM_SETCUEBANNER, 0, watermarkText);
        }

        #endregion

        #region [ Invoke ]

        /// <summary>
        /// Checks if call to the control is made from another thread and if needed calls <see cref="Control.Invoke"/> method.
        /// </summary>
        /// <param name="control">The control on which the method is called.</param>
        /// <param name="action">The action delegate that has to be invoked.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="action"/> is <c>null</c></exception>
        public static void Invoke(this Control control, Action action)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            if (control.InvokeRequired)
                control.Invoke((Delegate)action);
            else
                action();
        }

        /// <summary>
        /// Checks if call to the control is made from another thread and if needed calls <see cref="Control.Invoke"/> method.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <param name="control">The control on which the method is called.</param>
        /// <param name="action">The action delegate that has to be invoked.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="action"/> is <c>null</c></exception>
        public static void Invoke<TControl>(this TControl control, Action<TControl> action)
            where TControl : Control
        {
            if (control == null)
                throw new ArgumentNullException("control");

            if (control.InvokeRequired)
                control.Invoke(action, control);
            else
                action(control);
        }

        /// <summary>
        /// Checks if call to the control is made from another thread and if needed calls <see cref="Control.Invoke"/> method.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <typeparam name="TArg">The type of the delegate argument.</typeparam>
        /// <param name="control">The control on which the method is called.</param>
        /// <param name="action">The action delegate that has to be invoked.</param>
        /// <param name="argument">The argument that will be passed to the delegate</param>
        /// <exception cref="ArgumentNullException">when <paramref name="action"/> is <c>null</c></exception>
        public static void Invoke<TControl, TArg>(this TControl control, Action<TControl, TArg> action, TArg argument)
            where TControl : Control
        {
            if (control == null)
                throw new ArgumentNullException("control");

            if (control.InvokeRequired)
                control.Invoke(action, control, argument);
            else
                action(control, argument);
        }

        /// <summary>
        /// Checks if call to the control is made from another thread and if needed calls <see cref="Control.Invoke"/> method.
        /// </summary>
        /// <param name="control">The control on which the method is called.</param>
        /// <param name="action">The action delegate that has to be invoked.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="action"/> is <c>null</c></exception>
        public static void BeginInvoke(this Control control, Action action)
        {
            if (control == null)
                throw new ArgumentNullException("control");

            if (control.InvokeRequired)
                control.BeginInvoke((Delegate)action);
            else
                action.BeginInvoke(null, null);
        }

        /// <summary>
        /// Checks if call to the control is made from another thread and if needed calls <see cref="Control.Invoke"/> method.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <param name="control">The control on which the method is called.</param>
        /// <param name="action">The action delegate that has to be invoked.</param>
        /// <exception cref="ArgumentNullException">when <paramref name="action"/> is <c>null</c></exception>
        public static void BeginInvoke<TControl>(this TControl control, Action<TControl> action)
            where TControl : Control
        {
            if (control == null)
                throw new ArgumentNullException("control");

            if (control.InvokeRequired)
                control.BeginInvoke(action, control);
            else
                action.BeginInvoke(control, null, null);
        }

        /// <summary>
        /// Checks if call to the control is made from another thread and if needed calls <see cref="Control.Invoke"/> method.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <typeparam name="TArg">The type of the delegate argument.</typeparam>
        /// <param name="control">The control on which the method is called.</param>
        /// <param name="action">The action delegate that has to be invoked.</param>
        /// <param name="argument">The argument that will be passed to the delegate</param>
        /// <exception cref="ArgumentNullException">when <paramref name="action"/> is <c>null</c></exception>
        public static void BeginInvoke<TControl, TArg>(this TControl control, Action<TControl, TArg> action, TArg argument)
            where TControl : Control
        {
            if (control == null)
                throw new ArgumentNullException("control");

            if (control.InvokeRequired)
                control.BeginInvoke(action, control, argument);
            else
                action.BeginInvoke(control, argument, null, null);
        }

        /// <summary>
        /// Checks if call to the control is made from another thread and if needed calls <see cref="Control.Invoke"/> method.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="control">The control on which the method is called.</param>
        /// <param name="func">The action delegate that has to be invoked.</param>
        /// <returns>The value as returned from the delegate.</returns>
        /// <exception cref="ArgumentNullException">when <paramref name="action"/> is <c>null</c></exception>
        public static TResult Invoke<TControl, TResult>(this TControl control, Func<TControl, TResult> func)
            where TControl : Control
        {
            if (control == null)
                throw new ArgumentNullException("control");

            if (control.InvokeRequired)
                return (TResult)control.Invoke(func, control);
            else
                return func(control);
        }

        /// <summary>
        /// Checks if call to the control is made from another thread and if needed calls <see cref="Control.Invoke"/> method.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <typeparam name="TArg">The type of the delegate argument.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="control">The control on which the method is called.</param>
        /// <param name="func">The action delegate that has to be invoked.</param>
        /// <param name="argument">The argument that will be passed to the delegate</param>
        /// <returns>The value as returned from the delegate.</returns>
        /// <exception cref="ArgumentNullException">when <paramref name="action"/> is <c>null</c></exception>
        public static TResult Invoke<TControl, TArg, TResult>(this TControl control, Func<TControl, TArg, TResult> func, TArg argument)
            where TControl : Control
        {
            if (control == null)
                throw new ArgumentNullException("control");

            if (control.InvokeRequired)
                return (TResult)control.Invoke(func, control, argument);
            else
                return func(control, argument);
        }

        /// <summary>
        /// Checks if call to the control is made from another thread and if needed calls <see cref="Control.Invoke"/> method.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <typeparam name="TArg">The type of the delegate argument.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="control">The control on which the method is called.</param>
        /// <param name="func">The action delegate that has to be invoked.</param>
        /// <param name="argument">The argument that will be passed to the delegate</param>
        /// <returns>The value as returned from the delegate.</returns>
        /// <exception cref="ArgumentNullException">when <paramref name="action"/> is <c>null</c></exception>
        public static TResult Invoke<TControl, TArg, TResult>(this TControl control, Func<TArg, TResult> func, TArg argument)
            where TControl : Control
        {
            if (control == null)
                throw new ArgumentNullException("control");

            if (control.InvokeRequired)
                return (TResult)control.Invoke(func, argument);
            else
                return func(argument);
        }

        /// <summary>
        /// Checks if call to the control is made from another thread and if needed calls <see cref="Control.Invoke"/> method.
        /// </summary>
        /// <typeparam name="TControl">The type of the control.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="control">The control on which the method is called.</param>
        /// <param name="func">The action delegate that has to be invoked.</param>
        /// <returns>The value as returned from the delegate.</returns>
        /// <exception cref="ArgumentNullException">when <paramref name="action"/> is <c>null</c></exception>
        public static TResult Invoke<TControl, TResult>(this TControl control, Func<TResult> func)
            where TControl : Control
        {
            if (control == null)
                throw new ArgumentNullException("control");

            if (control.InvokeRequired)
                return (TResult)control.Invoke(func);
            else
                return func();
        }

        #endregion
    }
}
