using System;
using System.Collections;
using System.Diagnostics;
using System.Data;
namespace Insignia
{
   /// <summary>
   /// Simple data type, most commonly used in attaching a little extra information
   /// to a string you want to display in a control of some type. The nice things about
   /// this class is that it is simple and it doesn't require us to modify the control
   /// in any way. (We don't need to set the controls <c>DisplayMember</c> property).
   /// </summary>
   /// <author>Nathan Bullock</author>
   /// <created>13 August 2002</created>
   /// <modified>26 September 2002</modified>


   public class FG_Data : IComparable
   {
      #region Data Members
      /// <summary>
      ///  String displayed by the control.
      /// </summary>
      public string display;

      /// <summary> 
      /// A more verbose version of the <c>display</c> string.
      /// </summary>
      /// <remarks> 
      /// This data member is often used to store the string which would be displayed
      /// in a tooltip.
      /// </remarks>
      public string tooltip;

      /// <summary>
      /// An integer which identifies the <c>display</c> string.
      /// </summary>
      /// <remarks>
      /// This data member is often used to store the database ID associated with
      /// the display string. But more generally can store any identifier associated
      /// with the <c>display</c> string.
      /// </remarks>
      public int id;


      /// <summary>
      /// Indicate the status of this record, used when this represent a data row
      /// in the helper classes.
      /// </summary>
      /// <remarks>
      /// This data member is used to remember what kind of action has been taken
      /// with this record, then take different further action on it. 
      /// for example: if it is modified, you might want to update the data record 
      /// in the database, if it is deleted, you might want to delete it from the 
      /// database. and for new, you might want to insert this new data into the 
      /// database. 
      /// </remarks>
      //public RowStatus status = RowStatus.Unchanged;

      /// <summary>
      /// An object which can be used to store any miscellaneous data
      /// associated with the <c>display</c> string.
      /// </summary>
      public object data;

      /// <summary>
      /// Combo boxes, and possibly other controls, should be able to use this property
      /// to not allow a user to select this FG_Data item, but where it can still be
      /// displayed if the id is explicitly set to its value.
      /// </summary>
      public bool visible = true;

      public bool bdefault;

      public int additionID1;
      public int additionID2;

      public string tag;
      public string additiontag1;
      public string additiontag2;

      #endregion

      #region Constructors and Clone

      /// <summary>
      /// Empth FG_Data constructor.
      /// </summary>
      public FG_Data()
         : this("", -1)
      {
      }

      /// <summary>
      /// FG_Data constructor.
      /// </summary>
      /// <remarks>
      /// This constructor only requires you to assign one string to the FG_Data
      /// object, therefore both <c>display</c> and <c>tooltip</c> will have the
      /// same value.
      /// </remarks>
      /// <param name="display_string">value data member <c>display</c>
      /// and data member <c>tooltip</c>is set to.</param>
      /// <param name="identifier">value data member <c>id</c> is set to.</param>
      /// 
      public FG_Data(string display_string, int identifier)
         : this(display_string, identifier, false)
      {
      }

      public FG_Data(string display_string, int identifier, string ttag)
         : this(display_string, identifier, false)
      {
         tag = ttag;
      }


      public FG_Data(string display_string, int identifier, bool isdefault)
         :
         this(display_string, display_string, identifier, isdefault, null)
      {
      }


      public FG_Data(string display_string, int identifier, bool isdefault, object _data)
         :
         this(display_string, display_string, identifier, isdefault, _data)
      {
      }


      public FG_Data(string display_string, string tooltip_string, int identifier, bool isdefault)
         :
         this(display_string, tooltip_string, identifier, isdefault, null)
      {
      }

      public FG_Data(string display_string, string tooltip_string, int identifier, bool isdefault, object _data)
      {
         display = display_string;
         tooltip = tooltip_string;
         id = identifier;
         bdefault = isdefault;
         data = _data;

      }

      /// <summary>
      /// FG_Data constructor.
      /// </summary>
      /// <remarks>
      /// This constructor initializes all of the objects data members uniquely.
      /// </remarks>
      /// <param name="display_string">value data member <c>display</c> is set to.</param>
      /// <param name="tooltip_string">value data member <c>tooltip</c> is set to.</param>
      /// <param name="identifier">value data member <c>id</c> is set to.</param>
      /// <param name="row_status">initial status of this record.</param>
      /// 
      //public FG_Data(string display_string, string tooltip_string, int identifier, RowStatus row_status, bool isdefault, object _data)
      //{
      //   display = display_string;
      //   tooltip = tooltip_string;
      //   id = identifier;
      //   bdefault = isdefault;
      //   data = _data;
      //   status = row_status;
      //}
      #endregion

