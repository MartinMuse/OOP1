using System;
using System.Reflection;
using Menu_Builder.MyClasses;

namespace Menu_Builder.Class_Managment
{
    class Class__Managment
    {
        Object[] objects;
        Type[] types;
        int edit_obj_num;
        int edit_prop_num;
        PropertyInfo[] edit_obj_props;

        public Class__Managment()
        {
            Array.Resize(ref objects, 0);
            Assembly assembly = Assembly.Load("Menu Builder");
            types = assembly.GetTypes();

        }
        public int GetEditObjNum()
        {
            return edit_obj_num;
        }

        public int GetObjNum()
        {
            return objects.Length; ;
        }

        public int GetEditPropNum()
        {
            return edit_prop_num;
        }

        public string[] GetClasses()
        {
            string[] result = new string[types.Length];
            int j = 0;
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i].IsClass && (!types[i].IsAbstract) && types[i].FullName.Contains("MyClasses"))
                    result[j++] = types[i].Name;
            }
            Array.Resize(ref result, j);
            return result;
        }  

        public object CreateObject(string obj_name)
        {
            int objs_len = objects.Length;
            Array.Resize(ref objects, objs_len + 1);
            int i = 0;
            while (obj_name != types[i].Name)
            {
                i++;
            }
            objects[objs_len] = Activator.CreateInstance(types[i]);
            return objects[objs_len];
        }

        public string GetName()
        {
            Type type = objects[edit_obj_num].GetType();
            PropertyInfo prop_info = type.GetProperty("name", BindingFlags.Instance
                    | BindingFlags.Static
                    | BindingFlags.Public
                    | BindingFlags.NonPublic);
            return (string)prop_info.GetValue(objects[edit_obj_num]);////??
        }

        public void SetActiveLastObj()
        {
            edit_obj_num = objects.Length - 1;
            edit_prop_num = 0;
            GetProps();
        }
        public void SetActiveObj(int index)
        {
            edit_obj_num = index;
            edit_prop_num = 0;
            GetProps();
        }

        public void SetActiveProp(int num)
        {
            edit_prop_num = num;
        }

        public bool IsPropClass()
        {
            string a = edit_obj_props[edit_prop_num].PropertyType.Name;
            return edit_obj_props[edit_prop_num].PropertyType.IsClass &&
                edit_obj_props[edit_prop_num].PropertyType.Name != "String";
        }
        public void GetProps()
        {
            Type type = objects[edit_obj_num].GetType();
            edit_obj_props=type.GetProperties(BindingFlags.Instance
                    | BindingFlags.Static
                    | BindingFlags.Public
                    | BindingFlags.NonPublic);
        }

        public string[] GetObjPropsTxt()
        {
            string[] result = new string[edit_obj_props.Length];
            for (int i = 0; i < edit_obj_props.Length; i++)
            {
                result[i] = edit_obj_props[i].Name + " :  ";
                if (edit_obj_props[i].GetValue(objects[edit_obj_num]) != null)
                    if (edit_obj_props[i].PropertyType.IsClass &&
                    edit_obj_props[i].PropertyType.Name != "String")
                    {
                        Type type = objects[edit_obj_num].GetType();
                        PropertyInfo prop_info = type.GetProperty("menu_item", BindingFlags.Instance
                                | BindingFlags.Static
                                | BindingFlags.Public
                                | BindingFlags.NonPublic);
                        object buf = prop_info.GetValue(objects[edit_obj_num]);
                        type = buf.GetType();
                        prop_info = type.GetProperty("name", BindingFlags.Instance
                                | BindingFlags.Static
                                | BindingFlags.Public
                                | BindingFlags.NonPublic);
                        result[i] += (string)prop_info.GetValue(buf);
                    }
                    else
                        result[i] += Convert.ToString(edit_obj_props[i].GetValue(objects[edit_obj_num])) + ";";
                else
                    result[i] += "none;";


            }
            return result;
        }

        private object[] GetChildObjects()
        {
            object[] result = new object[0];
            for (int i = 0; i < objects.Length; i++)
                if (objects[i].GetType().BaseType == edit_obj_props[edit_prop_num].PropertyType)
                {
                    Array.Resize(ref result, result.Length + 1);
                    result[result.Length - 1] = objects[i];
                }
            return result;
        }
        public string[] GetChildObjectsTxt()
        {
            object[] children = GetChildObjects();
            string[] result = new string[children.Length];
            for (int i = 0; i < result.Length; i++)
            {
                Type type = children[i].GetType();
                PropertyInfo prop_info = type.GetProperty("name", BindingFlags.Instance
                        | BindingFlags.Static
                        | BindingFlags.Public
                        | BindingFlags.NonPublic);
                result[i] = (string)prop_info.GetValue(children[i]);
            }
            return result;
        }
        public bool SetPropVal(string val, int ind)
        {
            if (edit_obj_props[edit_prop_num].PropertyType.Name == "Int32")
            {
                try
                {
                    if (Convert.ToInt32(val) >= 0)
                    {
                        edit_obj_props[edit_prop_num].SetValue(objects[edit_obj_num],
                            Convert.ToInt32(val));
                        return true;
                    }
                    else return false;
                }
                catch
                {
                    return false;
                }
            }
            if (edit_obj_props[edit_prop_num].PropertyType.Name == "String")
            {
                edit_obj_props[edit_prop_num].SetValue(objects[edit_obj_num], val);
                return true;
            }
            if (edit_obj_props[edit_prop_num].PropertyType.IsClass &&
                edit_obj_props[edit_prop_num].PropertyType.Name != "String")
            {
                object[] children = GetChildObjects();
                edit_obj_props[edit_prop_num].SetValue(objects[edit_obj_num], children[ind]);
                return true;
                //edit_obj_props[edit_prop_num].SetValue(objects[edit_obj_num], CreateObject(val));
            }
            return false;
        }

        public void DeleteObj()
        {
            PropertyInfo[] buf_props;
            Type type;
            for (int i = 0; i < objects.Length; i++)
            {
                type = objects[i].GetType();
                buf_props = type.GetProperties(BindingFlags.Instance
                        | BindingFlags.Static
                        | BindingFlags.Public
                        | BindingFlags.NonPublic);
                for (int j = 0; j < buf_props.Length; j++)
                    if (ReferenceEquals(buf_props[j].GetValue(objects[i]), objects[edit_obj_num]))
                    {
                        buf_props[j].SetValue(objects[i], null);
                    }
            }
            objects[edit_obj_num] = null;
            for (int i = edit_obj_num; i < objects.Length - 1; i++)
            {
                objects[i] = objects[i + 1];
            }
            Array.Resize(ref objects, objects.Length - 1);
            if (edit_obj_num > 0)
                edit_obj_num -= 1;
        }
    }
}
