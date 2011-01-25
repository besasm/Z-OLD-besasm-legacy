﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Schema;
using System.Xml;
using System.Reflection;
using System.IO;

namespace SystemsAnalysis.EMGAATS.CrossSectionEditor
{
  public partial class frmXSectEditor : Form
  {
    private bool setLeftStation, setRightStation = false;

    public frmXSectEditor()
    {
      InitializeComponent();
      //chrtXSectDisplay.DataBindings.Clear();
      chrtXSectDisplay.FillSceneGraph += new Infragistics.UltraChart.Shared.Events.FillSceneGraphEventHandler(chrtXSectDisplay_FillSceneGraph);

    }

    void chrtXSectDisplay_FillSceneGraph(object sender, Infragistics.UltraChart.Shared.Events.FillSceneGraphEventArgs e)
    {
      if (e.SceneGraph.Count > 2)
        chrtXSectDisplay.InvalidDataReceived += new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(chrtXSectDisplay_InvalidDataReceived);
    }

    void chrtXSectDisplay_InvalidDataReceived(object sender, Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventArgs e)
    {
      e.LabelStyle.FontColor = Color.Blue;
      e.LabelStyle.FontSizeBestFit = false;
      e.LabelStyle.HorizontalAlign = StringAlignment.Center;
      e.LabelStyle.Font = new Font("Verdana", 12);
      e.Text = chrtXSectDisplay.EmptyChartText;      
      chrtXSectDisplay.InvalidDataReceived -= new Infragistics.UltraChart.Shared.Events.ChartDataInvalidEventHandler(chrtXSectDisplay_InvalidDataReceived);
    }



    //void chrtXSectDisplay_ValidDataReceived(

    private void ConfigureChart()
    {
      chrtXSectDisplay.DataSource = processedXSectDS.ChartTable;
      chrtXSectDisplay.ScatterChart.ColumnX = 1;
      chrtXSectDisplay.ScatterChart.ColumnY = 2;
      chrtXSectDisplay.ScatterChart.GroupByColumn = 0;
      chrtXSectDisplay.ScatterChart.UseGroupByColumn = true;

      this.bindingSource1.CurrentChanged += new System.EventHandler(this.bindingSource1_CurrentChanged);
    }

    private bool ValidateRawXml(string rawXml)
    {
      chrtXSectDisplay.EmptyChartText = "Validating xml...";      

      Stream xsdStream = System.Reflection.Assembly.
                      GetExecutingAssembly().GetManifestResourceStream("SystemsAnalysis.EMGAATS.CrossSectionEditor.LandXML-1.2.xsd");

      XmlSchema rawSchema;
      rawSchema = XmlSchema.Read(xsdStream, null);

      XmlSchemaSet schemaSet = new XmlSchemaSet();
      schemaSet.Add(rawSchema);

      XmlReaderSettings settings = new XmlReaderSettings();
      settings.Schemas.Add(schemaSet);
      settings.ValidationType = ValidationType.Schema;
      settings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;

      XmlReader reader = XmlReader.Create(rawXml, settings);

      try
      {
        XmlDocument doc = new XmlDocument();
        doc.Load(reader);
        if (doc.DocumentElement.Name != "LandXML")
          return false;

        while (reader.Read())
        {
        }
      }
      catch
      {
        return false;
      }
      finally
      {
        chrtXSectDisplay.EmptyChartText = "Error reading LandXML File";
        reader.Close();
      }
      return true;
    }

