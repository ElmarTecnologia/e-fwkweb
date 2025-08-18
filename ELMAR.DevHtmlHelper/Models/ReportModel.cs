using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Drawing;
using DevExpress.XtraReports.UI;

namespace ELMAR.DevHtmlHelper.Models
{
    public class ReportModel
    {
        //Atributos
        private bool _paisagem;

        private string infoColunas;

        private int MaxLargura;

        public string Titulo
        {
            get;
            set;
        }

        public string SubTitulo
        {
            get;
            set;
        }

        public string Logo
        {
            get;
            set;
        }

        public string HeaderInfo
        {
            get;
            set;
        }

        public string WaterMark
        {
            get;
            set;
        }

        public object Model
        {
            get;
            set;
        }

        public string sqlCode
        {
            get;
            set;
        }

        public string bdConnection
        {
            get;
            set;
        }

        public string[] ColunasOcultas
        {
            get;
            set;
        }

        public string[] Agrupadores
        {
            get;
            set;
        }

        public string[] Totalizadores
        {
            get;
            set;
        }

        public string[] ColunasCurrency
        {
            get;
            set;
        }

        public bool showTableBorders
        {
            get;
            set;
        }

        public bool showTotalizadores
        {
            get;
            set;
        }

        public bool GroupPageBreak
        {
            get;
            set;
        }

        public string SummaryOperation
        {
            get;
            set;
        }

        public bool Paisagem
        {
            get
            {
                return this._paisagem;
            }
            set
            {
                this._paisagem = value;
                this.MaxLargura = 600;
                if (this.Paisagem)
                {
                    this.MaxLargura = 950;
                }
            }
        }

        private Dictionary<string, Dictionary<string, string>> dicInfoColunas
        {
            get;
            set;
        }

        public string InfoColunas
        {
            get
            {
                return this.infoColunas;
            }
            set
            {
                this.infoColunas = value;
                this.setDicInfoColunas();
            }
        }

