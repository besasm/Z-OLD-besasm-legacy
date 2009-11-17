using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Utility;

using System;
using System.Collections.Generic;
using System.Text;
using SystemsAnalysis.Modeling;
using System.Collections;
using System.Collections.Specialized;
using SystemsAnalysis.Utils.Events;
using SystemsAnalysis.Types;
using SystemsAnalysis.DataAccess;

namespace SystemsAnalysis.Reporting.ReportLibraries
{
    public class FecReport : ReportBase
    {
        private Dictionary<int, FlowEstimationCatchment> fecs;
        private Dictionary<int, Links> _fecLinks;
        private Dictionary<int, Dscs> _fecDscs;
        private Dictionary<int, Nodes> _fecNodes;
        private Dictionary<int, ModelResultsReport> _fecResultsReports;
        private Dictionary<int, ModelReport> _fecReports;        

        private Links links;
        private Nodes nodes;
        private Dscs dscs;

        /// <summary>
        /// Creates a FecReport from a collections of Links, Nodes and Dscs
        /// </summary>
        /// <param name="links">A Links collection to report on</param>
        /// <param name="nodes">A Nodes collection to report on</param>
        /// <param name="dscs">A Dscs collection to report on</param>
        public FecReport(Links links, Nodes nodes, Dscs dscs)
        //: base(links, nodes, dscs)
        {
            this.OnStatusChanged(new StatusChangedArgs("Creating flow estimation catchment objects."));
            this.links = links;
            this.nodes = nodes;
            this.dscs = dscs;
            fecs = FlowEstimationCatchment.GetFecsFromLinks(links);
            //fecs = new Dictionary<int, FlowEstimationCatchment>();

        }

