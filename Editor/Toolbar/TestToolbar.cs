using UnityEngine;

namespace GameKit.Toolbar.Test
{
    [ToolbarMethodRepository("Example Class")]
    public class TestToolbarClass
    {
        private static TestToolbarClass instance;

        [ToolbarMethodData("One", "TestBoolReverse")]
        private static void Init()
        {
            instance = new TestToolbarClass();
            //  CollectInfoForToolbar.AuthorizeInstance(instance);
        }

        [ToolbarMethodData("One", "TestBool")]
        private void FirstSecond()
        {
            Debug.Log("Invoked");
        }

        [ToolbarMethodData("One", "TestBool")]
        private static void FirstThird(int arg, float x, string test, bool testbool)
        {
            Debug.LogFormat("{0} , {1} , {2} , {3}", arg, x, test, testbool);
        }

        [ToolbarMethodData("Third Method (Static)", "One")]
        private static void Second(bool testbool)
        {
            Debug.LogFormat("{0} , {1}", testbool, "test");
        }

        [ToolbarMethodData("123", "TestBool")]
        public void Third(Object uobject)
        {
            Debug.Log(uobject.name);
        }

        [ToolbarMethodData("Last", "TestBool")]
        public static void Four()
        {
        }

        private static bool TestBool()
        {
            return instance != null;
        }

        private static bool TestBoolReverse()
        {
            return instance == null;
        }
    }
}