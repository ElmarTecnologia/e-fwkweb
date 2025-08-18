using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data;
using DevExpress.Web;
using DevExpress.Web.Mvc;

namespace ELMAR.DevHtmlHelper.Models
{
    /// <summary>
    /// Classe auxiliar para criação dinâmica de configurações para o grid (GridViewSettings)
    /// </summary>
    public partial class GridViewSettingsHelper
    {
        private GridViewSettings exportGridViewSettings;
        private string gridName;
        private string gridTitulo;
        private string sqlCode;
        private string gridKey;
        private int gridTamPagina;
        private string Tema;
        private string Controller;
        private string Action;
        private string[] Filtros;
        private string[] Agrupadores;
        private bool Expanded;
        private string[] Totalizadores;
        private string[] ColunasOcultas;
        private string[] ColunasActions;
        private string[] ColunasLinks;
        private string ExportHeaderText;
        private object Model;

        public GridViewSettingsHelper(object Model, string gridName, string gridTitulo, string sqlCode, string gridKey, int gridTamPagina, string Tema, string Controller, string Action, string Filtros, string Agrupadores, bool Expanded, string Totalizadores, string ColunasOcultas, string ColunasActions, string ColunasDateTime, string ColunasCurrency, string ExportHeaderText)
        {
            this.sqlCode = sqlCode;
            this.gridName = gridName;
            this.gridTitulo = gridTitulo;
            this.gridKey = gridKey;
            this.gridTamPagina = gridTamPagina;
            this.Tema = Tema;
            this.Controller = Controller;
            this.Action = Action;
            this.Filtros = (Filtros != null) ? Filtros.Split(';') : null;
            this.Agrupadores = (Agrupadores != null) ? Agrupadores.Split(';') : null;
            this.Totalizadores = (Totalizadores != null) ? Totalizadores.Split(';') : null;
            this.ColunasOcultas = (ColunasOcultas != null) ? ColunasOcultas.Split(';') : null;
            this.ColunasActions = (ColunasActions != null) ? ColunasActions.Split(';') : null;
            this.Expanded = Expanded;
            this.ExportHeaderText = ExportHeaderText;
            this.Model = Model;
            //TODO: Add ColunasDateTime e ColunasCurrency
        }

        public GridViewSettings ExportGridViewSettings
        {
            get
            {
                if (exportGridViewSettings == null)
                {
                    exportGridViewSettings = CreateExportGridViewSettings();
                }
                return exportGridViewSettings;
            }
        }

