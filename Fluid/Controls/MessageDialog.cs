using System;

using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Fluid.Classes;

namespace Fluid.Controls
{
    /// <summary>
    /// An ok-cancel dialog.
    /// When one of the buttons is pressed, the dialog closes and raises the Result event.
    /// </summary>
    /// <example>
    /// You can create a MessageDialog, open it modal and decide what to do on the Result event:
    /// MessageDialog dlg = new MessageDlg():
    /// 
    /// dlg.Text = "Hello";
    /// dlg.Result += new EventHandler<DialogEventArgs>(dialog_Result);
    /// dlg.ShowModal(ShowAnimation.FromBottom);
    /// </example>
    public class MessageDialog : FluidPanel
    {
        protected override void InitControl()
        {
            base.InitControl();
            Bounds = new Rectangle(0, 0, 240, 100);
            BackColor = Color.Black;
            ForeColor = Color.White;
            GradientFill = true;
            GradientFillOffset = 24;
            EnableDoubleBuffer = true;
            FluidButton okBtn = new FluidButton("OK", 8, 68, 100, 24);
            okBtn.ForeColor = Color.White;
            okBtn.Anchor = AnchorBL;
            this.okBtn = okBtn;
            okBtn.BackColor = Color.Green;
            okBtn.Click += new EventHandler(okBtn_Click);

            FluidButton cancelBtn = new FluidButton("Cancel", 240 - 100 - 8, 68, 100, 24);
            this.cancelBtn = cancelBtn;
            cancelBtn.ForeColor = Color.White;
            cancelBtn.Anchor = AnchorBR;
            cancelBtn.BackColor = Color.Red;
            cancelBtn.Click += new EventHandler(cancelBtn_Click);
            Visible = false;

            FluidLabel textLabel = new FluidLabel("", 4, 12, Width - 8, 68 - 12 - 4);
            textLabel.Format.FormatFlags = 0;
            textLabel.Anchor = AnchorAll;
            textLabel.LineAlignment = StringAlignment.Center;
            textLabel.Alignment = StringAlignment.Center;
            this.textLabel = textLabel;

            Controls.Add(textLabel);
            Controls.Add(okBtn);
            Controls.Add(cancelBtn);
        }

        FluidLabel textLabel;
        FluidButton okBtn;
        FluidButton cancelBtn;

        /// <summary>
        /// Gets or sets the Alignment for the dialog text.
        /// </summary>
        public StringAlignment Alignment
        {
            get { return textLabel.Alignment; }
            set { textLabel.Alignment = value; }
        }

        /// <summary>
        /// Gets or sets the Line Alignment for the dialog text.
        /// </summary>
        public StringAlignment LineAlignment
        {
            get { return textLabel.LineAlignment; }
            set { textLabel.LineAlignment = value; }
        }

        /// <summary>
        /// Gets or sets the text for the dialog.
        /// </summary>
        public string Message
        {
            get { return textLabel.Text; }
            set
            {
                textLabel.Text = value;
                if (value != null && value.IndexOf('\n') >= 0)
                {
                    textLabel.LineAlignment = StringAlignment.Near;
                }
                else
                {
                    textLabel.LineAlignment = StringAlignment.Center;
                }
            }
        }

        /// <summary>
        /// Gets or sets the text for the OK button.
        /// </summary>
        public string OkText
        {
            get { return okBtn.Text; }
            set
            {
                if (value != null)
                {
                    if (value == string.Empty) value = "OK";
                    okBtn.Text = value;
                    okBtn.Visible = true;
                }
                else
                {
                    okBtn.Visible = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the text for the Cancel button.
        /// </summary>
        public string CancelText
        {
            get { return cancelBtn.Text; }
            set
            {
                if (value != null)
                {
                    if (value == string.Empty) value = "Cancel";
                    cancelBtn.Text = value;
                    cancelBtn.Visible = true;
                }
                else
                {
                    cancelBtn.Visible = false;
                }
            }
        }

        void okBtn_Click(object sender, EventArgs e)
        {
            Close(ShowTransition.FromBottom);
            OnResult(DialogResult.OK);
        }

        void cancelBtn_Click(object sender, EventArgs e)
        {
            Close(ShowTransition.FromBottom);
            OnResult(DialogResult.Cancel);
        }

        protected virtual void OnResult(DialogResult result)
        {
            if (Result != null)
            {
                dialogEventArgs.Result = result;
                Result(this, dialogEventArgs);
            }
        }



        /// <summary>
        /// Occurs when the either the ok or cancel button was pressed and the dialog is closed.
        /// </summary>
        private DialogEventArgs dialogEventArgs = new DialogEventArgs();

        /// <summary>
        /// Occurs when the either the ok or cancel button was pressed and the dialog is closed.
        /// </summary>
        public event EventHandler<DialogEventArgs> Result;

        public static void Show(string message, string okText, string cancelText)
        {
            Show(message, okText, cancelText, Color.Empty);
        }

        public static void Show(string message, string okText, string cancelText, Color backColor)
        {
            MessageDialog dialog = new MessageDialog();
            if (!backColor.IsEmpty) dialog.BackColor = backColor;
            dialog.Message = message;
            dialog.OkText = okText;
            dialog.CancelText = cancelText;
            dialog.ShowModal(ShowTransition.FromBottom);
        }

        public override void Show(ShowTransition transition)
        {
            CenterButtonIfSingle();
            base.Show(transition);
        }

        private void CenterButtonIfSingle()
        {
            if (okBtn.Visible || cancelBtn.Visible)
            {
                if (cancelBtn.Visible == false)
                {
                    okBtn.Left = (Width - okBtn.Width) / 2;
                }
                else
                {
                    okBtn.Left = ScaleX(8);
                }
                if (okBtn.Visible == false)
                {
                    cancelBtn.Left = (Width - cancelBtn.Width) / 2;
                }
                else
                {
                    cancelBtn.Left = Width - cancelBtn.Width - ScaleX(8);
                }
            }
        }

    }
}
