using NorthWind.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthWind.Win.BL
{
    public class DocumentoBL
    {
        public decimal SubTotal { get; set; }
        public decimal IGV {

            get { return SubTotal * (decimal)0.18; }
        }
        public decimal Total {
            get { return SubTotal + IGV; }
        }
        public List<ItemBE> oDetalle = new List<ItemBE>();

        public void AgregarDetalle(ItemBE oItem)
        {
            SubTotal += oItem.Total;
            oItem.Item = oDetalle.Count + 1;
            oDetalle.Add(oItem);
        }

        public List<ItemBE> GetDetalle()
        {
            return oDetalle;

        }

        public ItemBE ObtenerItem(List<ItemBE> oDetalle,String IdProducto)
        {
            ItemBE oItem = null;
                
                oItem=oDetalle.Find(
                delegate(ItemBE be){
                return be.Producto.CodProducto == IdProducto;
                }
            );
            return oItem;

        }
        public void EliminarItem(List<ItemBE> oDetalle, String IdProducto)
        {
            SubTotal -= oDetalle.Find(
                delegate(ItemBE be)
                {
                    return be.Producto.CodProducto == IdProducto;
                }

                ).Total;

            
            oDetalle.RemoveAt(oDetalle.FindIndex(
                delegate(ItemBE be)
                {
                    return be.Producto.CodProducto == IdProducto;
                }

                ));
        } 

    }
}