        private GridViewSettings CreateExportGridViewSettings()
        {
            var settings = new GridViewSettings() { Name = gridName };
            settings.SettingsText.Title = gridTitulo;
            settings.SettingsExport.EnableClientSideExportAPI = true;
            settings.SettingsExport.ExcelExportMode = DevExpress.Export.ExportType.DataAware;            
            settings.SettingsDetail.ExportMode = GridViewDetailExportMode.All;
            settings.SettingsExport.Landscape = true;
            settings.SettingsExport.LeftMargin = 1;
            settings.SettingsExport.RightMargin = 1;
            settings.SettingsExport.TopMargin = 1;
            settings.SettingsExport.BottomMargin = 1;
            settings.SettingsExport.ReportHeader = this.ExportHeaderText.ToUpper() + Environment.NewLine;
            settings.SettingsExport.ReportFooter = Environment.NewLine + "Criado em: " + DateTime.Now.ToShortDateString() + " às " + DateTime.Now.ToShortTimeString();

            List<string> AgrupadoresList = Agrupadores.ToList<string>();
            if(Model == null)
            {
                return settings;
            }
            foreach (System.Data.DataColumn col in (System.Data.DataColumnCollection)((System.Data.DataView)Model).ToTable().Columns)
            {
                var dataType = col.DataType.ToString();
                switch (dataType)
                {
                    case "System.DateTime":
                        settings.Columns.Add(column =>
                        {
                            column.FieldName = col.Caption;
                            column.ColumnType = MVCxGridViewColumnType.DateEdit;
                            column.Width = System.Web.UI.WebControls.Unit.Pixel(85);
                            column.GroupIndex = AgrupadoresList.IndexOf(col.Caption);
                        });
                        break;
                    case "System.Double":
                        settings.Columns.Add(column =>
                        {
                            column.FieldName = col.Caption;
                            column.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
                            column.PropertiesEdit.DisplayFormatString = "c";
                            if (((System.Data.DataView)Model).ToTable().Columns.Count > 12)
                            {
                                column.Width = System.Web.UI.WebControls.Unit.Pixel(80);
                            }
                            column.GroupIndex = AgrupadoresList.IndexOf(col.Caption);
                        });
                        break;
                    case "System.String":
                        settings.Columns.Add(column =>
                        {
                            column.FieldName = col.Caption;
                            column.UnboundType = DevExpress.Data.UnboundColumnType.String;
                            if (col.Caption.Equals("Histórico"))
                            {
                                column.Width = System.Web.UI.WebControls.Unit.Pixel(250);
                            }
                            if (((System.Data.DataView)Model).ToTable().Columns.Count > 12)
                            {
                                column.Width = System.Web.UI.WebControls.Unit.Pixel(85);
                            }
                            column.GroupIndex = AgrupadoresList.IndexOf(col.Caption);
                        });
                        break;
                    default:
                        settings.Columns.Add(col.Caption).GroupIndex = AgrupadoresList.IndexOf(col.Caption);
                        settings.Columns[col.Caption].Width = 90;
                        if (((System.Data.DataView)Model).ToTable().Columns.Count > 12)
                        {
                            settings.Columns[col.Caption].Width = System.Web.UI.WebControls.Unit.Pixel(80);
                        }
                        break;
                }
            }

            settings.SettingsPager.PageSize = gridTamPagina;
            settings.Theme = Tema;
            settings.EnableTheming = true;
            settings.Enabled = true;
            settings.SettingsBehavior.AllowSort = true;
            settings.SettingsBehavior.AllowGroup = true;
            settings.SettingsBehavior.AllowSelectByRowClick = true;
            settings.SettingsBehavior.AllowSelectSingleRowOnly = true;
            settings.Settings.ShowGroupPanel = true;
            settings.Settings.ShowFilterRow = true;
            settings.Settings.ShowTitlePanel = true;
            settings.Width = System.Web.UI.WebControls.Unit.Percentage(100);            
            settings.CallbackRouteValues = new
            {
                Controller = Controller
                ,
                Action = Action
                ,
                dtSource = Model
                ,
                sqlCode = sqlCode
                ,
                Tema = Tema
                ,
                tamPagina = gridTamPagina
                ,
                gridTitulo = gridTitulo
                ,
                gridKey = gridKey
                ,
                ExibeExportador = false
            };
            settings.KeyFieldName = gridKey;

            if (ColunasOcultas != null && !string.IsNullOrEmpty(ColunasOcultas[0]))
            {
                foreach (string name in ColunasOcultas)
                {
                    if (settings.Columns[name] != null)
                        settings.Columns[name].Visible = false;
                }
            }

            if (ColunasActions != null && !string.IsNullOrEmpty(ColunasActions[0]))
            {
                foreach (string item in ColunasActions)
                {
                    string name = item.Split('|')[0];
                    if (settings.Columns[name] != null)
                        settings.Columns[name].Visible = false;
                }
            }

            if (ColunasLinks != null && !string.IsNullOrEmpty(ColunasLinks[0]))
            {
                foreach (string item in ColunasLinks)
                {
                    string name = item.Split('|')[0];
                    if (settings.Columns[name] != null)
                        settings.Columns[name].Visible = false;
                }
            }

            settings.Style.Value = "border-style:none;margin-left:8px;";
            settings.Style.Value += "font-size:10px";

            if (((System.Data.DataView)Model).ToTable().Columns.Count > 12)
            {
                settings.Style.Value += "font-size:8px";
            }

            if (Totalizadores != null && !string.IsNullOrEmpty(Totalizadores[0]))
            {
                foreach (string name in Totalizadores)
                {
                    string[] totalizadoresParams = name.Split('|');
                    //settings.TotalSummary.Add(DevExpress.Data.SummaryItemType.Sum, totalizadoresParams[0]).DisplayFormat = "c";
                    string op = totalizadoresParams.Length > 1 ? totalizadoresParams[1] : "Sum";
                    SummaryItemType Operation = (SummaryItemType)System.Enum.Parse(typeof(SummaryItemType), op);
                    settings.TotalSummary.Add(Operation, totalizadoresParams[0]);
                }
                settings.Settings.ShowFooter = true;
            }

            settings.PreRender = (s, e) =>
            {
                if (string.IsNullOrEmpty(Agrupadores[0]))
                {
                    return;
                }
                var gridView = s as MVCxGridView;
                //if(this.Expanded)
                    gridView.ExpandAll();
            };

            return settings;
        }
    }
}