        private Dictionary<int, Links> FecLinks
        {
            [System.Diagnostics.DebuggerStepThroughAttribute]
            get
            {
                if (this._fecLinks == null)
                {
                    this.InitFecLinks();
                }
                return this._fecLinks;
            }
        }
        private void InitFecLinks()
        {
            this.OnStatusChanged(new StatusChangedArgs("Associating Links with Flow Estimation Catchments."));
            _fecLinks = new Dictionary<int, Links>();
            Links fecStartLinks = new Links();
            foreach (FlowEstimationCatchment fec in fecs.Values)
            {
                if (fec.FEMethod == Enumerators.FlowEstimationMethods.NS)
                {
                    continue;
                }
                fecStartLinks.Add(links.FindByMLinkID(fec.MLinkID));
            }
            foreach (FlowEstimationCatchment fec in fecs.Values)
            {
                if (fec.FEMethod == Enumerators.FlowEstimationMethods.NS)
                {
                    _fecLinks.Add(fec.FecID, new Links());
                    continue;
                }

                Link startLink = links.FindByMLinkID(fec.MLinkID);
                Links startLinks = new Links();
                if (startLink == null)
                {
                    this.OnStatusChanged(new StatusChangedArgs("MLinkID missing for FEC " + fec.FecName + ".", StatusChangeType.Error));
                    continue;
                }
                startLinks.Add(startLink);


                Links stopLinks = new Links();
                foreach (Link l in fecStartLinks.Values)
                {
                    if (l.MLinkID != startLink.MLinkID)
                    {
                        stopLinks.Add(l);
                    }
                }
                foreach (int i in fec.StopLinks)
                {
                    Link l = links.FindByMLinkID(i);
                    if (l == null)
                    {
                        this.OnStatusChanged(new StatusChangedArgs("MLinkID missing for FEC " + fec.FecName + ".", StatusChangeType.Error));
                        continue;
                    }
                    stopLinks.Add(l);
                }

                Links fecLinks;
                fecLinks = links.Trace(startLinks, stopLinks);

                /* The stop link of a downstream catchment will
                * be the start link of an upstream catchment.  This link
                * would be double counted, as will all connected Dscs. Since 
                * we assign flow from connected Dscs to the upstream node of 
                * link we will assign the overlapping link to the upstream catchment.
                * Therefore remove any stop links from fecLinks collection. */
                foreach (Link l in stopLinks.Values)
                {
                    if (fecLinks.Contains(l.LinkID) && !fec.StopLinks.Contains(l.MLinkID))
                    {
                        fecLinks.Remove(l.LinkID);
                    }
                }

                _fecLinks.Add(fec.FecID, fecLinks);
                fecStartLinks.Add(startLink);
            }
            //Debug, write out links to text file
            /*System.IO.StreamWriter tw = new System.IO.StreamWriter(@"D:\temp\linkqc", false);            
            foreach (Link l in links.Values)
            {
            tw.WriteLine("basin: " + l.MLinkID);
            }            
            foreach (KeyValuePair<int, Links> kvp in _fecLinks)
            {
            Links fecLinks = kvp.Value;

            foreach (Link l in fecLinks.Values)
            {
            tw.WriteLine(kvp.Key.ToString() + ":" + l.MLinkID);
            }
            }
            tw.Close();*/
        }
        private Dictionary<int, Nodes> FecNodes
        {
            [System.Diagnostics.DebuggerStepThroughAttribute]
            get
            {
                if (this._fecNodes == null)
                {
                    this.InitFecNodes();
                }
                return this._fecNodes;
            }
        }
        private void InitFecNodes()
        {
            this.OnStatusChanged(new StatusChangedArgs("Associating Nodes with Flow Estimation Catchments."));
            _fecNodes = new Dictionary<int, Nodes>();
            foreach (FlowEstimationCatchment fec in this.fecs.Values)
            {
                Nodes n = new Nodes();
                foreach (Link l in FecLinks[fec.FecID].Values)
                {
                    if (nodes.Contains(l.USNodeName) && !n.Contains(l.USNodeName))
                    {
                        n.Add(nodes[l.USNodeName]);
                    }
                }
                _fecNodes.Add(fec.FecID, n);
            }

        }
        private Dictionary<int, Dscs> FecDscs
        {
            [System.Diagnostics.DebuggerStepThroughAttribute]
            get
            {
                if (this._fecDscs == null)
                {
                    this.InitFecDscs();
                }
                return this._fecDscs;
            }
        }
        private void InitFecDscs()
        {
            this.OnStatusChanged(new StatusChangedArgs("Associating Dscs with Flow Estimation Catchments."));
            _fecDscs = new Dictionary<int, Dscs>();
            foreach (FlowEstimationCatchment fec in fecs.Values)
            {
                _fecDscs.Add(fec.FecID, dscs.Select((Links)FecLinks[fec.FecID]));
            }
        }
        private Dictionary<int, ModelResultsReport> FecResultsReports
        {
            get
            {
                if (_fecResultsReports == null) { InitFecResultsReports(); }
                return _fecResultsReports;
            }
        }
        private void InitFecResultsReports()
        {
            _fecResultsReports = new Dictionary<int, ModelResultsReport>();
            foreach (KeyValuePair<int, FlowEstimationCatchment> kvp in this.fecs)
            {
                this.OnStatusChanged(new StatusChangedArgs("Associating Model Results with Flow Estimation Catchment " + kvp.Value.FecName + "."));
                int fecID = kvp.Value.FecID;
                ModelResultsReport mrp = new ModelResultsReport(FecLinks[fecID], FecNodes[fecID], FecDscs[fecID]);
                mrp.StatusChanged += new OnStatusChangedEventHandler(this.OnStatusChanged);
                _fecResultsReports.Add(fecID, mrp);
            }
        }
        private Dictionary<int, ModelReport> FecReports
        {
            get
            {
                if (_fecReports == null) { InitFecReports(); }
                return _fecReports;
            }
        }
        private void InitFecReports()
        {
            _fecReports = new Dictionary<int, ModelReport>();
            foreach (KeyValuePair<int, FlowEstimationCatchment> kvp in this.fecs)
            {
                this.OnStatusChanged(new StatusChangedArgs("Associating Model Report with Flow Estimation Catchment " + kvp.Value.FecName + "."));
                int fecID = kvp.Value.FecID;
                ModelReport mrp = new ModelReport(FecLinks[fecID], FecNodes[fecID], FecDscs[fecID]);
                mrp.StatusChanged += new OnStatusChangedEventHandler(this.OnStatusChanged);
                _fecReports.Add(fecID, mrp);
            }
        }        

        public int[] FecIDList()
        {
            int i = 0;
            int[] fecIDList;
            fecIDList = new int[fecs.Count];
            foreach (FlowEstimationCatchment fec in fecs.Values)
            {
                fecIDList[i] = fec.FecID;
                i++;
            }
            return fecIDList;
        }

        public string Name(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            return fecs[fecID].FecName;
        }
        public string Method(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            return Convert.ToString(fecs[fecID].FEMethod);
        }
        public double DscArea(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            return FecReports[fecID].DscArea(parameters);
        }
        public string DscAreaSummary(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            return FecReports[fecID].DscAreaSummary(parameters);
        }

