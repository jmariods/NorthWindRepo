using NorthWind.Entity;
using NorthWind.Win.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NorthWind.Win
{
    public partial class frmDocumento : Form
    {
        public frmDocumento()
        {
            InitializeComponent();
        }


        #region Variables globales
        TbClienteBE otmpCliente;
        TbProductoBE otmpProducto;
        DocumentoBL oFacturaBL = new DocumentoBL();
        String IdProducto = String.Empty;
        #endregion


        #region Eventos del Formulario
        private void button1_Click(object sender, EventArgs e)
        {
            //Boton Seleccionar Cliente
            frmCliente oFrmCliente = new frmCliente();
            oFrmCliente.onClienteSeleccionado += new EventHandler<TbClienteBE>(
                oFrmCliente_OnClienteSeleccionado);
            oFrmCliente.Show();
        }


        void oFrmCliente_OnClienteSeleccionado(object sender, TbClienteBE e)
        {
            txtcliente.Text = e.Nombre;
            txtruc.Text = e.Ruc;
            otmpCliente = e;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Boton Seleccionar Producto
            frmProducto oFrmProducto = new frmProducto();
            oFrmProducto.onProductoSeleccionado += new EventHandler<TbProductoBE>(oFrmProducto_OnProductoSeleccionado);
            txtcantidad.Focus();
            oFrmProducto.Show();
        }


        void oFrmProducto_OnProductoSeleccionado(object sender, TbProductoBE e)
        {
            txtproducto.Text = e.Descripcion;
            txtprecio.Text = e.Precio;
            otmpProducto = e;
        }


        private void button3_Click(object sender, EventArgs e)
        {
            //Validacion
            bool bFlag = false;
            DocumentoErrorProvider.Clear();
            bFlag = ValidarCliente();
            bFlag = ValidarProducto();
            bFlag = ValidarCantidad();

            if (!bFlag)
            {
                //Agregar Items
                ItemBE oItem = new ItemBE();
                oItem = oFacturaBL.ObtenerItem(oFacturaBL.GetDetalle(), otmpProducto.CodProducto);

                if (oItem == null)
                {
                    oFacturaBL.AgregarDetalle(new ItemBE()
                    {
                        Cantidad = Convert.ToInt32(txtcantidad.Text),
                        Precio = Convert.ToDecimal(txtprecio.Text),
                        Producto = otmpProducto
                    });

                }
                else
                {

                    oFacturaBL.EliminarItem(oFacturaBL.GetDetalle(), otmpProducto.CodProducto);

                    oFacturaBL.AgregarDetalle(new ItemBE()
                    {
                        Cantidad = oItem.Cantidad + Convert.ToInt32(txtcantidad.Text),
                        Precio = Convert.ToDecimal(txtprecio.Text),
                        Producto = otmpProducto
                    });
                }



                //Actualizar DataGrid
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = oFacturaBL.GetDetalle();


                txtsubtotal.Text = oFacturaBL.SubTotal.ToString();
                txtigv.Text = oFacturaBL.IGV.ToString();
                txttotal.Text = oFacturaBL.Total.ToString();
            }

        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Delete)
            {
                if (dataGridView1.RowCount > 0)
                {

                    if (dataGridView1.RowCount == 1)
                    {
                        oFacturaBL.EliminarItem(oFacturaBL.GetDetalle(), oFacturaBL.GetDetalle().ElementAt(0).Producto.CodProducto);
                    }
                    else
                    {
                        oFacturaBL.EliminarItem(oFacturaBL.GetDetalle(), IdProducto);
                    }

                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = oFacturaBL.GetDetalle();

                    txtsubtotal.Text = oFacturaBL.SubTotal.ToString();
                    txtigv.Text = oFacturaBL.IGV.ToString();
                    txttotal.Text = oFacturaBL.Total.ToString();
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ExportarAPlano();

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            Int32 indice = e.RowIndex;
            string s = dataGridView1.Rows[indice].Cells[1].ToString();
            IdProducto = oFacturaBL.GetDetalle().ElementAt(indice).Producto.CodProducto;

        }
        private void button4_Click(object sender, EventArgs e)
        {

        }
        #endregion


        #region Validaciones de Formulario

        private bool ValidarCantidad()
        {

            if (txtcantidad.Text.Trim() == String.Empty)
            {
                DocumentoErrorProvider.SetError(txtcantidad, "Ingrese cantidad de productos");
                return true;
            }
            char[] testArr = txtcantidad.Text.ToCharArray();
            bool testBool = false;
            for (int i = 0; i < testArr.Length; i++)
            {
                if (!char.IsNumber(testArr[i]))
                {
                    testBool = true;
                }
            }
            if (testBool == true)
            {
                DocumentoErrorProvider.SetError(txtcantidad, "Cantidad Invalida");
                return testBool;

            }
            return false;

        }
        private bool ValidarCliente()
        {

            if (txtcliente.Text.Trim() == String.Empty)
            {
                DocumentoErrorProvider.SetError(button1, "Seleccione un cliente");
                return true;

            }
            return false;
        }
        private bool ValidarProducto()
        {
            if (txtproducto.Text.Trim() == String.Empty)
            {
                DocumentoErrorProvider.SetError(button2, "Seleccione un producto");
                return true;
            }

            return false;

        }

        #endregion

        #region Exportar
        private void ExportarAPlano()
        {

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string ruta = saveFileDialog1.FileName;
                using (StreamWriter sw = new StreamWriter(ruta))
                {
                    //Formateando Documento
                    string line0 = "                	Documento de Pago 			          ";
                    string line1 = "**********************************************************";
                    string cab1 = string.Format(@"Fecha: {0}	RUC:{1}		   ", dateTimePicker1.Text.Trim(), txtruc.Text);
                    string cab2 = string.Format(@"Nombre: {0}", txtcliente.Text);

                    //Escribiendo Cabecera
                    sw.WriteLine(line1);
                    sw.WriteLine(line0);
                    sw.WriteLine(line1);
                    sw.WriteLine(cab1);
                    sw.WriteLine(cab2);
                    sw.WriteLine(line1);

                    //Escribiendo Detalle
                    StringBuilder Rowbind = new StringBuilder();
                    for (int k = 0; k < dataGridView1.Columns.Count; k++)
                    {

                        Rowbind.Append(dataGridView1.Columns[k].HeaderText + ' ');
                    }

                    Rowbind.Append("\r\n");
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        for (int k = 0; k < dataGridView1.Columns.Count; k++)
                        {

                            Rowbind.Append(dataGridView1.Rows[i].Cells[k].Value.ToString() + ' ');
                        }

                        Rowbind.Append("\r\n");
                    }
                    sw.WriteLine(Rowbind);

                    //Formateando Totales
                    string det1 = string.Format(@"				SubTotal: 	{0}", txtsubtotal.Text);
                    string det2 = string.Format(@"				IGV: 	{0}", txtigv.Text);
                    string det3 = string.Format(@"				Total: 	{0}", txttotal.Text);

                    //Escribiendo Totales
                    sw.WriteLine(line1);
                    sw.WriteLine(det1);
                    sw.WriteLine(det2);
                    sw.WriteLine(det3);
                    sw.WriteLine(line1);


                    sw.Flush();
                    sw.Close();

                    MessageBox.Show("Se generó correctamente el documento en " + ruta, "Generación Satisfactoria", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }


        }
        #endregion






    }
}
