﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Barco.Data;
using System.Linq;



namespace Barco
{
    /// <summary>
    /// Interaction logic for JobRequest.xaml
    /// </summary>
    public partial class JobRequest : Window    
    {
        //remove this line if working with DAO static class
        //private static Barco2021Context DAO = new Barco2021Context();


        //private static Barco2021Context context = new Barco2021Context();

        //public RqRequest request = new RqRequest();
        //private RqOptionel optional = new RqOptionel();
        //private List<Eut> eutList = new List<Eut>();
        //private RqRequestDetail Detail = new RqRequestDetail();


        private List<Part> parts = new List<Part>();

        List<CheckBox> emcBoxes = new List<CheckBox>();
        List<CheckBox> envBoxes = new List<CheckBox>();
        List<CheckBox> relBoxes = new List<CheckBox>();
        List<CheckBox> prodBoxes = new List<CheckBox>();
        List<CheckBox> greenBoxes = new List<CheckBox>();
        List<CheckBox> selectionBoxes = new List<CheckBox>();



        private static DAO dao;
        public JobRequest()
        {
            InitializeComponent();
            dao = DAO.Instance();


            cmbDivision.ItemsSource = dao.getDivisions();
            cmbDivision.DisplayMemberPath = "Afkorting";
            cmbDivision.SelectedValuePath = "Afkorting";

            cmbJobNature.ItemsSource = dao.getJobNatures();
            cmbJobNature.DisplayMemberPath = "Nature";
            cmbJobNature.SelectedValuePath = "Nature";

            createBoxLists();

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sPartNo = txtPartNr.Text;
                string sNetWeight = txtNetWeight.Text;
                string sGrossWeight = txtGrossWeight.Text;

                if (sPartNo == "" || sNetWeight == "" || sGrossWeight == "")
                {
                    MessageBox.Show("please fill in all values");
                }
                else
                {
                    parts.Add(new Part()
                    {
                        NetWeight = txtNetWeight.Text,
                        GrossWeight = txtGrossWeight.Text,
                        partNo = txtPartNr.Text
                    });
                    refreshGUI();

                    //lstbNetWeight.Items.Add(sNetWeight);
                    //lstbGrossWeight.Items.Add(sGrossWeight);

                   
                }

            }
            catch (NullReferenceException)
            {
                MessageBox.Show("please fill in all fields");
            }

        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            var selectedPart = (Part)lstParts.SelectedItem;

