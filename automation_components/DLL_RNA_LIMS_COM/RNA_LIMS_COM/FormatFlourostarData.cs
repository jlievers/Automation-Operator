using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using CsvHelper;
using CsvHelper.Configuration;
using System.Windows.Forms;

namespace RNA_LIMS_COM
{
    public class FormatFlourostarData
    {
        public class QuantPlate : Plate
        {
            public double R2 { get; set; }
            public double Slope { get; set; }
            public double AvgConcentration { get; set; }
            public int CountBelowThreshold { get; set; }
            public List<QuantData> FormattedJSON { get; set; }

            public QuantPlate(string dataFilePath)
            {
                AddQuantData(dataFilePath);
            }
            public QuantPlate(Plate plate)
            {
                this.Barcode = plate.Barcode;
                this.Type = plate.Type;
                this.Position = plate.Position;
            }

            private void AddQuantData(string dataFilePath)
            {
                //int linesToSkip = 16;    
                int linesToSkip = 18;   //skips first 18 lines
                List<RawQuantData> myRawQuantData = new List<RawQuantData>();
                List<QuantData> myFormattedQuantData = new List<QuantData>();
                try
                {
                    var quantFile = File.ReadAllLines(dataFilePath).ToArray();  //r2 on the 11th line
                    //this.R2 = Math.Round(Convert.ToDouble(quantFile[11].Split(',')[2]), 4);   
                    this.R2 = Math.Round(Convert.ToDouble(quantFile[15].Split(',')[2]), 4);         //updated r2 to 3rd position on the 16th line
                    //this.Slope = Math.Round(Convert.ToDouble(quantFile[9].Split(',')[2]), 0);  
                    this.Slope = Math.Round(Convert.ToDouble(quantFile[10].Split(',')[2]), 0);      //updated slope to 3rd position on the 11th line
                    using (TextReader reader = File.OpenText(dataFilePath))
                    {
                        CsvReader csv = new CsvReader(reader);
                        csv.Configuration.Delimiter = ",";
                        csv.Configuration.MissingFieldFound = null;
                        csv.Configuration.RegisterClassMap<QuantDataMap>(); //allows us to map weirdly named headers in csv
                        int lineCounter = 0;
                        for (int i = 0; i < linesToSkip; i++) csv.Read(); //advances the reader past lines
                        while (csv.Read())
                        {
                            RawQuantData Record = csv.GetRecord<RawQuantData>();
                            myRawQuantData.Add(Record);
                            lineCounter++;
                            if (lineCounter == 384) break;  //stops the reader once 384 quant values have been read
                        }
                        csv.Dispose();
                    }

                    //Average replicates 
                    myRawQuantData = myRawQuantData.Where(x => x.content.Contains("Sample")).ToList();    //only get samples (no standards)
                    foreach (var sampleReplicates in myRawQuantData.GroupBy(x => x.content))
                    {
                        double sum = 0;
                        int sampleCount = 0;
                        foreach (var sample in sampleReplicates)
                        {
                            double concentration = Convert.ToDouble(sample.concentration);
                            if (concentration < 200 && concentration > 0.01)////These need to be variables
                            {
                                sum += concentration;
                                sampleCount++;
                            }
                        }
                        double averageConcentration;
                        if (sampleCount == 0) averageConcentration = 0.0001;     //if all of the quant values were outside of the limits
                        else
                        {
                            averageConcentration = sum / sampleCount;
                            averageConcentration = Math.Round(averageConcentration, 3);
                        }
                        RawQuantData averageQuantData = new RawQuantData
                        {
                            content = sampleReplicates.First().content,
                            concentration = (float)averageConcentration,
                            well = ConvertQuantWellToSourceWell(sampleReplicates.First().well)
                        };
                        QuantData newQuantData = new QuantData(averageQuantData);   //reformat RawQuantData into QuantData
                        myFormattedQuantData.Add(newQuantData);
                    }
                    //this.AvgConcentration = Math.Round(myFormattedQuantData.Average(x => x.concentration), 3);   //get the average from all concentrations
                    //this.CountBelowThreshold = myFormattedQuantData.Count(y => y.concentration < 0.2);  //get the count of samples that are less than 0.4ng/uL
                    this.FormattedJSON = myFormattedQuantData;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }

            private string ConvertQuantWellToSourceWell(string well)
            {
                string row = well.Substring(0, 1);
                int column = Convert.ToInt32(well.Substring(1));

                string rows = "ABCDEFGHIJKLMNOP";
                string rows96 = "ABCDEFGH"; //needed to convert upper left quadrant to 96 well source
                string rows384upperLeft = "ACEGIKMO";
                int rowIndex = rows.IndexOf(row);

                try
                {
                    column = (column + 1) / 2;
                    row = rows96[rows384upperLeft.IndexOf(row)].ToString();
                }
                catch
                {
                    MessageBox.Show("There was an error converting this 384 well to a quadrant");
                }
                return row + column.ToString();
            }

            //public bool ScreenQuantDataXXXXX()
            //{
            //    bool failed = false;
            //    string errorText = "";
            //    if (this.R2 < .69)
            //    {
            //        errorText += "The Standards R2 for this reading is below the threshold:\nLower Limit (R2): " + 0.69.ToString() +
            //            "\nCurrent Value (R2): " + this.R2.ToString() + "\n";
            //        failed = true;
            //    }
            //    if (this.Slope < .69)
            //    {
            //        errorText += "The Standards Slope for this reading is below the threshold:\nLower Limit (Slope): " + 0.69.ToString() +
            //            "\nCurrent Value (Slope): " + this.Slope.ToString() + "\n";
            //        failed = true;
            //    }
            //    if (this.AvgConcentration < .69)
            //    {
            //        errorText += "The Average Concentration for this plate is below the threshold:\nLower Limit (Avg Con.): " + 0.69.ToString() +
            //            "\nCurrent Value (Avg Con.): " + this.AvgConcentration.ToString() + "\n";
            //        failed = true;
            //    }

            //    if (failed == true)
            //    {
            //        DialogResult result = MessageBox.Show("There was an issue with the Picogreen Quant for the following plate:\n\nSource Plate Barcode: " +
            //            this.SourcePlateBarcode + "\nSource Plate Quadrant: " + this.SourcePlateQuadrant + "\n\nError:\n\n" + errorText +
            //            "\nWould you like to abort the Quant Split Norm process for the remaining quadrants on " + this.SourcePlateBarcode +
            //            "?\n\nYES - The source plate (" + this.SourcePlateBarcode + ") will be returned to the Output Hotel, and the Quant Split Norm WFs\n will be failed back to post Library Prep" +
            //            "\n\nNO - The Quant Data will be used, which may result in samples being failed back to Extraction", "iPCR Quant Failure", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            //        if (result == DialogResult.No) failed = false;
            //    }
            //    return failed;
            //}
        }

        public class QuantData
        {
            public string well { get; set; }
            public float concentration { get; set; }
            public QuantData() { }
            public QuantData(RawQuantData rawQuantData)
            {
                //remove leading zero from well
                this.well = rawQuantData.well.Substring(0, 1) + rawQuantData.well.Substring(1).TrimStart(new Char[] { '0' });
                this.concentration = rawQuantData.concentration;
            }
        }

        public class RawQuantData : QuantData
        {
            public string content { get; set; }
        }

        //Class map that allows us to read multiple header strings as a particular attribute (ex " 1 - 1" as concentration)
        private sealed class QuantDataMap : ClassMap<RawQuantData>
        {
            public QuantDataMap()
            {
                Map(m => m.well).Name("Well");
                Map(m => m.content).Name("Content");
                Map(m => m.concentration).Name("Concentration", " 1 - 1");
            }
        }

        public class Plate
        {
            public string Type { get; set; }
            public string Barcode { get; set; }
            public int Position { get; set; }


        }

    }
}

