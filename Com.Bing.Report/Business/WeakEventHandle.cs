using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Com.Bing.Business
{
    public class InnerException : ApplicationException
    {
        public InnerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class WeakEventHandler<TEventArgs> where TEventArgs : EventArgs
    {
        private WeakReference reference;
        public WeakReference Reference
        {
            get { return reference; }
            private set { reference = value; }
        }

        private MethodInfo method;
        public MethodInfo Method
        {
            get { return method; }
            private set { method = value; }
        }

        private EventHandler<TEventArgs> handle;
        public EventHandler<TEventArgs> Handler
        {
            get { return handle; }
            private set { handle = value; }
        }

        public WeakEventHandler(EventHandler<TEventArgs> eventHandler)
        {
            Reference = new WeakReference(eventHandler.Target);
            Method = eventHandler.Method;
            Handler = Invoke;
        }

        public void Invoke(object sender, TEventArgs e)
        {
            try
            {
                Method.Invoke(Reference.Target, new object[] { sender, e });
            }
            catch (Exception ex)
            {
                throw new InnerException(ex.GetBaseException().Message, ex.GetBaseException());
            }
        }

        public static implicit operator EventHandler<TEventArgs>(WeakEventHandler<TEventArgs> weakHandler)
        {
            return weakHandler.Handler;
        }
    }

    //dqj 与WeakEventHandler很相似，不过写成范形搞不定
	public class WeakNavBarLinkEventHandler
    {
        private WeakReference reference;
        public WeakReference Reference
        {
            get { return reference; }
            private set { reference = value; }
        }

        private MethodInfo method;
        public MethodInfo Method
        {
            get { return method; }
            private set { method = value; }
        }

        private DevExpress.XtraNavBar.NavBarLinkEventHandler handle;
        public DevExpress.XtraNavBar.NavBarLinkEventHandler Handler
        {
            get { return handle; }
            private set { handle = value; }
        }

        public WeakNavBarLinkEventHandler(DevExpress.XtraNavBar.NavBarLinkEventHandler eventHandler)
        {
            Reference = new WeakReference(eventHandler.Target);
            Method = eventHandler.Method;
            Handler = Invoke;
        }

        public void Invoke(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            try
            {
                Method.Invoke(Reference.Target, new object[] { sender, e });
            }
            catch (Exception ex)
            {
                throw new InnerException(ex.GetBaseException().Message, ex.GetBaseException());
            }
        }

        public static implicit operator DevExpress.XtraNavBar.NavBarLinkEventHandler(WeakNavBarLinkEventHandler weakHandler)
        {
            return weakHandler.Handler;
        }
    }

    public class WeakEventHandler
    {
        private WeakReference reference;
        public WeakReference Reference
        {
            get { return reference; }
            private set { reference = value; }
        }

        private MethodInfo method;
        public MethodInfo Method
        {
            get { return method; }
            private set { method = value; }
        }

        private EventHandler handle;
        public EventHandler Handler
        {
            get { return handle; }
            private set { handle = value; }
        }

        public WeakEventHandler(EventHandler eventHandler)
        {
            Reference = new WeakReference(eventHandler.Target);
            Method = eventHandler.Method;
            Handler = Invoke;
        }

        public void Invoke(object sender, EventArgs e)
        {
            try
            {
                Method.Invoke(Reference.Target, new object[] { sender, e });
            }
            catch (Exception ex)
            {
                throw new InnerException(ex.GetBaseException().Message, ex.GetBaseException());
            }
        }

        public static implicit operator EventHandler(WeakEventHandler weakHandler)
        {
            return weakHandler.Handler;
        }
    }
}