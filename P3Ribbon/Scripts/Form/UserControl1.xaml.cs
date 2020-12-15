using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace P3Ribbon.Scripts.Form
{
    /// <summary>
    /// Logica di interazione per UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : Window, IDisposable
    {
        public UserControl1()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var t = new Transaction(Supporto.doc, "Carica libreria"))
            {
                t.Start();
                TrasferisciStandard.TrasferisciTipiDoc(Supporto.app, Supporto.doc);
                //Materiale.PreAggiorna(m_doc); //lo faccio gia andare in form_libreriac_combobox_aggiorna:  
                //Form_Libreria_Combobox_Aggiorna();
                //TEMP DA SISTEARE CON BOOLEANI
                try
                {
                    App.comboMat.AddItems(Materiale.comboBoxMemberDatas);
                }
                catch
                { }
                // this.Close();
                //}
                Supporto.doc.Regenerate();
                t.Commit();
            }




        }
    }
}
