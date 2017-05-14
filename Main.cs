using System;
using System.Drawing;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using CsvHelper;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;
using Excel;
using MathNet.Numerics.LinearRegression;
using FileHelpers;
using FileHelpers.DataLink;
using MathNet.Numerics;
using GeotrackerShapefileMaker.Internals;

namespace GeotrackerShapefileMaker
{
    public partial class Main : Form
    {
        public string savepath;
        QueryBuilder query = new QueryBuilder();
        SaveFileDialog save = new SaveFileDialog();
        EDF edf = new EDF();
        List<Request> requests = new List<Request>();
        public int i = 0;


        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {




            cBox_Basins.DataSource = query.gis_names;
            cBox_Contams.DataSource = query.contaminatesLong;

            cBox_TimeFrame.DataSource = query.timeframes;
            cBox_Selection.DataSource = query.mytypes;
            save.Filter = "Shapefiles (*.shp)|*.shp";
            //save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            lbl_Status.ForeColor = Color.Green;
            cBox_Basins.SelectedIndex = 83;
            cBox_Contams.SelectedIndex = 175;
            cBox_TimeFrame.SelectedIndex = 3;
            cBox_Selection.SelectedIndex = 2;

            ExcelStorage provider = new ExcelStorage(typeof(Request));

            provider.StartRow = 2;
            provider.StartColumn = 1;

            provider.FileName = @"C:\Users\Arnold\Desktop\query.xls";
            Request[] maps = (Request[])provider.ExtractRecords();
            requests = maps.ToList();

        }

        private void btn_Run_Click(object sender, EventArgs e)
        {



            if (i == requests.Count) this.Close();
            query.gis_name = requests[i].BASIN;
            query.mytype = requests[i].CONTAM;
            query.timeframe = cBox_TimeFrame.Text;
            query.parlable1 = query.parlable1s[query.contaminatesLong.IndexOf(requests[i].CONTAM)];
            lbl_Status.ForeColor = Color.Red;

            lbl_Status.Text = "Working On " + query.gis_name + " " + query.parlable1;
            edf.mcl = ExtractNum(query.levels[query.contaminatesLong.IndexOf(requests[i].CONTAM)]);
            edf.savepath = @"C:\Users\Arnold\Desktop\Groundwater\County data\" + query.gis_name + " " + query.parlable1 + ".shp";

            Bground_Main.RunWorkerAsync();








        }

        private void Bground_Main_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {

            edf.GetInputs2(@"C:\Users\Arnold\Desktop\Groundwater\County data\gama_edf_losangeles.txt", query.parlable1);
        }

        private void Bground_Main_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            //save.ShowDialog();
            edf.GetShapefile(save.FileName);
            lbl_Status.ForeColor = Color.Green;
            File.Delete("Resource/subjectfile.zip");
            File.Delete(query.result);
            lbl_Status.Text = "Complete";
            edf = new EDF();
            i++;
            btn_Run.PerformClick();



        }