    private bool LoadRawXml(string rawXml)
    {
      chrtXSectDisplay.EmptyChartText = "Loading xml...";
      XmlReader reader = XmlReader.Create(openFileDialog.FileName);
      bool loaded = false;

      //landXmlDoc.Schemas.Add(
      while (reader.ReadToFollowing("CrossSect"))
      {
        if (reader.Name != "CrossSect")
          continue;

        ProcessedXSectDataSet.XSectsRow xSectsRow =
            processedXSectDS.XSects.NewXSectsRow();

        xSectsRow.Description = "";
        xSectsRow.LongName = reader["name"];
        xSectsRow.Station = Double.Parse(reader["sta"]);

        reader.ReadToFollowing("CrossSectSurf");
        xSectsRow.XSectName = reader["name"];
        xSectsRow.Description = reader["desc"];
        reader.ReadToFollowing("PntList2D");
        reader.Read();
        string[] pointList = reader.Value.Split(' ');

        xSectsRow.LeftOverbankStation = Double.MaxValue;
        xSectsRow.RightOverbankStation = Double.MinValue;
        processedXSectDS.XSects.AddXSectsRow(xSectsRow);

        for (int i = 0; i < pointList.Length; i += 2)
        {
          loaded = true;
          ProcessedXSectDataSet.PointListRow pointListRow =
            processedXSectDS.PointList.NewPointListRow();

          pointListRow.Station = Double.Parse(pointList[i]);
          pointListRow.Elevation = Double.Parse(pointList[i + 1]);
          if (xSectsRow.LeftOverbankStation > pointListRow.Station)
            xSectsRow.LeftOverbankStation = pointListRow.Station;
          if (xSectsRow.RightOverbankStation < pointListRow.Station)
            xSectsRow.RightOverbankStation = pointListRow.Station;

          pointListRow.SetParentRow(xSectsRow);
          processedXSectDS.PointList.AddPointListRow(pointListRow);
        }

      }

      this.bindingSource1.CurrentChanged += new System.EventHandler(this.bindingSource1_CurrentChanged);
      reader.Close();
      return loaded;
    }

    private void LoadXSect(string xSectName)
    {
      processedXSectDS.ChartTable.Clear();

      ProcessedXSectDataSet.XSectsRow xSectRow
        = processedXSectDS.XSects.FindByXSectName(xSectName);

      double minElev = Double.MaxValue;
      double maxElev = Double.MinValue;

      foreach (ProcessedXSectDataSet.PointListRow pointRow in xSectRow.GetPointListRows())
      {
        processedXSectDS.ChartTable.AddChartTableRow("Cross-Section", pointRow.Station, pointRow.Elevation);

        if (minElev > pointRow.Elevation)
          minElev = pointRow.Elevation;
        if (maxElev < pointRow.Elevation)
          maxElev = pointRow.Elevation;
      }

      processedXSectDS.ChartTable.AddChartTableRow("Left Overbank Station", xSectRow.LeftOverbankStation, minElev);
      processedXSectDS.ChartTable.AddChartTableRow("Left Overbank Station", xSectRow.LeftOverbankStation, maxElev);
      processedXSectDS.ChartTable.AddChartTableRow("Right Overbank Station", xSectRow.RightOverbankStation, minElev);
      processedXSectDS.ChartTable.AddChartTableRow("Right Overbank Station", xSectRow.RightOverbankStation, maxElev);

      chrtXSectDisplay.TitleTop.Text = "Cross-Section " + xSectName + " (Longitudinal Station: " + xSectRow.LongName + ")";
    }

    void ValidationCallBacKHandler(object sender, ValidationEventArgs e)
    {
      chrtXSectDisplay.DataSource = null;
      chrtXSectDisplay.EmptyChartText += "\nError: " + e.Message;
    }

    private void frmXSectEditor_Load(object sender, EventArgs e)
    {

    }

    private void bindingSource1_CurrentChanged(object sender, EventArgs e)
    {
      if (bindingSource1.Count > 0)
      {
        ProcessedXSectDataSet.XSectsRow xSectsRow = SelectedXSectsRow;
        LoadXSect(xSectsRow.XSectName);
      }
    }

    private ProcessedXSectDataSet.XSectsRow SelectedXSectsRow
    {
      get
      {
        if (bindingSource1.Current == null)
          return null;
        else
          return (ProcessedXSectDataSet.XSectsRow)((System.Data.DataRowView)bindingSource1.Current).Row;
      }
    }