            if (parts.Contains(selectedPart))
            {
                parts.Remove(selectedPart);
                lstParts.Items.Remove(selectedPart);
                refreshGUI();
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //create error sequence
                List<string> errors = new List<string>();

                RqRequest request = new RqRequest();
                RqOptionel optional = new RqOptionel();
                List<Eut> eutList = new List<Eut>();
                RqRequestDetail Detail = new RqRequestDetail();

                //declare vars for object
                string input_Abbreviation = txtReqInitials.Text.ToString();
                string input_ProjectName = txtEutProjectname.Text.ToString();

                bool input_Battery = false;

                if (dateExpectedEnd.SelectedDate != null)
                {
                    DateTime input_EndDate = (DateTime)dateExpectedEnd.SelectedDate;

                }
                else
                {
                    errors.Add("please specify a end date");
                }

                if(parts.Count > 0)
                {
                    foreach(Part part in parts)
                    {
                        request.EutPartnumbers += part.partNo + " ; ";
                        request.GrossWeight += part.GrossWeight + " ; ";
                        request.NetWeight += part.NetWeight + " ; ";
                    }
                }
                else
                {
                    errors.Add("please enter parts");
                }

                string specialRemarks = txtRemark.Text.ToString();

                string netWeights = "";
                string grossWeights = "";
                string partNums = "";

                //parts section
                if (parts.Count > 0)
                {
                    foreach (Part part in parts)
                    {
                        netWeights += part.NetWeight + "; ";
                        grossWeights += part.GrossWeight + "; ";
                        partNums += part.partNo + "; ";
                    }
                }
                else
                {
                    errors.Add("Please add parts to test");
                }


                //check if radio buttons are checked

                if ((bool)rbtnBatNo.IsChecked)
                {
                    input_Battery = true;
                }
                else if ((bool)rbtnBatNo.IsChecked && (bool)rbtnBatYes.IsChecked)
                {
                    errors.Add("please check if batteries are needed");
                }
                else
                {
                    input_Battery = false;
                }

                //check if requester exists

                if (!dao.IfPersonExists(input_Abbreviation))
                {
                    errors.Add("the requester inititals do not match any employee");
                }

                //check if the job nature is selected
                //checkbox area
                if (!(bool)cbEmc.IsChecked && !(bool)cmEnvorimental.IsChecked && !(bool)cmRel.IsChecked && !(bool)cmProdSafety.IsChecked && !(bool)cmGrnComp.IsChecked)
                {
                    errors.Add("Please select a job nature");
                }
                else
                {
                    List<string> validation = ValidateCheckboxes();
                    errors.AddRange(validation);
                }

                //check if the dates are set
                List<string> valiDate = checkDates();
                errors.AddRange(valiDate);

                //check if other fields are empty

                if (txtEutProjectname.Text is null)
                {
                    errors.Add("please fill in a project name");
                }
                //error handling
                if (errors.Count > 0)
                {
                    err_output.ItemsSource = errors;
                }
                else
                {
                    //request object 
                    request.Requester = input_Abbreviation;
                    request.BarcoDivision = cmbDivision.SelectedValue.ToString();
                    request.JobNature = cmbJobNature.SelectedValue.ToString();
                    request.RequestDate = DateTime.Now;
                    request.EutProjectname = txtEutProjectname.Text;
                    request.Battery = input_Battery;
                    request.ExpectedEnddate = (DateTime)dateExpectedEnd.SelectedDate;
                    request.NetWeight = netWeights;
                    request.GrossWeight = grossWeights;
                    request.EutPartnumbers = partNums;
                    request.HydraProjectNr = 1.ToString();

                    Detail.Testdivisie = "ECO";
                    Detail.Pvgresp = "resp";
                    //Detail.IdRequest = request.IdRequest;

                    //optional object
                    optional.Link = txtLinkTestplan.Text;

                    //eut objects
                    eutList = getEutData(Detail);

                    dao.AddRequest(request, Detail, optional);
                }


            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message.ToString());
                //MessageBox.Show("Please fill in all fields"):
            }
        }



        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            HomeScreen homeScreen = new HomeScreen();
            Close();
            homeScreen.ShowDialog();
        }

        private void refreshGUI()
        {
            lstParts.Items.Clear();
            foreach (Part part in parts)
            {
                lstParts.Items.Add(part);
            }
        }




        public class Part
        {
            public string partNo { get; set; }
            public string NetWeight { get; set; }
            public string GrossWeight { get; set; }
        }

        public void createBoxLists()
        {
            emcBoxes.Add(cbEmcEut1);
            emcBoxes.Add(cbEmcEut2);
            emcBoxes.Add(cbEmcEut3);
            emcBoxes.Add(cbEmcEut4);
            emcBoxes.Add(cbEmcEut5);

            envBoxes.Add(cmEnvorimentalEut1);
            envBoxes.Add(cmEnvorimentalEut2);
            envBoxes.Add(cmEnvorimentalEut3);
            envBoxes.Add(cmEnvorimentalEut4);
            envBoxes.Add(cmEnvorimentalEut5);

            relBoxes.Add(cmRelEut1);
            relBoxes.Add(cmRelEut2);
            relBoxes.Add(cmRelEut3);
            relBoxes.Add(cmRelEut4);
            relBoxes.Add(cmRelEut5);

            prodBoxes.Add(cmProdSafetyEut1);
            prodBoxes.Add(cmProdSafetyEut2);
            prodBoxes.Add(cmProdSafetyEut3);
            prodBoxes.Add(cmProdSafetyEut4);
            prodBoxes.Add(cmProdSafetyEut5);

            greenBoxes.Add(cmGrnCompEut1);
            greenBoxes.Add(cmGrnCompEut2);
            greenBoxes.Add(cmGrnCompEut3);
            greenBoxes.Add(cmGrnCompEut4);
            greenBoxes.Add(cmGrnCompEut5);

            selectionBoxes.Add(cmEnvorimental);
            selectionBoxes.Add(cbEmc);
            selectionBoxes.Add(cmRel);
            selectionBoxes.Add(cmProdSafety);
            selectionBoxes.Add(cmGrnComp);
        }

        private void cbEmc_Checked(object sender, RoutedEventArgs e)
        {
            enableBoxes(cbEmc);
        }

        private void cmEnvorimental_Checked(object sender, RoutedEventArgs e)
        {
            enableBoxes(cmEnvorimental);

        }

        private void cmRel_Checked(object sender, RoutedEventArgs e)
        {
            enableBoxes(cmRel);

        }

        private void cmProdSafety_Checked(object sender, RoutedEventArgs e)
        {
            enableBoxes(cmProdSafety);

        }

        private void cmGrnComp_Checked(object sender, RoutedEventArgs e)
        {
            enableBoxes(cmGrnComp);

        }

        private void enableBoxes(CheckBox selected)
        {
            List<CheckBox> targets = new List<CheckBox>();
            if (selected == cbEmc)
            {
                targets = emcBoxes;
            }
            else if (selected == cmEnvorimental)
            {
                targets = envBoxes;
            }
            else if (selected == cmRel)
            {
                targets = relBoxes;
            }
            else if (selected == cmProdSafety)
            {
                targets = prodBoxes;
            }
            else if (selected == cmGrnComp)
            {
                targets = greenBoxes;
            }

            foreach (CheckBox box in targets)
            {
                box.IsEnabled = true;
            }
        }
        private List<string> ValidateCheckboxes()
        {
            List<string> outcome = new List<string>();
            if ((bool)cbEmc.IsChecked)
            {
                int counter = 0;
                foreach (CheckBox box in emcBoxes)
                {
                    if ((bool)box.IsChecked)
                    {
                        counter++;
                    }
                }
                if (counter != 1)
                {
                    outcome.Add("please check emc data");
                }
            }
            if ((bool)cmEnvorimental.IsChecked)
            {
                int counter = 0;
                foreach (CheckBox box in envBoxes)
                {
                    if ((bool)box.IsChecked)
                    {
                        counter++;
                    }
                }
                if (counter != 1)
                {
                    outcome.Add("please check environmental data");
                }
            }
            if ((bool)cmRel.IsChecked)
            {
                int counter = 0;
                foreach (CheckBox box in relBoxes)
                {
                    if ((bool)box.IsChecked)
                    {
                        counter++;
                    }
                }
                if (counter != 1)
                {
                    outcome.Add("please check reliability data");
                }
            }
            if ((bool)cmProdSafety.IsChecked)
            {
                int counter = 0;
                foreach (CheckBox box in prodBoxes)
                {
                    if ((bool)box.IsChecked)
                    {
                        counter++;
                    }
                }
                if (counter != 1)
                {
                    outcome.Add("please check product safety data");
                }
            }
            if ((bool)cmGrnComp.IsChecked)
            {
                int counter = 0;
                foreach (CheckBox box in greenBoxes)
                {
                    if ((bool)box.IsChecked)
                    {
                        counter++;
                    }
                }
                if (counter != 1)
                {
                    outcome.Add("please check green compliance data");
                }
            }
            return outcome;
        }

        private List<string> checkDates()
        {
            List<string> result = new List<string>();
            if ((bool)cbEmcEut1.IsChecked || (bool)cmEnvorimentalEut1.IsChecked || (bool)cmGrnCompEut1.IsChecked || (bool)cmProdSafetyEut1.IsChecked || (bool)cmGrnCompEut1.IsChecked)
            {
                if (DatePickerEUT1.SelectedDate == null)
                {
                    result.Add("please provide a date for EUT 1");
                }
            }
            if ((bool)cbEmcEut2.IsChecked || (bool)cmEnvorimentalEut2.IsChecked || (bool)cmGrnCompEut2.IsChecked || (bool)cmProdSafetyEut2.IsChecked || (bool)cmGrnCompEut2.IsChecked)
            {
                if (DatePickerEUT2.SelectedDate == null)
                {
                    result.Add("please provide a date for EUT 2");
                }
            }
            if ((bool)cbEmcEut3.IsChecked || (bool)cmEnvorimentalEut3.IsChecked || (bool)cmGrnCompEut3.IsChecked || (bool)cmProdSafetyEut3.IsChecked || (bool)cmGrnCompEut3.IsChecked)
            {
                if (DatePickerEUT3.SelectedDate == null)
                {
                    result.Add("please provide a date for EUT 3");
                }
            }
            if ((bool)cbEmcEut4.IsChecked || (bool)cmEnvorimentalEut4.IsChecked || (bool)cmGrnCompEut4.IsChecked || (bool)cmProdSafetyEut4.IsChecked || (bool)cmGrnCompEut4.IsChecked)
            {
                if (DatePickerEUT4.SelectedDate == null)
                {
                    result.Add("please provide a date for EUT 4");
                }
            }
            if ((bool)cbEmcEut5.IsChecked || (bool)cmEnvorimentalEut5.IsChecked || (bool)cmGrnCompEut5.IsChecked || (bool)cmProdSafetyEut5.IsChecked || (bool)cmGrnCompEut5.IsChecked)
            {
                if (DatePickerEUT5.SelectedDate == null)
                {
                    result.Add("please provide a date for EUT 5");
                }
            }
            return result;
        }

        private List<Eut> getEutData(RqRequestDetail Detail)
        {
            List<Eut> result = new List<Eut>();

            //get first eut and date
            if (DatePickerEUT1.SelectedDate != null)
            {
                DateTime date = (DateTime)dateExpectedEnd.SelectedDate;
                string description = "";
                if ((bool)cbEmcEut1.IsChecked)
                {
                    description = "EMC - EUT 1";
                }
                if ((bool)cmEnvorimentalEut1.IsChecked)
                {
                    description = "Environmental - EUT 1";
                }
                if ((bool)cmGrnCompEut1.IsChecked)
                {
                    description = "Green Compliance - EUT 1";
                }
                if ((bool)cmRelEut1.IsChecked)
                {
                    description = "Reliability - EUT 1";
                }
                if ((bool)cmProdSafetyEut1.IsChecked)
                {
                    description = "Product Safety - EUT 1";
                }
                result.Add(new Eut()
                {
                    IdRqDetail = Detail.IdRqDetail,
                    AvailableDate = date,
                    OmschrijvingEut = description
                });
            }

            //get second eut and date
            if (DatePickerEUT2.SelectedDate != null)
            {
                DateTime date = (DateTime)dateExpectedEnd.SelectedDate;
                string description = "";
                if ((bool)cbEmcEut2.IsChecked)
                {
                    description = "EMC - EUT 2";
                }
                if ((bool)cmEnvorimentalEut2.IsChecked)
                {
                    description = "Environmental - EUT 2";
                }
                if ((bool)cmGrnCompEut2.IsChecked)
                {
                    description = "Green Compliance - EUT 2";
                }
                if ((bool)cmRelEut2.IsChecked)
                {
                    description = "Reliability - EUT 2";
                }
                if ((bool)cmProdSafetyEut2.IsChecked)
                {
                    description = "Product Safety - EUT 2";
                }
                result.Add(new Eut()
                {
                    IdRqDetail = Detail.IdRqDetail,
                    AvailableDate = date,
                    OmschrijvingEut = description
                });
            }

            //get third eut and date
            if (DatePickerEUT3.SelectedDate != null)
            {
                DateTime date = (DateTime)dateExpectedEnd.SelectedDate;
                string description = "";
                if ((bool)cbEmcEut3.IsChecked)
                {
                    description = "EMC - EUT 3";
                }
                if ((bool)cmEnvorimentalEut3.IsChecked)
                {
                    description = "Environmental - EUT 3";
                }
                if ((bool)cmGrnCompEut3.IsChecked)
                {
                    description = "Green Compliance - EUT 3";
                }
                if ((bool)cmRelEut3.IsChecked)
                {
                    description = "Reliability - EUT 3";
                }
                if ((bool)cmProdSafetyEut3.IsChecked)
                {
                    description = "Product Safety - EUT 3";
                }
                result.Add(new Eut()
                {
                    IdRqDetail = Detail.IdRqDetail,
                    AvailableDate = date,
                    OmschrijvingEut = description
                });
            }
            //get fourth eut and date
            if (DatePickerEUT4.SelectedDate != null)
            {
                DateTime date = (DateTime)dateExpectedEnd.SelectedDate;
                string description = "";
                if ((bool)cbEmcEut4.IsChecked)
                {
                    description = "EMC - EUT 4";
                }
                if ((bool)cmEnvorimentalEut4.IsChecked)
                {
                    description = "Environmental - EUT 4";
                }
                if ((bool)cmGrnCompEut4.IsChecked)
                {
                    description = "Green Compliance - EUT 4";
                }
                if ((bool)cmRelEut4.IsChecked)
                {
                    description = "Reliability - EUT 4";
                }
                if ((bool)cmProdSafetyEut4.IsChecked)
                {
                    description = "Product Safety - EUT 4";
                }
                result.Add(new Eut()
                {
                    IdRqDetail = Detail.IdRqDetail,
                    AvailableDate = date,
                    OmschrijvingEut = description
                });
            }

            //get fifth eut and date
            if (DatePickerEUT5.SelectedDate != null)
            {
                DateTime date = (DateTime)dateExpectedEnd.SelectedDate;
                string description = "";
                if ((bool)cbEmcEut5.IsChecked)
                {
                    description = "EMC - EUT 5";
                }
                if ((bool)cmEnvorimentalEut5.IsChecked)
                {
                    description = "Environmental - EUT 5";
                }
                if ((bool)cmGrnCompEut5.IsChecked)
                {
                    description = "Green Compliance - EUT 5";
                }
                if ((bool)cmRelEut5.IsChecked)
                {
                    description = "Reliability - EUT 5";
                }
                if ((bool)cmProdSafetyEut5.IsChecked)
                {
                    description = "Product Safety - EUT 5";
                }
                result.Add(new Eut()
                {
                    IdRqDetail = Detail.IdRqDetail,
                    AvailableDate = date,
                    OmschrijvingEut = description
                });
            }
            return result;

        }

        private void btnCancel_Click_1(object sender, RoutedEventArgs e)
        {
            HomeScreen login = new HomeScreen();
            Close();
            login.ShowDialog();
        }
    }
}