      #region Conversion to string using casting or ToString
      /// <summary>
      /// Returns the <c>display</c> string.
      /// </summary>
      /// <remarks>
      /// This is the method which allows us not to have to tell each control
      /// which data member we want displayed, since it will call this method
      /// by default.
      /// </remarks>
      /// <returns>
      /// The <c>display</c> string.
      /// </returns>
      public override string ToString()
      {
         return display;
      }

      /// <summary>
      /// Makes implicit casting to <c>string</c> consistent
      /// with <c>ToString()</c> method.
      /// </summary>
      /// <remarks>
      /// This is only defined for consistency, not normally used.
      /// </remarks>
      /// <returns>
      /// The <c>display</c> string.
      /// </returns>
      public static implicit operator string(FG_Data data)
      {
         return data.display;
      }

      #endregion

      #region Find, etc
      static public FG_Data findDefaultIndex(FG_Data[] data)
      {
         if (data == null || data.Length == 0) return null;
         foreach (FG_Data tmp in data) if (tmp.bdefault == true) return tmp;
         return data[0];
      }

      static public int findDefaultID(FG_Data[] data)
      {
         if (data == null) return -1;
         foreach (FG_Data tmp in data) if (tmp.bdefault == true) return tmp.id;
         //set default to 'Unknow' generally if no default record has been set.
         return 0;
      }

      /// <summary>
      /// Finds the index of the <c>FG_Data</c> object whos <c>id</c> is equal
      /// to a given value.
      /// </summary>
      /// <static/>
      /// <author>Nathan Bullock</author>
      /// <param name="data">The FG_Data array to search through.</param>
      /// <param name="identifier">The id value of the FG_Data item you wish to find.</param>
      /// <returns>The index of the first FG_Data object with the given <c>id</c> value, otherwise -1.</returns>
      static public int getIndex(FG_Data[] data, int identifier)
      {
         for (int i = 0; i < data.Length; i++)
            if (data[i].id == identifier) return i;
         return -1;
      }

      static public int getIndex(FG_Data[] data, string display)
      {
         for (int i = 0; i < data.Length; i++)
            if (data[i].display == display) return i;
         return -1;
      }

      /// <summary>
      /// Finds the <c>FG_Data</c> object whos <c>id</c> is equal
      /// to a given value.
      /// </summary>
      /// <static/>
      /// <author>Nathan Bullock</author>
      /// <param name="data">The FG_Data array to search through.</param>
      /// <param name="identifier">The id value of the FG_Data item you wish to find.</param>
      /// <returns>The first FG_Data object with the given <c>id</c> value, otherwise null.</returns>
      static public FG_Data getItem(FG_Data[] data, int identifier)
      {
         if (data == null) return null;
         foreach (FG_Data tmp in data) if (tmp.id == identifier) return tmp;
         return null;
      }
      #endregion

      #region Remove and Add Items from FG_Data[]
      static public FG_Data[] removeItem(FG_Data[] data, int identifier)
      {
         if (data.Length == 0) return data;

         int index = 0;
         for (index = 0; index < data.Length; index++)
            if (data[index].id == identifier) break;
         for (int i = index; i < data.Length - 1; i++)
            data[i] = data[i + 1];
         data[data.Length - 1] = null;

         FG_Data[] fg_temp = new FG_Data[data.Length - 1];
         for (int j = 0; j < data.Length - 1; j++)
            fg_temp[j] = data[j];

         return fg_temp;
      }

      static public FG_Data[] InsertItem(FG_Data[] data, FG_Data item)
      {
         FG_Data[] fg_temp = null;
         if (data == null && item != null)
         {
            fg_temp = new FG_Data[1];
            fg_temp[0] = item;
            return fg_temp;
         }
         else if (data != null && item == null)
            return data;
         else if (data == null && item == null)
            return null;
         else
         {
            fg_temp = new FG_Data[data.Length + 1];

            for (int i = 0; i < data.Length; i++)
               fg_temp[i+1] = data[i];

            fg_temp[0] = item;
            return fg_temp;
         }
      }
      static public FG_Data[] AddItem(FG_Data[] data, FG_Data item)
      {
         FG_Data[] fg_temp = null;
         if (data == null && item != null)
         {
            fg_temp = new FG_Data[1];
            fg_temp[0] = item;
            return fg_temp;
         }
         else if (data != null && item == null)
            return data;
         else if (data == null && item == null)
            return null;
         else
         {
            fg_temp = new FG_Data[data.Length + 1];

            for (int i = 0; i < data.Length; i++)
               fg_temp[i] = data[i];

            fg_temp[data.Length] = item;
            return fg_temp;
         }
      }