    private void chrtXSectDisplay_ChartDataClicked(object sender, Infragistics.UltraChart.Shared.Events.ChartDataEventArgs e)
    {
      ProcessedXSectDataSet.XSectsRow xSectRow = SelectedXSectsRow;
      if (setLeftStation)
      {
        if (e.DataValue < xSectRow.RightOverbankStation)
          xSectRow.LeftOverbankStation = e.DataValue;
        else
          MessageBox.Show("Left station must be on left side of channel.");
      }


      if (setRightStation)
      {
        if (e.DataValue > xSectRow.LeftOverbankStation)
          xSectRow.RightOverbankStation = e.DataValue;
        else
          MessageBox.Show("Right station must be on right side of channel.");
      }

      setLeftStation = false;
      setRightStation = false;
      this.LoadXSect(xSectRow.XSectName);
      Cursor = Cursors.Default;
    }

    private void txtLeftStation_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
    {
      setLeftStation = !setLeftStation;
      setRightStation = false;
      if (!setLeftStation)
      {
        Cursor = Cursors.Default;
        return;
      }
      Cursor = Cursors.Cross;

    }

    private void txtRightStation_EditorButtonClick(object sender, Infragistics.Win.UltraWinEditors.EditorButtonEventArgs e)
    {
      setRightStation = !setRightStation;
      setLeftStation = false;
      if (!setRightStation)
      {
        Cursor = Cursors.Default;
        return;
      }
      Cursor = Cursors.Cross;


    }

    private void optStationOrder_ValueChanged(object sender, EventArgs e)
    {
      SwapLeftAndRight();
    }

    private void SwapLeftAndRight()
    {
      var pointsQuery = from points in processedXSectDS.PointList select points;
      foreach (ProcessedXSectDataSet.PointListRow row in pointsQuery)
      {
        row.Station = row.Station * -1;
      }

      var xSectsQuery = from xsects in processedXSectDS.XSects select xsects;
      foreach (ProcessedXSectDataSet.XSectsRow row in xSectsQuery)
      {
        double temp = row.LeftOverbankStation;
        row.LeftOverbankStation = row.RightOverbankStation * -1;
        row.RightOverbankStation = temp * -1;
      }

      LoadXSect(SelectedXSectsRow.XSectName);
      return;
    }

    private void chkReadyForExport_CheckedValueChanged(object sender, EventArgs e)
    {
      if (chkReadyForExport.Checked)
      {
        //bindingSource1.MoveNext();
      }
    }

    private void bindingSource1_BindingComplete(object sender, BindingCompleteEventArgs e)
    {
      if (e.BindingCompleteContext ==
        BindingCompleteContext.DataSourceUpdate && e.Exception == null)

        e.Binding.BindingManagerBase.EndCurrentEdit();
    }

    private void ultraToolbarsManager1_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
    {

      switch (e.Tool.Key)
      {
        case "btnSaveProcessedData":    // ButtonTool
          SaveProcessedData();
          break;

        case "btnSendToMaster":    // ButtonTool
          ExportToMaster();
          break;

        case "btnLoadProcessedData":    // ButtonTool
          LoadProcessedData();
          break;

        case "btnLoadRawXSect":    // ButtonTool
          LoadRawData();
          break;

        case "btnExit":    // ButtonTool
          Application.Exit();
          break;
      }

      return;
    }

    private void SaveProcessedData()
    {
      if (saveFileDialog.ShowDialog() != DialogResult.OK)
        return;

      processedXSectDS.WriteXml(saveFileDialog.FileName);
    }

    private void LoadProcessedData()
    {
      if (openFileDialog.ShowDialog() != DialogResult.OK)
        return;

      chrtXSectDisplay.DataBindings.Clear();
      processedXSectDS.ReadXml(openFileDialog.FileName);
      if (processedXSectDS.XSects.Count > 0)
        LoadXSect(processedXSectDS.XSects[0].XSectName);
      else
        chrtXSectDisplay.EmptyChartText = "Error loading processed xml file";

      ConfigureChart();
    }

    private void ExportToMaster()
    {
    }

    private void LoadRawData()
    {
      chrtXSectDisplay.DataBindings.Clear();
      if (openFileDialog.ShowDialog() != DialogResult.OK)
        return;

      processedXSectDS.Clear();
      if (!ValidateRawXml(openFileDialog.FileName))
        return;

      if (!LoadRawXml(openFileDialog.FileName))
        return;

      LoadXSect(processedXSectDS.XSects[0].XSectName);

      ConfigureChart();
    }


  }
}

