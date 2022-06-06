using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CAR_SPARE_PARTS.Models.Order;
using CAR_SPARE_PARTS.Models.Store;
using Microsoft.Office.Interop.Word;
using Word = Microsoft.Office.Interop.Word;

namespace CAR_SPARE_PARTS.Classes
{
    public class OrdersData
    {

        int UserID { get; set; }

        public OrdersData(int userId)
        {
            UserID = userId;
        }

        public void AddOrder(Order order)
        {
            using (var dbContext = new OrderContext())
            {
                Order existOrder = dbContext.Orders.SingleOrDefault(o => o.UserID == UserID);
                if (existOrder != null)
                {
                    existOrder.Name = order.Name;
                    existOrder.LastName = order.LastName;
                    existOrder.MiddleName = order.MiddleName;
                    existOrder.Address = order.Address;
                    existOrder.Date = order.Date;
                    existOrder.PriceOfOrder = order.PriceOfOrder;
                }
                else
                {
                    dbContext.Orders.Add(order);
                }
                dbContext.SaveChanges();
            }
            using (var dbContext = new CartProductListContext())
            {
                dbContext.CartProductsList.RemoveRange(dbContext.CartProductsList);
                dbContext.SaveChanges();
            }
        }

        public Order GetExistOrder()
        {
            using (var dbContext = new OrderContext())
            {
                return dbContext.Orders.SingleOrDefault(o => o.UserID == UserID);
            }
        }

        public void ExportOrders()
        {
            var application = new Microsoft.Office.Interop.Word.Application();
            Thread.Sleep(1000);
            Document document = application.Documents.Add();
            application.Visible = true;

            using (var dbContext = new OrderContext())
            {
                InsertParagraph(document, "Заказы выполненные за все время", "Обычный", alignment: WdParagraphAlignment.wdAlignParagraphCenter);
                InsertOrderTable(document, dbContext.Orders.OrderByDescending(p => p.PriceOfOrder).ToList());
            }
            document.SaveAs2(@"C:\Users\vladi\OneDrive\Рабочий стол");
        }

        private void InsertParagraph(Document document, string text, string styleType, WdParagraphAlignment alignment = WdParagraphAlignment.wdAlignParagraphLeft, WdColor color = WdColor.wdColorBlack)
        {
            Word.Paragraph paragraph = document.Paragraphs.Add();
            Word.Range range = paragraph.Range;
            range.Shading.ForegroundPatternColor = color;
            range.Text = text;
            paragraph.set_Style(styleType);
            paragraph.Alignment = alignment;
            range.InsertParagraphAfter();
        }

        private void InsertOrderTable(Document document, List<Order> orders)
        {
            Word.Paragraph tableParagrap = document.Paragraphs.Add();
            Word.Range tableRange = tableParagrap.Range;
            Word.Table ordersTable = document.Tables.Add(tableRange, orders.Count() + 1, 7);
            ordersTable.Borders.InsideLineStyle = ordersTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            ordersTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            ordersTable.Borders.InsideLineStyle = ordersTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            ordersTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;


            Word.Range cellRange;
            cellRange = ordersTable.Cell(1, 1).Range;
            cellRange.Text = "ID ПОЛЬЗОВАТЕЛЯ";
            cellRange = ordersTable.Cell(1, 2).Range;
            cellRange.Text = "ФАМИЛИЯ";
            cellRange = ordersTable.Cell(1, 3).Range;
            cellRange.Text = "ИМЯ";
            cellRange = ordersTable.Cell(1, 4).Range;
            cellRange.Text = "ОТЧЕСТВО";
            cellRange = ordersTable.Cell(1, 5).Range;
            cellRange.Text = "АДРЕС";
            cellRange = ordersTable.Cell(1, 6).Range;
            cellRange.Text = "ДАТА ПОЛУЧЕНИЯ ЗАКАЗА";
            cellRange = ordersTable.Cell(1, 7).Range;
            cellRange.Text = "СТОИМОСТЬ";

            for(int i = 1; i <= 7; i++)
            {
                ordersTable.Cell(1, i).Range.Shading.BackgroundPatternColor = Word.WdColor.wdColorGray20;
            }

            WdColor temporaryColor;
            for (int i = 0; i < orders.Count(); ++i)
            {
                

                var currentOrder = orders[i];

                cellRange = ordersTable.Cell(i + 2, 1).Range;
                cellRange.Text = currentOrder.UserID.ToString();

                cellRange = ordersTable.Cell(i + 2, 2).Range;
                cellRange.Text = currentOrder.LastName;

                cellRange = ordersTable.Cell(i + 2, 3).Range;
                cellRange.Text = currentOrder.Name;

                cellRange = ordersTable.Cell(i + 2, 4).Range;
                cellRange.Text = currentOrder.MiddleName;

                cellRange = ordersTable.Cell(i + 2, 5).Range;
                cellRange.Text = currentOrder.Address;

                cellRange = ordersTable.Cell(i + 2, 6).Range;
                cellRange.Text = currentOrder.Date;

                cellRange = ordersTable.Cell(i + 2, 7).Range;
                cellRange.Text = currentOrder.PriceOfOrder.ToString();
                if (orders[i].PriceOfOrder >= 10000)
                {
                    temporaryColor = WdColor.wdColorDarkRed;
                }
                else if (orders[i].PriceOfOrder >= 7500)
                {
                    temporaryColor = WdColor.wdColorRed;
                }
                else if (orders[i].PriceOfOrder >= 5000)
                {
                    temporaryColor = WdColor.wdColorOrange;
                }
                else if(orders[i].PriceOfOrder >= 2500)
                {
                    temporaryColor = WdColor.wdColorLightOrange;
                }
                else if (orders[i].PriceOfOrder >= 1500)
                {
                    temporaryColor = WdColor.wdColorDarkYellow;
                }
                else if (orders[i].PriceOfOrder >= 1000)
                {
                    temporaryColor = WdColor.wdColorYellow;
                }
                else if (orders[i].PriceOfOrder >= 500)
                {
                    temporaryColor = WdColor.wdColorLightYellow;
                }
                else if (orders[i].PriceOfOrder >= 250)
                {
                    temporaryColor = WdColor.wdColorDarkGreen;
                }
                else if (orders[i].PriceOfOrder >= 100)
                {
                    temporaryColor = WdColor.wdColorGreen;
                }
                else
                {
                    temporaryColor = WdColor.wdColorLightGreen;
                }
                for (int j = 1; j <= 7; j++)
                {
                    ordersTable.Cell(i + 2, j).Range.Shading.BackgroundPatternColor = temporaryColor;
                }
            }

            ordersTable.AllowAutoFit = true;
            Word.Column firstColumn = ordersTable.Columns[1];
            firstColumn.SetWidth(100, Word.WdRulerStyle.wdAdjustFirstColumn);
        }
    }
}