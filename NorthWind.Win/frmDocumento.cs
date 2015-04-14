using NorthWind.Entity;
using NorthWind.Win.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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

        private void button1_Click(object sender, EventArgs e)
        {
            //Boton Seleccionar Cliente
            frmCliente oFrmCliente = new frmCliente();
            oFrmCliente.onClienteSeleccionado += new EventHandler<TbClienteBE>(
                oFrmCliente_OnClienteSeleccionado);
            oFrmCliente.Show();
        }

        TbClienteBE otmpCliente;
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
            oFrmProducto.Show();
        }

        TbProductoBE otmpProducto;
        void oFrmProducto_OnProductoSeleccionado(object sender, TbProductoBE e)
        {
            txtproducto.Text = e.Descripcion;
            txtprecio.Text = e.Precio;            
            otmpProducto = e;
        }

        DocumentoBL oFacturaBL = new DocumentoBL();
        private void button3_Click(object sender, EventArgs e)
        {
            ValidarCantidad();
            
            
            //Boton Agregar Factura
            oFacturaBL.AgregarDetalle(new ItemBE() { 
                Cantidad=Convert.ToInt32(txtcantidad.Text),
                Precio=Convert.ToDecimal(txtprecio.Text),
                Producto=otmpProducto
            });

            //Actualizar DataGrid
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = oFacturaBL.GetDetalle();


            txtsubtotal.Text = oFacturaBL.SubTotal.ToString();
            txtigv.Text = oFacturaBL.IGV.ToString();
            txttotal.Text = oFacturaBL.Total.ToString();

        }

        private void ValidarCantidad(){

            if (txtcantidad.Text.Trim() == String.Empty)
            {
                DocumentoErrorProvider.SetError(txtcantidad, "Ingrese cantidad de productos");
                return;
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
                return;

            }


        }
    }
}
