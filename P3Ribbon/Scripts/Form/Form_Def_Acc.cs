using System;
using System.Windows;
using System.Globalization;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.Creation;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Windows.Forms;

namespace P3Ribbon.Scripts
{
    public partial class Form_Def_Acc : System.Windows.Forms.Form
    {
        public static int classe_form = 666;
        public bool eng_form = false;
        public int vita_form;
        public static int zona_form = 666;
        public static bool ok_premuto = false;


        // fa schifo, forse era meglio mettere gli if dentro a Colora_Bottoni_Se_Parametri_Compilati
        private Button TrovaBottoneClasse(int c)
        {
            System.Windows.Forms.Button rtn = new Button();
            if (c == 1) rtn = c_1;
            else if (c == 2) rtn = c_2;
            else if (c == 3) rtn = c_3;
            else if (c == 4) rtn = c_4;

            return rtn;
        }
        private Button TrovaBottoneZona(int z)
        {
            System.Windows.Forms.Button rtn = new Button();
            if (z == 1) rtn = z1;
            else if (z == 2) rtn = z2;
            else if (z == 3) rtn = z3;
            else if (z == 4) rtn = z4;

            return rtn;
        }
        private void Colora_Bottoni_Se_Parametri_Compilati()
        {
            if (classe_form != 666)
            {
                Button bc = TrovaBottoneClasse(classe_form);
                Colora_bottone_classe(bc, classe_form);
            }
            if (zona_form != 666)
            {
                Button bz = TrovaBottoneZona(zona_form);
                Colora_bottone_zona(bz, zona_form);
            }
        }

        public Form_Def_Acc()
        {

            InitializeComponent();
            Colora_Bottoni_Se_Parametri_Compilati();
        }


        private void b_ok_Click(object sender, EventArgs e)
        {
            ok_premuto = true;

            Par_Sismici.classe = classe_form;
            Par_Sismici.eng = eng_form;
            Par_Sismici.vita = vita_form;
            Par_Sismici.zona = zona_form;

            this.Close();
        }


        public void AccToClasseUso()
        {
            string str = float_acc.Text.Replace(".", ",");
            //var _acc = Convert.ToDecimal(new CultureInfo("en-US"));
            //Console.WriteLine(acc.ToString(CultureInfo.CreateSpecificCulture("it-it")));
            double acc;
            bool coversioneriusicta = double.TryParse(str, out acc);
            if (coversioneriusicta)
            {
                if (acc > 0.35)
                {
                    Colora_bottone_zona(z1, 1);
                    float_acc.BackColor = System.Drawing.Color.Red;
                }
                else if (acc > 0.25 && acc <= 0.35)
                {
                    Colora_bottone_zona(z1, 1);
                }
                else if ((acc > 0.15 && acc <= 0.25))
                {
                    Colora_bottone_zona(z2, 2);
                }
                else if ((acc > 0.05 && acc <= 0.15))
                {
                    Colora_bottone_zona(z3, 3);
                }
                else if ((acc < 0.05))
                {
                    Colora_bottone_zona(z4, 4);
                }
            }
            else
            {
                float_acc.BackColor = System.Drawing.Color.Red;
            }
        }


        private void b_annulla_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void float_acc_TextChanged(object sender, EventArgs e)
        {
            AccToClasseUso();
        }

        private void Colora_bottone_zona(System.Windows.Forms.Button bottone, int z)
        {
            z1.BackColor = System.Drawing.Color.LightSteelBlue;
            z2.BackColor = System.Drawing.Color.LightSteelBlue;
            z3.BackColor = System.Drawing.Color.LightSteelBlue;
            z4.BackColor = System.Drawing.Color.LightSteelBlue;
            float_acc.BackColor = System.Drawing.Color.White;

            bottone.BackColor = System.Drawing.Color.SteelBlue;
            zona_form = z;

        }

        private void Colora_bottone_classe(System.Windows.Forms.Button bottone, int c)
        {
            c_1.BackColor = System.Drawing.Color.LightSteelBlue;
            c_2.BackColor = System.Drawing.Color.LightSteelBlue;
            c_3.BackColor = System.Drawing.Color.LightSteelBlue;
            c_4.BackColor = System.Drawing.Color.LightSteelBlue;

            bottone.BackColor = System.Drawing.Color.SteelBlue;
            classe_form = c;
        }

        private void z1_Click(object sender, EventArgs e)
        {

            Colora_bottone_zona(z1, 1);

        }

        private void z2_Click(object sender, EventArgs e)
        {

            Colora_bottone_zona(z2, 2);
        }

        private void z3_Click(object sender, EventArgs e)
        {

            Colora_bottone_zona(z3, 3);
        }

        private void z4_Click(object sender, EventArgs e) { Colora_bottone_zona(z4, 4); }
        private void c_1_Click(object sender, EventArgs e) { Colora_bottone_classe(c_1, 1); }
        private void c_2_Click(object sender, EventArgs e) { Colora_bottone_classe(c_2, 2); }
        private void c_3_Click(object sender, EventArgs e) { Colora_bottone_classe(c_3, 3); }
        private void c_4_Click(object sender, EventArgs e) { Colora_bottone_classe(c_4, 4); }


    }
}
