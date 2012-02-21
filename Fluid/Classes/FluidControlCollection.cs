using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Fluid.Controls
{
    // TODO: Make System.ComponentModel.Generic.BindingList derived!!
    public class FluidControlCollection : IList<FluidControl>
    {
        public FluidControlCollection(IContainer container)
            : base()
        {
            this.container = container;
        }

        private IContainer container;

        private List<FluidControl> controls = new List<FluidControl>();

        /// <summary>
        /// Occurs when the collection has changed.
        /// </summary>
        protected virtual void OnNotifyChanged()
        {
            if (NotifyChanged != null) NotifyChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Notify that something has happened to the collection or it's ITouchControl properties.
        /// </summary>
        public void NotifyChange()
        {
            OnNotifyChanged();
        }

        /// <summary>
        /// Occurs when the collection has changed.
        /// </summary>
        public event EventHandler NotifyChanged;

        #region ICollection<ISmartControl> Members

        public void Add(FluidControl item)
        {
            controls.Add(item);
            item.Container = container;
            OnControlAdded(item);
            OnNotifyChanged();
        }

        public void Insert(int index, FluidControl control)
        {
            controls.Insert(index, control);
            control.Container = container;
            OnControlAdded(control);
            OnNotifyChanged();
        }

        public event EventHandler<ControlEventArgs> ControlAdded;


        private void OnControlAdded(FluidControl control)
        {
            Size size = container.Bounds.Size;
            control.Container = container;
            control.Scale(container.ScaleFactor);
            control.OnControlAdded(container);
            INotifyControlAdded notify = container as INotifyControlAdded;
            if (notify != null) notify.ControlAdded(control);
            if (ControlAdded != null)
            {
                ControlEventArgs e = new ControlEventArgs(control);
                ControlAdded(this, e);
            }
        }

        private void OnControlRemoved(FluidControl control)
        {
            SizeF f = container.ScaleFactor;
            SizeF scale = new SizeF(1f / f.Width, 1f / f.Height);
            control.Container = null;
            control.Scale(scale);
        }

        private void RemoveControls()
        {
            SizeF f = container.ScaleFactor;
            SizeF scale = new SizeF(1f / f.Width, 1f / f.Height);
            foreach (FluidControl c in this)
            {
                c.Scale(scale);
                c.Container = null;
            }
        }

        public void Clear()
        {
            RemoveControls();
            controls.Clear();
            OnNotifyChanged();
        }

        public bool Contains(FluidControl item)
        {
            return controls.Contains(item);
        }

        public void CopyTo(FluidControl[] array, int arrayIndex)
        {
            controls.CopyTo(array, arrayIndex);
            OnNotifyChanged();
        }

        public int Count
        {
            get { return controls.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(FluidControl control)
        {
            OnControlRemoved(control);
            bool result = controls.Remove(control);
            if (result) OnNotifyChanged();
            return result;
        }

        #endregion

        #region IEnumerable<ISmartControl> Members

        public IEnumerator<FluidControl> GetEnumerator()
        {
            return controls.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return controls.GetEnumerator();
        }

        #endregion

        internal int IndexOf(FluidControl control)
        {
            return controls.IndexOf(control);
        }

        #region IList<ISmartControl> Members

        int IList<FluidControl>.IndexOf(FluidControl item)
        {
            return controls.IndexOf(item);
        }



        public void RemoveAt(int index)
        {
            FluidControl c = this[index];
            OnControlRemoved(c);
            controls.RemoveAt(index);
        }

        public FluidControl this[int index]
        {
            get { return controls[index]; }
            set { controls[index] = value; }
        }

        #endregion
    }
}