        private void cBox_Selection_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cBox_Selection.Text == "all") lbl_selection.Text = "≥ 0";
            if (cBox_Selection.Text == "detections") lbl_selection.Text = "> 0";
            if (cBox_Selection.Text == "mcl")
            {
                lbl_selection.Text = "> " + query.levels[query.contaminatesLong.IndexOf(cBox_Contams.Text)];
            }
            edf.mcl = ExtractNum(query.levels[query.contaminatesLong.IndexOf(cBox_Contams.Text)]);
        }

        private void cBox_Contams_SelectedIndexChanged(object sender, EventArgs e)
        {

            cBox_Selection_SelectedIndexChanged(sender, e);

        }

        public double ExtractNum(string value)
        {
            string b = string.Empty;
            double result;

            for (int i = 0; i < value.Length; i++)
            {
                if (Char.IsDigit(value[i]) || value[i] == '.')
                    b += value[i];
            }

            if (b.Length > 0)
                return result = double.Parse(b);
            else return -9999;

        }

    }



    [DelimitedRecord("|")]
    public class EDFSample
    {
        public string ID;
        public string NAME;
        public string LATITUDE;
        public string LONGITUDE;
        public string CHEMICAL;
        public string QUALIFIER;
        public string RESULT;
        public string UNITS;
        public string DATE;
        public int DateInt
        {
            get { return int.Parse(DATE); }
        }

        public double ResultDouble
        {
            get
            {
                double result;
                double.TryParse(RESULT, out result);


                return result;
            }
        }
        public string DATASET_CAT;
        public string DATASET;
        public string COUNTY;
        public string RB;
        public string GW_BASIN_NAME;
        public string ASSEMBLY;
        public string SENATE;

    }

    /// <summary>
    /// Basic Well class containing multiple samples
    /// </summary>
    public class Well
    {
        public string ID;
        public List<string> Names;
        public List<string> Lat;
        public List<string> Lon;
        public double MCL;
        public string Units;

        public List<double> Results
        {
            get
            {
                return Latest.Select(sample => sample.ResultDouble).ToList();
            }
        }
        public double ScaleSize;

        public double ScaleTrend;

        public List<double> LatDouble
        {
            get
            {
                return Lat.Select(double.Parse).ToList();
            }
        }

        public List<double> LonDouble
        {
            get
            {
                return Lon.Select(double.Parse).ToList();
            }
        }
        public string Contaminate;
        public List<EDFSample> Samples;
        public List<List<EDFSample>> SampsByName;
        public List<double> R2
        {
            get
            {
                return SampsByName.Select(GetR2).ToList();
            }
        }
        public List<double> Trends
        {
            get { return SampsByName.Select(GetTrend).ToList(); }
        }

        public List<string> Trending
        {
            get
            {
                List<string> result = new List<string>();
                string half = (MCL / 2).ToString("#.##");
                string full = MCL.ToString("#.##");
                for (int i = 0; i < SampsByName.Count; i++)
                {

                    if (Results[i] > MCL && Trends[i] > 0) result.Add("Worsening, Above Critical Level(>" + full + " " + Units + ")");
                    else if (Results[i] >= (MCL / 2) && Results[i] <= MCL && Trends[i] > 0) result.Add("Worsening, Near Critical Level(" + half + "-" + full + " " + Units + ")");
                    else if (Results[i] < (MCL / 2) && Trends[i] > 0) result.Add("Worsening, Below Critical Level(<" + half + " " + Units + ")");


                    else if (Results[i] > MCL && Trends[i] < 0) result.Add("Improving, Above Critical Level(>" + full + " " + Units + ")");
                    else if (Results[i] >= (MCL / 2) && Results[i] <= MCL && Trends[i] < 0) result.Add("Improving, Near Critical Level(" + half + "-" + full + " " + Units + ")");
                    else if (Results[i] < (MCL / 2) && Trends[i] < 0) result.Add("Improving, Below Critical Level(<" + half + " " + Units + ")");





                    else if (Results[i] >= MCL) result.Add("Stable, Above Critical Level(>" + full + " " + Units + ")");
                    else if (Results[i] < (MCL / 2)) result.Add("Stable, Below Critical Level(<" + half + " " + Units + ")");
                    else if (Results[i] >= (MCL / 2) && Results[i] < MCL) result.Add("Stable, Near Critical Level(" + half + "-" + full + " " + Units + ")");
                    else result.Add("error");
                }
                return result;
            }
        }


        public List<EDFSample> Latest
        {
            get
            {
                return SampsByName.Select(GetLast).ToList();
            }
        }

        public List<int> SampsNumbers
        {
            get
            {
                return SampsByName.Select(t => t.Count).ToList();
            }
        }

        public List<string> RecentSampleDate
        {
            get
            {
                return Latest.Select(sample => epoch2string(sample.DateInt)).ToList();

            }
        }
        public List<string> ChartURL
        {
            get
            {
                var result = new List<string>();

                for (var i = 0; i < Latest.Count; i++)
                {
                    result.Add("http://geotracker.waterboards.ca.gov/gama/gamamap/public/linechart.asp?dataset=EDF&global_id=" +
                        ID + "&locid=" + Names[i] + "&parlabel=" + Contaminate);
                }
                return result;
            }

        }

        public int TopWellIndex;


        public int TopTrendIndex;


        public int TopNumIndex;



        public Well() { }

        public Well(EDFSample initialSample)
        {

            ID = initialSample.ID;
            Contaminate = initialSample.CHEMICAL;
            Lat = new List<string>();
            Lon = new List<string>();
            Units = initialSample.UNITS;
            Samples = new List<EDFSample>() { initialSample };

        }

        public void AddSample(EDFSample s)
        {
            Samples.Add(s);
        }

        public void SortSamples()
        {
            SampsByName = new List<List<EDFSample>>();
            Names = new List<string>();
            int index = -1;
            for (int i = 0; i < Samples.Count; i++)
            {
                if (Names.Contains(Samples[i].NAME))
                {
                    index = Names.IndexOf(Samples[i].NAME);
                    SampsByName[index].Add(Samples[i]);
                }
                else
                {
                    var holder = new List<EDFSample>();
                    holder.Add(Samples[i]);
                    SampsByName.Add(holder);
                    Names.Add(Samples[i].NAME);
                }
            }

            for (int i = 0; i < SampsByName.Count; i++)
            {

                SampsByName[i].Sort((x, y) => x.DateInt.CompareTo(y.DateInt));
                SampsByName[i].Reverse();
                Lat.Add(SampsByName[i][0].LATITUDE);
                Lon.Add(SampsByName[i][0].LONGITUDE);
                Results.Add(SampsByName[i][0].ResultDouble);
                Lat.Add(SampsByName[i][0].LATITUDE);

            }


            GetTopTrendIndex();
            GetTopWellIndex();
        }

        private string epoch2string(int epoch)
        {
            return new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddDays(epoch).ToShortDateString();
        }

        public double GetR2(List<EDFSample> samples)
        {
            if (samples.Count <= 1) return 0;
            double[] Time = new double[samples.Count];
            double[] Amount = new double[samples.Count];

            int counter = 0;
            foreach (EDFSample item in samples)
            {
                Time[counter] = int.Parse(item.DATE);
                Amount[counter] = double.Parse(item.RESULT);
                counter++;
            }
            Tuple<double, double> t = SimpleRegression.Fit(Time, Amount);
            double a = t.Item2;
            double b = t.Item1;
            double[] model = new double[Time.Length];
            for (int x = 0; x < Time.Count(); x++)
            {
                model[x] = (Time[x] * a) + b;
            }

            return GoodnessOfFit.RSquared(model, Amount);
        }

        public double GetTrend(List<EDFSample> samples)
        {
            double[] Time = new double[samples.Count];
            double[] Amount = new double[samples.Count];

            int counter = 0;
            foreach (EDFSample item in samples)
            {
                Time[counter] = int.Parse(item.DATE);
                Amount[counter] = double.Parse(item.RESULT);
                counter++;
            }
            if (Time.Length > 1)
            {
                double result = SimpleRegression.Fit(Time, Amount).Item2;

                return result;
            }
            else
            {

                return 0;

            }

        }



        public EDFSample GetLast(List<EDFSample> samples)
        {


            return samples[0];

        }

        public void GetTopWellIndex()
        {

            int holder = 0;
            for (int i = 0; i < Results.Count; i++)
            {
                if (Results[i] > Results[holder]) holder = i;
            }
            TopWellIndex = holder;

        }

        public void GetTopTrendIndex()
        {

            int holder = 0;
            for (int i = 0; i < Results.Count; i++)
            {
                if (Trends[i] > Trends[holder]) holder = i;
            }
            TopTrendIndex = holder;

        }

        public int GetTopTrendIndex(int minSamples)
        {

            int holder = 0;

            if (SampsNumbers.Max() >= minSamples)
            {
                for (int i = 0; i < Results.Count; i++)
                {
                    if (SampsNumbers[i] < minSamples) continue;
                    if (Trends[i] > Trends[holder]) holder = i;
                }
                return holder;
            }
            else
            {
                for (int i = 0; i < Results.Count; i++)
                {
                    if (Trends[i] > Trends[holder]) holder = i;
                }
                return holder;
            }

        }


        public void GetTopNumIndex()
        {
            int holder = -1;
            for (int i = 0; i < Results.Count; i++)
            {
                if (SampsNumbers[i] > SampsNumbers[holder]) holder = i;

            }

        }
    }

    public class EDF
    {
        public List<Well> WellList;
        public List<string> WellIDs;
        public List<EDFSample> SampleList;
        public string ContaminateRequest;

        public string filepath;
        public string savepath;

        public List<string> ListedContaminants;
        public List<string> ListedUnits;
        public List<int> ListedYears;
        public int c;

        public double mcl;


        public EDF()
        {
            WellList = new List<Well>();

            WellIDs = new List<string>();

        }

        public EDF(string inpath)
        {
            WellList = new List<Well>();

            WellIDs = new List<string>();
            GetInputs(inpath);
        }

        public void GetInputs(string inpath)
        {
            filepath = inpath;
            SampleList = GetEDFsamples(inpath);
            Samples2Wells();

        }

        public void GetInputs2(string inpath, string request)
        {
            filepath = inpath;
            SampleList = GetEDFsamples2(inpath, request);
            Samples2Wells();

        }


        public void Samples2Wells()
        {
            int counter = 0;
            while (SampleList.Any())
            {
                if (SampleList[0].RESULT != null)
                {

                    if (WellIDs.Contains(SampleList[0].ID))
                    {
                        WellList.Find(i => i.ID == SampleList[0].ID).Samples.Add(SampleList[0]);
                    }
                    else
                    {
                        Well w = new Well(SampleList[0]);
                        w.MCL = this.mcl;
                        WellList.Add(w);
                        WellIDs.Add(w.ID);
                        counter++;
                    }


                }
                SampleList.RemoveAt(0);
            }

            foreach (Well well in WellList)
            {
                well.SortSamples();
            }
        }



        public List<EDFSample> GetEDFsamples(string path)
        {
            ExcelStorage provider = new ExcelStorage(typeof(EDFSample));

            provider.StartRow = 2;
            provider.StartColumn = 1;

            provider.FileName = path;
            EDFSample[] edf = (EDFSample[])provider.ExtractRecords();
            return edf.ToList();
        }

        public List<EDFSample> GetEDFsamples2(string path, string contam)
        {
            StreamReader reader = new StreamReader(path);
            List<EDFSample> result = new List<EDFSample>();
            reader.ReadLine();
           
            while (!reader.EndOfStream)
            {
                string lineblock = reader.ReadLine();
                string[] line = lineblock.Split('\t');
                if (line[3] == contam)
                {
                    EDFSample x = new EDFSample();

                    x.NAME = line[0];
                    x.LATITUDE = line[1];
                    x.LONGITUDE = line[2];
                    x.CHEMICAL = line[3];
                    x.QUALIFIER = line[4];
                    x.RESULT = line[5];
                    x.UNITS = line[6];
                    x.DATE = line[7];
                    x.DATASET_CAT = line[8];
                    x.DATASET = line[9];
                    x.COUNTY = line[10];
                    x.RB = line[11];
                    x.GW_BASIN_NAME = line[12];
                    x.ASSEMBLY = line[13];
                    x.SENATE = line[14];

                }
                Console.WriteLine(c);
            }

            return result;
        }




        public void GetShapefile(string filepath)
        {


            FeatureSet fs = new FeatureSet(FeatureType.Point);
            fs.DataTable.Columns.Add(new DataColumn("ID", typeof(string)));
            fs.DataTable.Columns.Add(new DataColumn("Name", typeof(string)));
            fs.DataTable.Columns.Add(new DataColumn("Lat", typeof(double)));
            fs.DataTable.Columns.Add(new DataColumn("Lon", typeof(double)));
            fs.DataTable.Columns.Add(new DataColumn("Contam", typeof(string)));
            fs.DataTable.Columns.Add(new DataColumn("Trend", typeof(double)));
            fs.DataTable.Columns.Add(new DataColumn("Trending", typeof(string)));
            fs.DataTable.Columns.Add(new DataColumn("Rsquared", typeof(double)));
            fs.DataTable.Columns.Add(new DataColumn("MCL", typeof(double)));
            fs.DataTable.Columns.Add(new DataColumn("Units", typeof(string)));
            fs.DataTable.Columns.Add(new DataColumn("NumSamp", typeof(int)));
            fs.DataTable.Columns.Add(new DataColumn("LastDate", typeof(string)));
            fs.DataTable.Columns.Add(new DataColumn("Result", typeof(double)));
            fs.DataTable.Columns.Add(new DataColumn("Link", typeof(string)));




            for (int a = 0; a < WellList.Count; a++)
            {

                int c = WellList[a].GetTopTrendIndex(3);
                Coordinate vertices = new Coordinate();
                vertices.X = WellList[a].LonDouble[c];
                vertices.Y = WellList[a].LatDouble[c];
                DotSpatial.Topology.Point Geom = new DotSpatial.Topology.Point(vertices);
                fs.AddFeature(Geom);
                fs.DataTable.Rows[a].BeginEdit();
                fs.DataTable.Rows[a]["ID"] = WellList[a].ID;
                fs.DataTable.Rows[a]["Name"] = WellList[a].Names[c];
                fs.DataTable.Rows[a]["Lat"] = WellList[a].LatDouble[c];
                fs.DataTable.Rows[a]["Lon"] = WellList[a].LonDouble[c];
                fs.DataTable.Rows[a]["Contam"] = WellList[a].Contaminate;
                fs.DataTable.Rows[a]["Trending"] = WellList[a].Trending[c];
                fs.DataTable.Rows[a]["Trend"] = WellList[a].Trends[c];
                fs.DataTable.Rows[a]["Rsquared"] = WellList[a].R2[c];
                fs.DataTable.Rows[a]["MCL"] = WellList[a].MCL;
                fs.DataTable.Rows[a]["Units"] = WellList[a].Latest[c].UNITS;
                fs.DataTable.Rows[a]["NumSamp"] = WellList[a].SampsByName[c].Count;
                fs.DataTable.Rows[a]["LastDate"] = WellList[a].RecentSampleDate[c];
                fs.DataTable.Rows[a]["Result"] = WellList[a].Results[c];
                fs.DataTable.Rows[a]["Link"] = WellList[a].ChartURL[c];
                fs.DataTable.Rows[a].EndEdit();
            }

            fs.Projection =
                KnownCoordinateSystems.Geographic.NorthAmerica.NorthAmericanDatum1983;

            fs.SaveAs(savepath, true);

            //IFeatureSet buff = new FeatureSet();
            //buff = fs.Buffer(.003, true);
            //string filepathB = filepath.Replace(".shp", "buffer.shp");
            //buff.Projection =
            //KnownCoordinateSystems.Geographic.NorthAmerica.NorthAmericanDatum1983;
            //buff.SaveAs(filepathB, true);
        }
    }



    public class QueryBuilder
    {
        //http://geotracker.waterboards.ca.gov/gama/gamamap/export_data.asp?parlabel1=NO3&timeframe=10YR&mytype=all&dataset=EDF,&gis_layer=GW_BASIN&gis_name=UPPER%20SANTA%20ANA%20VALLEY%20-%20CHINO%20(8-2.01)
        public string basestring = "http://geotracker.waterboards.ca.gov/gama/gamamap/export_data.asp?";
        public string result;

        public string parlable1 = "NO3";
        public string timeframe = "10YR";
        public string mytype = "all";
        public string dataset = "EDF";
        public string gis_layer = "GW_BASIN";
        public string gis_name = "COASTAL PLAIN OF LOS ANGELES - CENTRAL (4-11.04)";




        public List<string> gis_names;
        public List<string> parlable1s;
        public List<string> contaminatesLong;
        public List<string> filestore;
        public List<string> levels;
        public string[] timeframes = { "ALL", "1YR", "3YR", "10YR" };
        public string[] mytypes = { "all", "mcl", "detections" };

        public string cmd
        {
            get
            {
                return CommandBuilder();
            }
        }




        public QueryBuilder()
        {
            gis_names = new List<string>();
            levels = new List<string>();
            contaminatesLong = new List<string>();
            parlable1s = new List<string>();
            filestore = new List<string>();
            GetBasins();
            GetContams();

        }


        public void GetBasins()
        {
            StreamReader reader = new StreamReader("Resource/basinlist.csv");
            var csv = new CsvReader(reader);
            csv.Configuration.HasHeaderRecord = false;
            while (csv.Read())
            {
                gis_names.Add(csv.GetField<string>(0));
            }
            reader.Close();
        }

        public void GetContams()
        {
            StreamReader reader = new StreamReader("Resource/chems.csv");
            var csv = new CsvReader(reader);
            csv.Configuration.HasHeaderRecord = false;
            while (csv.Read())
            {
                parlable1s.Add(csv.GetField<string>(0));
                contaminatesLong.Add(csv.GetField<string>(1));
                levels.Add(csv.GetField<string>(2));
            }
            reader.Close();

        }

        public string CommandBuilder()
        {
            string result = basestring + "parlabel1=" + parlable1 + "&timeframe=" + timeframe + "&mytype=" + mytype + "&dataset=" + dataset + "&gis_layer=" + gis_layer + "&gis_name=" + gis_name.Replace(" ", "%20");
            return result;
        }



        public string GetLongName(string shortname)
        {
            int index = parlable1s.IndexOf(shortname);
            return contaminatesLong[index];
        }

        public string GetShortName(string longname)
        {
            int index = contaminatesLong.IndexOf(longname);
            return parlable1s[index];
        }

        public string DownloadData()
        {

            WebClient wc = new WebClient();

            wc.DownloadFile(cmd, "Resource/subjectfile.zip");

            using (var unzip = new Unzip("Resource/subjectfile.zip"))
            {
                string file = "error";
                // list all files in the archive
                foreach (var fileName in unzip.FileNames)
                {
                    file = fileName;
                }
                // extract single file to a specifie4d location
                unzip.Extract(file, "Resource/" + file);

                return result = "Resource/" + file;

            }
        }





    }

    [DelimitedRecord("|")]
    public class Request
    {
        public string BASIN;
        public string CONTAM;
    }





}
