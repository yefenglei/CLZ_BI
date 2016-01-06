using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Svg;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Text;
using System.Drawing.Imaging;

namespace CLZ.Controllers
{
    public class ChartHelperController : Controller
    {
        //
        // GET: /ChartHelper/
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Index()
        {
            //判断参数是否正确
            //type是可以自己指定的导出类型
            //svg是默认传递的
            //filename是可以自己指定的文件名
            if (Request.Form["type"] != null && Request.Form["svg"] != null && Request.Form["filename"] != null)
            {
                //获得相应参数
                string tType = Request.Form["type"].ToString();
                string tSvg = Request.Form["svg"].ToString();
                string tFileName = Request.Form["filename"].ToString();
                if (tFileName == "")
                {
                    tFileName = "chart";
                }
                
                //将svg转换为二进制流
                MemoryStream tData = new MemoryStream(Encoding.UTF8.GetBytes(tSvg));
                MemoryStream tStream = new MemoryStream();

                string tTmp = new Random().Next().ToString();

                string tExt = "";
                string tTypeString = "";
                //获取导出类型
                switch (tType)
                {
                    case "image/png":
                        tTypeString = "-m image/png";
                        tExt = "png";
                        break;
                    case "image/jpeg":
                        tTypeString = "-m image/jpeg";
                        tExt = "jpg";
                        break;
                    case "application/pdf":
                        tTypeString = "-m application/pdf";
                        tExt = "pdf";
                        break;
                    case "image/svg+xml":
                        tTypeString = "-m image/svg+xml";
                        tExt = "svg";
                        break;
                }

                if (tTypeString != "")
                {
                    string tWidth = Request.Form["width"].ToString();
                    Svg.SvgDocument tSvgObj = SvgDocument.Open(tData);
                    
                    //tSvgObj.Width = new SvgUnit((float)Convert.ToDouble(tWidth));
                    
                    switch (tExt)
                    {
                        case "jpg":
                            tSvgObj.Draw().Save(tStream, ImageFormat.Jpeg);
                            //DELETING SVG FILE 
                            //if (File.Exists(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tSvgOuputFile))
                            //{
                            //    File.Delete(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tSvgOuputFile);
                            //}
                            break;
                        case "png":
                            tSvgObj.Draw().Save(tStream, ImageFormat.Png);
                            ////DELETING SVG FILE
                            //if (File.Exists(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tSvgOuputFile))
                            //{
                            //    File.Delete(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tSvgOuputFile);
                            //}
                            break;
                        case "pdf":

                            PdfWriter tWriter = null;
                            Document tDocumentPdf = null;
                            try
                            {
                                // First step saving png that would be used in pdf
                                tSvgObj.Draw().Save(tStream, ImageFormat.Png);

                                // Creating pdf document
                                tDocumentPdf = new Document(new iTextSharp.text.Rectangle((float)tSvgObj.Width, (float)tSvgObj.Height));
                                // setting up margin to full screen image
                                tDocumentPdf.SetMargins(0.0f, 0.0f, 0.0f, 0.0f);
                                // creating image
                                iTextSharp.text.Image tGraph = iTextSharp.text.Image.GetInstance(tStream.ToArray());
                                tGraph.ScaleToFit((float)tSvgObj.Width, (float)tSvgObj.Height);

                                tStream = new MemoryStream();

                                // Insert content
                                tWriter = PdfWriter.GetInstance(tDocumentPdf, tStream);
                                tDocumentPdf.Open();
                                tDocumentPdf.NewPage();
                                tDocumentPdf.Add(tGraph);
                                tDocumentPdf.CloseDocument();

                                //// deleting png
                                //if (File.Exists(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tOutputFile.Substring(0, tOutputFile.LastIndexOf(".")) + ".png"))
                                //{
                                //    File.Delete(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tOutputFile.Substring(0, tOutputFile.LastIndexOf(".")) + ".png");
                                //}

                                //// deleting svg
                                //if (File.Exists(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tSvgOuputFile))
                                //{
                                //    File.Delete(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tSvgOuputFile);
                                //}

                            }
                            catch (Exception ex)
                            {
                                throw ex;

                            }
                            finally
                            {
                                //正确释放资源
                                tDocumentPdf.Close();
                                tDocumentPdf.Dispose();
                                tWriter.Close();
                                tWriter.Dispose();
                                tData.Dispose();
                                tData.Close();

                            }
                            break;
                        case "svg":
                            tStream = tData;
                            break;
                    }
                    //if (File.Exists(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tOutputFile))
                    //{

                    //Putting session to be able to delete file in temp directory
                    //Session["sFileToTransmit_highcharts_export"] = File.ReadAllBytes(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tOutputFile);
                    //First step deleting disk file;
                    //File.Delete(Server.MapPath(null) + ConfigurationManager.AppSettings["EXPORT_TEMP_FOLDER"].ToString() + tOutputFile);

                    //保存图表路径 可以自己指定
                    tFileName = Server.MapPath("~/Report/") + tFileName + "." + tExt;
                    
                    //Response.ClearContent();
                    Response.ClearHeaders();
                    Response.ContentType = tType;
                    //Response.AppendHeader("Content-Disposition", "attachment; filename=" + tFileName + "." + tExt + "");
                    //将二进制流保存为指定路径下的具体文件    
                    System.IO.File.WriteAllBytes(tFileName, tStream.ToArray());
                    //Response.BinaryWrite(tStream.ToArray());                            
                    //Response.Write("恭喜您，highcharts导出成功，路径为" + tFileName);
                    //Response.End();
                    //}
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(1,JsonRequestBehavior.AllowGet);
        }

    }
}
