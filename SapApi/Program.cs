using SAP2000.services;
using System;
using System.Windows.Forms;

namespace SAP2000.WinForms
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            ISap2000ApiService sapApiService = new Sap2000ApiService();

            Application.Run(new Form1(sapApiService));
        }
    }
}