        public XtraReport CreateReportViewer()
        {
            //Cria o Objeto XtraReport
            XtraReport xtraReport = new XtraReport();
            xtraReport.PaperKind = System.Drawing.Printing.PaperKind.A4;
            xtraReport.Landscape = this.Paisagem;
            xtraReport.VerticalContentSplitting = VerticalContentSplitting.Smart;
            //Adiciona a marca d'água
            if (!string.IsNullOrEmpty(this.WaterMark))
            {
                try
                {
                    xtraReport.Watermark.Image = System.Drawing.Image.FromFile(this.WaterMark);
                    xtraReport.Watermark.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                    xtraReport.Watermark.ImageTiling = false;
                    xtraReport.Watermark.ImageViewMode = ImageViewMode.Clip;
                    xtraReport.Watermark.ImageTransparency = 250;
                    xtraReport.Watermark.ShowBehind = true;
                }
                catch { }
            }
            //Cabeçalho da página
            PageHeaderBand pageHeaderBand = new PageHeaderBand
            {
                HeightF = 160f,
                TextAlignment = TextAlignment.TopCenter,
                KeepTogether = true
            };
            xtraReport.Bands.Add(pageHeaderBand);
            //Adiciona a Logo ao cabeçalho da página
            if (!string.IsNullOrEmpty(this.Logo))
            {
                try
                {
                    pageHeaderBand.Controls.Add(new XRPictureBox
                    {
                        Image = System.Drawing.Image.FromFile(this.Logo),
                        WidthF = 90f,
                        HeightF = 90f,
                        Sizing = ImageSizeMode.Squeeze,
                        LocationF = new System.Drawing.PointF(0f, 10f)
                    });
                }
                catch { }
            }
            //Adicionando informações ao cabeçalho
            if (!string.IsNullOrEmpty(this.HeaderInfo))
            {
                pageHeaderBand.Controls.Add(new XRLabel
                {
                    Text = this.HeaderInfo,
                    SizeF = new System.Drawing.SizeF(350f, 60f),
                    LocationF = new System.Drawing.PointF(150f, 10f),
                    TextAlignment = TextAlignment.TopLeft,
                    Font = new System.Drawing.Font("Arial", 12f, System.Drawing.FontStyle.Bold)
                });
            }
            //Título do relatório
            pageHeaderBand.Controls.Add(new XRLabel
            {
                Text = this.Titulo,
                SizeF = new System.Drawing.SizeF((float)this.MaxLargura, 20f),
                LocationF = new System.Drawing.PointF(0f, 100f),
                TextAlignment = TextAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Arial", 12f, System.Drawing.FontStyle.Bold)
            });
            //Subtítulo do relatório
            pageHeaderBand.Controls.Add(new XRLabel
            {
                Text = this.SubTitulo,
                SizeF = new System.Drawing.SizeF((float)this.MaxLargura, 20f),
                LocationF = new System.Drawing.PointF(10f, 120f),
                TextAlignment = TextAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold)
            });
            pageHeaderBand.Controls.Add(new XRLine
            {
                WidthF = (float)this.MaxLargura,
                LocationF = new System.Drawing.PointF(10f, 130f)
            });
            XRCrossBandControl xRCrossBandControl = new XRCrossBandBox();
            //Criação da banda detalhe
            DetailBand detailBand = new DetailBand();
            detailBand.HeightF = 30f;
            detailBand.KeepTogether = true;
            if (this.showTableBorders)
            {
            }
            xtraReport.Bands.Add(detailBand);
            if (!(this.Model is IEnumerable))
            {
                if (!string.IsNullOrEmpty(this.sqlCode))
                {
                    FwkContexto fwkContexto = new FwkContexto(this.bdConnection);
                    //Cria o modelo a partir do código SQL
                    this.Model = fwkContexto.SelectionQuery(this.sqlCode);
                }
            }
            float num = 0f;
            //Banda de agrupamento do cabeçalho
            GroupHeaderBand groupHeaderBand = new GroupHeaderBand();
            if (this.Agrupadores.Length == 0 || this.Agrupadores[0].Equals(string.Empty))
            {
                groupHeaderBand.HeightF = 15f;
                num += groupHeaderBand.HeightF;
                groupHeaderBand.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold);
                groupHeaderBand.RepeatEveryPage = true;
                //Não separa os dados do grupo
                groupHeaderBand.GroupUnion = GroupUnion.WholePage;
                ////Exibe totalizadores no report footer
                //if (this.showTotalizadores)
                //{
                //    foreach (var item in Totalizadores)
                //    {
                //        XRLabel xRLabel = new XRLabel
                //        {
                //            LocationF = new System.Drawing.PointF(0f, 2f),
                //            SizeF = new System.Drawing.SizeF((float)this.MaxLargura, 10f),
                //            TextAlignment = TextAlignment.MiddleCenter,
                //            Font = new System.Drawing.Font("Arial", 8f, System.Drawing.FontStyle.Bold)
                //        };
                //        xRLabel.DataBindings.Add("Text", null, item);                        
                //        xRLabel.TextFormatString = "{0:c2}";
                //        this.SetSummaryLabel(xRLabel, item+" Total: ", this.SummaryOperation, "Report");
                //        groupHeaderBand.Controls.Add(xRLabel);
                //    }                    
                //}
                xtraReport.Bands.Add(groupHeaderBand);
            }
            float num2 = 0f;
            int num3 = 1;
            GroupHeaderBand grpHeaderBand = new GroupHeaderBand();
            grpHeaderBand.HeightF = 30f;
            grpHeaderBand.RepeatEveryPage = true;
            xtraReport.Bands.Add(grpHeaderBand);
            //Adicionando os dados ao relatório
            foreach (DataColumn dataColumn in ((DataView)this.Model).ToTable().Columns)
            {
                //Adiciona caso não pertença às colunas ocultas
                if (this.ColunasOcultas.Length <= 0 || !this.ColunasOcultas.ToList<string>().Contains(dataColumn.Caption))
                {
                    //Adiciona os grupos
                    if (this.Agrupadores.Length <= 0 || !this.Agrupadores.ToList<string>().Contains(dataColumn.Caption))
                    {
                        int num4 = int.Parse(this.GetColParametro(dataColumn.Caption, "Largura", "135"));
                        XRLabel xRLabel2 = new XRLabel
                        {
                            //LocationF = new System.Drawing.PointF(num2, num + 35f),
                            LocationF = new System.Drawing.PointF(num2, 15f),
                            SizeF = new System.Drawing.SizeF((float)num4, 15f),
                            TextAlignment = TextAlignment.MiddleCenter,
                            Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Bold),
                            Padding = new PaddingInfo(10, 20, 0, 0),
                            Text = dataColumn.Caption
                        };
                        //Exibe as bordas da tabela
                        if (this.showTableBorders)
                        {
                            xRLabel2.BorderColor = System.Drawing.Color.Black;
                            xRLabel2.BorderWidth = 1;
                            xRLabel2.Borders = BorderSide.All;
                        }
                        //xtraReport.Bands[xtraReport.Bands.Count - this.Agrupadores.Length].Controls.Add(xRLabel2);
                        grpHeaderBand.Controls.Add(xRLabel2);
                        //Define a parametrização dinâmica [Align = Alinhamento]
                        string colParametro = this.GetColParametro(dataColumn.Caption, "Align", "Left");
                        TextAlignment textAlignment = TextAlignment.MiddleLeft;
                        switch (colParametro)
	                    {
                            case "Center":
                            textAlignment = TextAlignment.MiddleCenter;
                            break;
                            case "Right":
                            textAlignment = TextAlignment.MiddleRight;
                            break;
	                    }
                        string stringFormat = this.GetColParametro(dataColumn.Caption, "StringFormat", "");
                        XRLabel xRLabel3 = new XRLabel
                        {
                            LocationF = new System.Drawing.PointF(num2, 0f),
                            SizeF = new System.Drawing.SizeF((float)num4, detailBand.HeightF),
                            TextAlignment = textAlignment,
                            Padding = new PaddingInfo(10, 20, 0, 0),
                            Font = new System.Drawing.Font("Arial", 7f),
                            TextFormatString = stringFormat
                        };
                        if (this.showTableBorders)
                        {
                            xRLabel3.BorderColor = System.Drawing.Color.Black;
                            xRLabel3.BorderWidth = 1;
                            xRLabel3.Borders = BorderSide.Top;
                        }
                        num2 += (float)num4;
                        //if (Totalizadores.Contains(dataColumn.Caption))
                        //{
                        //    xRLabel3.Summary.Func = (SummaryFunc)System.Enum.Parse(typeof(SummaryFunc), SummaryOperation);
                        //    xRLabel3.Summary.Running = (SummaryRunning)System.Enum.Parse(typeof(SummaryRunning), "Report");                            
                        //}
                        xRLabel3.DataBindings.Add("Text", null, dataColumn.Caption);
                        if (ColunasCurrency.Contains(dataColumn.Caption))
                        {
                            xRLabel3.TextFormatString = "{0:c2}";
                        }
                        xRLabel3.DataBindings.Add("Text", null, dataColumn.Caption);
                        detailBand.Controls.Add(xRLabel3);
                        continue;                    
                    }                    
                    //Criando os grupos
                    groupHeaderBand = new GroupHeaderBand();
                    groupHeaderBand.HeightF = 10f;
                    num += groupHeaderBand.HeightF;
                    groupHeaderBand.Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold);
                    //Exibe as informações do agrupamento em cada página
                    groupHeaderBand.RepeatEveryPage = true;
                    groupHeaderBand.GroupUnion = GroupUnion.WholePage;
                    //Adiciona a quebra de página / totalizadores ao final do agrupamento
                    if (num3 == this.Agrupadores.Length)
                    {
                        //groupHeaderBand.PageBreak = PageBreak.BeforeBand;
                        groupHeaderBand.PageBreak = GroupPageBreak? PageBreak.BeforeBand: PageBreak.None;
                        if (this.showTotalizadores)
                        {
                            XRLabel xRLabel = new XRLabel
                            {
                                LocationF = new System.Drawing.PointF(0f, 20f),
                                SizeF = new System.Drawing.SizeF((float)this.MaxLargura, 10f),
                                TextAlignment = TextAlignment.MiddleCenter,
                                Font = new System.Drawing.Font("Arial", 8f, System.Drawing.FontStyle.Bold)                                
                            };
                            this.SetSummaryLabel(xRLabel, "Qtde.: ", this.SummaryOperation, "Group");
                            groupHeaderBand.Controls.Add(xRLabel);
                        }
                    }
                    xtraReport.Bands.Add(groupHeaderBand);
                    GroupField item = new GroupField(dataColumn.Caption);
                    groupHeaderBand.GroupFields.Add(item);
                    XRLabel xRLabel4 = new XRLabel
                    {
                        LocationF = new System.Drawing.PointF(0f, 10f),
                        SizeF = new System.Drawing.SizeF((float)this.MaxLargura, 10f),
                        TextAlignment = TextAlignment.MiddleCenter,
                        Font = new System.Drawing.Font("Arial", 10f, System.Drawing.FontStyle.Bold)
                    };
                    xRLabel4.DataBindings.Add("Text", null, dataColumn.Caption);
                    groupHeaderBand.Controls.Add(xRLabel4);
                    //Colunas informativas
                    if (((DataView)this.Model).ToTable().Columns.Contains(dataColumn.Caption + "_INFO"))
                    {
                        XRLabel xRLabel5 = new XRLabel
                        {
                            LocationF = new System.Drawing.PointF(0f, 20f),
                            SizeF = new System.Drawing.SizeF((float)this.MaxLargura, 10f),
                            TextAlignment = TextAlignment.MiddleCenter,
                            Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Bold)
                        };
                        xRLabel5.DataBindings.Add("Text", null, dataColumn.Caption + "_INFO");
                        groupHeaderBand.Controls.Add(xRLabel5);
                    }
                    num3++;
                }
            }
            PageFooterBand pageFooterBand = new PageFooterBand
            {
                HeightF = 50f,
                TextAlignment = TextAlignment.TopCenter
            };

            //Exibe totalizadores no report footer
            if (this.showTotalizadores)
            {
                foreach (var item in Totalizadores)
                {
                    XRLabel xRLabel = new XRLabel
                    {
                        LocationF = new System.Drawing.PointF(0f, 0f),
                        SizeF = new System.Drawing.SizeF((float)this.MaxLargura, 10f),
                        TextAlignment = TextAlignment.MiddleCenter,
                        Font = new System.Drawing.Font("Arial", 8f, System.Drawing.FontStyle.Bold)
                    };
                    xRLabel.DataBindings.Add("Text", null, item);
                    xRLabel.TextFormatString = "{0:c2}";
                    this.SetSummaryLabel(xRLabel, item + " Total: ", this.SummaryOperation, "Report");
                    pageFooterBand.Controls.Add(xRLabel);
                }
            }

            //Adicionando o rodapé da página
            xtraReport.Bands.Add(pageFooterBand);
            pageFooterBand.Controls.Add(new XRLabel
            {
                Text = this.Titulo,
                LocationF = new System.Drawing.PointF(5f, 40f),
                SizeF = new System.Drawing.SizeF((float)this.MaxLargura, 30f),
                Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Bold)
            });
            //Adicionando o rodapé do relatório
            ReportFooterBand reportFooterBand = new ReportFooterBand
            {
                HeightF = 70f,
                TextAlignment = TextAlignment.TopRight
            };
            xtraReport.Bands.Add(reportFooterBand);
            reportFooterBand.Controls.Add(new XRLine
            {
                WidthF = (float)this.MaxLargura,
                LocationF = new System.Drawing.PointF(10f, 10f)
            });
            //Informação da geração do relatório
            reportFooterBand.Controls.Add(new XRLabel
            {
                Text = "Gerado em " + DateTime.Now.ToShortDateString() + " às " + DateTime.Now.ToShortTimeString(),
                LocationF = new System.Drawing.PointF(5f, 40f),
                SizeF = new System.Drawing.SizeF((float)this.MaxLargura, 30f),
                Font = new System.Drawing.Font("Arial", 9f, System.Drawing.FontStyle.Bold)
            });
            //Aplicado o Model ao DataSource do relatório
            xtraReport.DataSource = this.Model;

            return xtraReport;
        }

        /// <summary>
        /// Método para extrair os dados das colunas informativas
        /// </summary>
        private void setDicInfoColunas()
        {
            string[] array = this.InfoColunas.Split(';');
            this.dicInfoColunas = new Dictionary<string, Dictionary<string, string>>();
            if (array.Length > 0)
            {
                string[] array2 = array;
                for (int i = 0; i < array2.Length; i++)
                {
                    string text = array2[i];
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    string[] array3 = text.Split('|');
                    string key = array3[0];
                    for (int j = 1; j < array3.Length; j++)
                    {
                        string[] array4 = array3[j].Split('=');
                        dictionary.Add(array4[0].Trim(), array4[1]);
                    }
                    this.dicInfoColunas.Add(key.Trim(), dictionary);
                }
            }
        }

        /// <summary>
        /// Pega os parâmetros relacionados a uma colunas a partir do caption e nome do parâmetro
        /// </summary>
        /// <param name="caption">Caption da coluna</param>
        /// <param name="parametro">Nome do parâmetro</param>
        /// <param name="defaultValue">Valor padrão para o parâmetro</param>
        /// <returns></returns>
        public string GetColParametro(string caption, string parametro, string defaultValue = "")
        {
            string result;
            if (this.dicInfoColunas != null && this.dicInfoColunas.ContainsKey(caption))
            {
                if (this.dicInfoColunas.ContainsKey(caption))
                {
                    if (dicInfoColunas[caption].ContainsKey(parametro))
                    {
                        result = this.dicInfoColunas[caption][parametro];
                        return result;
                    }
                }
            }
            result = defaultValue;
            return result;
        }

        /// <summary>
        /// Define o label com as informações de somatórios
        /// </summary>
        /// <param name="label">XRLabel alvo da configuração</param>
        /// <param name="info">Informação do totalizador</param>
        public void SetSummaryLabel(XRLabel label, string info, string SummaryOperation = "Count", string SummaryRunning = "Report")
        {
            label.Summary = new XRSummary
            {
                Func = (SummaryFunc)System.Enum.Parse(typeof(SummaryFunc), SummaryOperation),
                Running = (SummaryRunning)System.Enum.Parse(typeof(SummaryRunning), SummaryRunning),
                IgnoreNullValues = true,
                FormatString = info + " {0}",
                TreatStringsAsNumerics = true
            };
        }
    }
}