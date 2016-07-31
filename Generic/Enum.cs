using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if !SILVERLIGHT
namespace Generic
#else
namespace ISIS.Client.Common
#endif
{
   /// <summary>
   /// By Niko For Message
   /// </summary>
   #region [Message Box]

   [System.Flags]
   public enum MsgBoxStyle : int
   {
      /// <summary>
      /// OK button only (default).
      /// </summary>
      OKOnly = 0,
      /// <summary>
      /// OK and Cancel buttons.
      /// </summary>
      OKCancel = 1,
      /// <summary>
      /// Abort, Retry, and Ignore buttons.
      /// </summary>
      AbortRetryIgnore = 2,
      /// <summary>
      /// Yes, No, and Cancel buttons.
      /// </summary>
      YesNoCancel = 3,
      /// <summary>
      /// Yes and No buttons.
      /// </summary>
      YesNo = 4,
      /// <summary>
      /// Retry and Cancel buttons.
      /// </summary>
      RetryCancel = 5,
      /// <summary>
      /// Critical message.
      /// </summary>
      Critical = 16,
      /// <summary>
      /// Warning query.
      /// </summary>
      Question = 32,
      /// <summary>
      /// Warning message.
      /// </summary>
      Exclamation = 48,
      /// <summary>
      /// Information message.
      /// </summary>
      Information = 64,
      /// <summary>
      /// First button is default (default).
      /// </summary>
      DefaultButton1 = 0,
      /// <summary>
      /// Second button is default.
      /// </summary>
      DefaultButton2 = 256,
      /// <summary>
      /// Third button is default.
      /// </summary>
      DefaultButton3 = 512,
      /// <summary>
      /// Application modal message box (default).
      /// </summary>
      ApplicationModal = 0,
      /// <summary>
      /// System modal message box.
      /// </summary>
      SystemModal = 4096,
      /// <summary>
      /// Help text.
      /// </summary>
      MsgBoxHelpButton = 16384,
      /// <summary>
      /// Foreground message box window.
      /// </summary>
      MsgBoxSetForeground = 65536,
      /// <summary>
      /// Right-aligned text.
      /// </summary>
      MsgBoxRight = 524288,
      /// <summary>
      /// Right-to-left reading text (Hebrew and Arabic systems).
      /// </summary>
      MsgBoxRtlReading = 1048576,
   }

   public enum MsgBoxResult
   {
      Abort,
      Cancel,
      Ignore,
      No,
      Ok,
      Retry,
      Yes,
   }

   #endregion
}