        public int DscCount(IDictionary<string, Parameter> parameters)
        {

            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            return FecReports[fecID].DscCount(parameters);
        }
        public int PipeCount(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            return FecReports[fecID].PipeCount(parameters);
        }
        /// <summary>
        /// Returns the length of pipe within the specified Fec in miles
        /// </summary>
        /// <param name="parameters">A parameter collection containing the FecID of interest</param>
        /// <returns>The length of pipe within the specified Fec in miles</returns>
        public double PipeLength(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            return FecReports[fecID].PipeLength(parameters);
        }
        public double PipeLengthNonstandardMaterial(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            double length = 0;
            foreach (Link l in FecLinks[fecID].Values)
            {
                int yearBuilt = l.InstallDate.Year;
                double diameter = l.Diameter;
                if (l.IsGravityFlow && l.Material != "CSP" && l.Material != "HDPE" && l.Material != "PVC")
                {
                    length += l.Length;
                }
            }
            return length / 5280.0;
        }
        public string PipeDiamRange(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            Links fecLinks = (Links)FecLinks[fecID];
            if (fecLinks.Count == 0)
            {
                return "N/A";
            }
            return
            Convert.ToString(fecLinks.GetMinPipeDiam()) +
            "\" to " +
            Convert.ToString(fecLinks.GetMaxPipeDiam() + "\"");
        }
        public string PipeInstallDateRange(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            Links fecLinks = (Links)FecLinks[fecID];
            if (fecLinks.Count == 0)
            {
                return "N/A";
            }
            return
            Convert.ToString(fecLinks.GetOldestPipeInstallYear()) +
            " to " +
            Convert.ToString(fecLinks.GetNewestPipeInstallYear());
        }
        /// <summary>
        /// Returns the design manual average base flow generated by Dscs within the specified Fec in cfs
        /// </summary>
        /// <param name="parameters">A parameter collection containing the FecID of interest</param>
        /// <returns>The design manual average base flow generated by Dscs within the specified Fec in cfs</returns>
        public double DMAverageBaseFlow(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            Dscs fecDscs = (Dscs)FecDscs[fecID];
            return fecDscs.DesignManualBaseFlow(Enumerators.TimeFrames.EX);

        }
        /// <summary>
        /// Returns the design manual peak base flow generated by Dscs within the specified Fec in cfs
        /// </summary>
        /// <param name="parameters">A parameter collection containing the FecID of interest</param>
        /// <returns>The design manual peak base flow generated by Dscs within the specified Fec in cfs</returns>
        public double DMPeakBaseFlow(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            Dscs fecDscs = (Dscs)FecDscs[fecID];
            return Math.Pow(fecDscs.DesignManualBaseFlow(Enumerators.TimeFrames.EX), 0.905)*2.65;

        }
        public double MeasuredABF(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            FlowEstimationCatchment fec = fecs[fecID];
            if (fec.FEMethod == Enumerators.FlowEstimationMethods.NS || fec.FEMethod == Enumerators.FlowEstimationMethods.ME)
            {
                throw new Exception("MeasuredABF does not apply to unmonitored FECs");
            }
            return (double)fec.FecResults.MonitoredABF;
        }

        public double MeasuredToDMABFRatio(IDictionary<string, Parameter> parameters)
        {
            return MeasuredABF(parameters) / DMAverageBaseFlow(parameters);
        }
        public double MeasuredPeakBF(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            FlowEstimationCatchment fec = fecs[fecID];
            if (fec.FEMethod == Enumerators.FlowEstimationMethods.NS || fec.FEMethod == Enumerators.FlowEstimationMethods.ME)
            {
                throw new Exception("MeasuredPeakBF does not apply to unmonitored FECs");
            }
            return (double)fec.FecResults.MonitoredPeakBF;
        }
        public double MeasuredToDMPeakBFRatio(IDictionary<string, Parameter> parameters)
        {
            return MeasuredPeakBF(parameters) / DMPeakBaseFlow(parameters);
        }
        /// <summary>
        /// Returns the peak measured RDII from an FEC in cfs. An exception is thrown if
        /// the Fec is unmonitored or unsewered (FEMethod == NS or ME)
        /// </summary>
        /// <param name="parameters">A parameters collection containing the FecID of interest</param>
        /// <returns>The peak measured RDII from an FEC in cfs</returns>
        public double PeakMeasuredRDII(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            FlowEstimationCatchment fec = fecs[fecID];
            if (fec.FEMethod == Enumerators.FlowEstimationMethods.NS || fec.FEMethod == Enumerators.FlowEstimationMethods.ME)
            {
                throw new Exception("PeakMeasuredRDII does not apply to unmonitored FECs");
            }
            return (double)fec.FecResults.EXRDIICfs;
        }
        /// <summary>
        /// Returns the peak measured RDII per acre from an FEC in gpad. An exception is thrown if
        /// the Fec is unmonitored or unsewered (FEMethod == NS or ME)
        /// </summary>
        /// <param name="parameters">A parameters collection containing the FecID of interest</param>
        /// <returns>The peak measured RDII per acre from an FEC in gpad</returns>
        public double PeakMeasuredRDIIPerAcre(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            FlowEstimationCatchment fec = fecs[fecID];
            if (fec.FEMethod == Enumerators.FlowEstimationMethods.NS || fec.FEMethod == Enumerators.FlowEstimationMethods.ME)
            {
                throw new Exception("PeakMeasuredRDIIPerAcre does not apply to unmonitored FECs");
            }
            return (double)fec.FecResults.EXRDIIGpad;
        }