      static public FG_Data[] AddItems(FG_Data[] data, FG_Data[] items)
      {
         if (data == null && items != null)
            return items;
         else if (data != null && items == null)
            return data;
         else if (data == null && items == null)
            return null;

         FG_Data[] fg_temp = new FG_Data[data.Length + items.Length];
         for (int i = 0; i < data.Length; i++)
            fg_temp[i] = data[i];
         for (int i = data.Length; i < data.Length + items.Length; i++)
            fg_temp[i] = items[i - data.Length];

         return fg_temp;
      }
      #endregion

      #region CompareTo
      public int CompareTo(object o)
      {
         return display.CompareTo(o.ToString());
      }
      #endregion

      #region Combine Teacher FG_Data elements to one
      public void Combine(FG_Data d)
      {
         display += "; " + d.display;
         tooltip += "\n" + d.tooltip;
      }

      static public FG_Data Combine(ArrayList al)
      {
         FG_Data tmp = null;
         foreach (FG_Data d in al)
            if (tmp == null) tmp = new FG_Data(d.display, d.tooltip, d.id, true);
            else tmp.Combine(d);
         if (tmp == null) return new FG_Data("", 0);
         return tmp;
      }
      #endregion

      #region SortByID
      public static FG_Data[] SortById(FG_Data[] data)
      {
         Array.Sort(data, new CompareByID());
         return data;
      }

      public static FG_Data[] SortByDisplay(FG_Data[] data)
      {
         Array.Sort(data, new CompareByValue());
         return data;
      }

      #endregion

   }


   #region FG_Data Comparison Classes
   public class CompareByValue : IComparer
   {
      public CompareByValue() { }

      public int Compare(object obj1, object obj2)
      {
         return ((FG_Data)obj1).display.CompareTo(((FG_Data)obj2).display);
      }
   }

   public class CompareByID : IComparer
   {
      public CompareByID()
      {
      }

      public int Compare(object obj1, object obj2)
      {
         if (((FG_Data)obj1).id < ((FG_Data)obj2).id) return -1;
         if (((FG_Data)obj1).id > ((FG_Data)obj2).id) return 1;
         return 0;
      }
   }
   #endregion

   #region Search Data Class
   public class Search_Data
   {
      public enum FieldType
      {
         Money,
         String,
         DateTime,
         ID
      }
      public string display;
      public string tooltip;
      public int id;
      public int defaultID = -1;			// zhz
      public object data;
      public bool bdefault;
      public int[] matchpattern;
      public int screencontroltype; //addintionID1
      public int additionID2;
      public string tablename;
      public string colid;
      public string colname;
      public FieldType fieldType;

      public string criteria;
      public Search_Data(string display_string, string search_string, int identifier,
         int[] match, int controltype, string table_name,
         string col_id, string col_name, string table_criteria, int defID)
         : this(display_string, search_string, identifier,
         match, controltype, table_name, col_id, col_name, table_criteria)
      {
         defaultID = defID;
      }


      public Search_Data(string display_string, string search_string, int identifier,
                          int[] match, int controltype, string table_name,
                          string col_id, string col_name, string table_criteria)
      {
         display = tooltip = display_string;
         if (search_string != "") tooltip = search_string;
         id = identifier;
         matchpattern = match;
         screencontroltype = controltype;
         bdefault = false;
         tablename = table_name;
         colid = col_id;
         colname = col_name;
         criteria = table_criteria;
         defaultID = -1;
      }

      public Search_Data(string display_string, string search_string, int identifier,
         int[] match, int controltype, string table_name,
         string col_id, string col_name)
         : this(display_string, search_string, identifier,
         match, controltype, table_name, col_id, col_name, "")
      {
      }
      //TextBox control constructor1
      public Search_Data(string display_string, string search_string, int identifier, int[] match, int controltype)
         : this(display_string, search_string, identifier, match, controltype, "", "", "", "")
      {
      }

      //TextBox control constructor2
      public Search_Data(string display_string, string search_string, int identifier, int[] match, int controltype, FieldType ft)
         : this(display_string, search_string, identifier, match, controltype)
      {
         fieldType = ft;
      }

      public Search_Data(string display_string, string search_string, int identifier, int[] match, int controltype, FieldType ft, int defID)
         : this(display_string, search_string, identifier, match, controltype, ft)
      {
         defaultID = defID;
      }

   }
   #endregion
}
