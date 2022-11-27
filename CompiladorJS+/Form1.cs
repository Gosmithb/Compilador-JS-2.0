using Compilador;
using System.ComponentModel;

namespace CompiladorJS_
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ejecutarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var lexico = new Lexico(txtBox.Text);
            lexico.EjecutarLexico();

            var objSintactico = new Sintactico(lexico.listaDeToken);
            objSintactico.EjecutarSintactico(objSintactico.listaDeTokens);

            List<Error> listaErroresLexico = lexico.listaDeErrorLexico;
            List<Error> listaErroresSintactico = objSintactico.listaDeError;

            List<Error> listaErrores = listaErroresLexico.Union(listaErroresSintactico).ToList();

            var Lista = new BindingList<Token>(lexico.listaDeToken);
            dataGridTokens.DataSource = null;
            dataGridTokens.DataSource = Lista;
            dataGridViewErrores.DataSource = null;
            dataGridViewErrores.DataSource = listaErrores;
        }
    }
}