        public double CharacterizationABF(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            Enumerators.TimeFrames timeFrame;
            timeFrame = Enumerators.GetTimeFrameEnum(parameters["TimeFrame"].Value);
            FlowEstimationCatchment fec = fecs[fecID];
            if (timeFrame == Enumerators.TimeFrames.EX)
            {
                return (double)fec.FecResults.MonitoredABF;
            }
            else
            {
                return (double)(fec.FecResults.FUFUABFFactor * fec.FecResults.MonitoredABF + fec.FecResults.FUEXABFFactor * fec.FecResults.MonitoredABF);
            }
        }
        public double CharacterizationPeakBF(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            Enumerators.TimeFrames timeFrame;
            timeFrame = Enumerators.GetTimeFrameEnum(parameters["TimeFrame"].Value);
            FlowEstimationCatchment fec = fecs[fecID];
            if (timeFrame == Enumerators.TimeFrames.EX)
            {
                return (double)fec.FecResults.MonitoredPeakBF;
            }
            else
            {
                return (double)(fec.FecResults.FUFUABFFactor * fec.FecResults.MonitoredPeakBF + fec.FecResults.FUEXABFFactor * fec.FecResults.MonitoredPeakBF);
            }
        }
        public double CharacterizationPeakRDII(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            Enumerators.TimeFrames timeFrame;
            timeFrame = Enumerators.GetTimeFrameEnum(parameters["TimeFrame"].Value);
            FlowEstimationCatchment fec = fecs[fecID];
            if (timeFrame == Enumerators.TimeFrames.EX)
            {
                return (double)fec.FecResults.EXRDIICfs;
            }
            else
            {
                return (double)(fec.FecResults.FUFURDIIFactor * fec.FecResults.EXRDIICfs + fec.FecResults.FUEXRDIIFactor * fec.FecResults.EXRDIICfs);
            }
        }
        public double CharacterizationPeakTotalFlow(IDictionary<string, Parameter> parameters)
        {
            return CharacterizationPeakBF(parameters) + CharacterizationPeakRDII(parameters);
            /*int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            Enumerators.TimeFrames timeFrame;
            timeFrame = Enumerators.GetTimeFrameEnum(parameters["TimeFrame"].Value);
            FlowEstimationCatchment fec = fecs[fecID];
            switch (timeFrame)
            {
            case Enumerators.TimeFrames.EX:
            return CharacterizationPeakBF(parameters) + CharacterizationPeakRDII(parameters);
            case Enumerators.TimeFrames.FU:
            return (double)(fec.FecResults.FUFUABFFactor * fec.FecResults.MonitoredABF + fec.FecResults.FUEXABFFactor * fec.FecResults.MonitoredABF);
            }*/
        }
        public double CharacterizationPercentIncrease(IDictionary<string, Parameter> parameters)
        {
            double existingPeak, futurePeak;
            parameters.Add("TimeFrame", new Parameter("TimeFrame", "EX"));
            existingPeak = CharacterizationPeakTotalFlow(parameters);
            parameters["TimeFrame"] = new Parameter("TimeFrame", "FU");
            futurePeak = CharacterizationPeakTotalFlow(parameters);
            return (futurePeak - existingPeak) / existingPeak * 100;
        }

        public int SurfaceFloodingCount(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            ModelResultsReport mrp = FecResultsReports[fecID];
            return mrp.SurfaceFloodingCount(parameters);
        }
        public string SurfaceFloodingSummary(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            ModelResultsReport mrp = FecResultsReports[fecID];
            return mrp.SurfaceFloodingSummary(parameters);
        }
        public int SewerBackupRiskCount(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            return FecResultsReports[fecID].SewerBackupRiskCount(parameters);
        }
        public string SewerBackupRiskSummary(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            return FecResultsReports[fecID].SewerBackupRiskSummary(parameters);
        }

        public string PipeSurchargeLengthCountSummary(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            //return FecResultsReports[fecID].PipeSurchargeSummary(parameters);
            return FecResultsReports[fecID].PipeSurchargeLengthCountSummary(parameters);            
        }

        public int PipeGradeCount(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            ModelReport modelReport = this._fecReports[fecID];
            return modelReport.PipeGradeCount(parameters);
        }

        public int PipesWithoutGradeCount(IDictionary<string, Parameter> parameters)
        {
            int fecID;
            fecID = parameters["FECID"].ValueAsInt;
            ModelReport modelReport = this._fecReports[fecID];
            return modelReport.PipesWithoutGradeCount(parameters);
        }



    }
}