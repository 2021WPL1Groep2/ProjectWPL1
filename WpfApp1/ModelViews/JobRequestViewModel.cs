﻿using Barco.Data;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
namespace Barco
{ //bianca
    public class JobRequestViewModel : ViewModelBase
    {
        public JobRequest screen;
        public RqRequest request = new RqRequest();
        //binding value
        public Part selectedPart { get; set; }
        // buttons : Add/Remove, Send/Cancel
        public ICommand CancelCommand { get; set; }
        public ICommand SendCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public string txtPartNr { get; set; } // EUT Partnumber
        public string txtNetWeight { get; set; } //net weight
        public string txtGrossWeight { get; set; } //gross weight
        public string txtLinkTestplan { get; set; } // link to testplan
        public string txtReqInitials { get; set; } // requester initials 
        public string txtEutProjectname { get; set; } //EUT Project name
        public string txtRemark { get; set; } // special remarks
        public string txtFunction { get; set; } //function
        public DateTime dateExpectedEnd { get; set; }
        public string selectedDivision { get; set; }
        public string SelectedJobNature { get; set; }
        // EUT foreseen availability date
        public DateTime DatePickerEUT1 { get; set; }
        public DateTime DatePickerEUT2 { get; set; }
        public DateTime DatePickerEUT3 { get; set; }
        public DateTime DatePickerEUT4 { get; set; }
        public DateTime DatePickerEUT5 { get; set; }
        private ObservableCollection<Part>
            lstParts = new ObservableCollection<Part>(); // for partnumber+ net/gross weight
        public ObservableCollection<Part> listParts
        {
            get { return lstParts; }
        }
        private ObservableCollection<string> _err_output { get; set; } // listview for errors
        //remove this line if working with DAO static class
        //private static Barco2021Context DAO = new Barco2021Context();
        private DAO dao;
        public JobRequestViewModel jobRequestViewModel;
        public RqOptionel optional = new RqOptionel();
        public List<Eut> eutList = new List<Eut>();
        public RqRequestDetail Detail = new RqRequestDetail();//to do delete 
        public List<Part> parts = new List<Part>();
        List<bool> emcBoxes = new List<bool>();
        List<bool> envBoxes = new List<bool>();
        List<bool> relBoxes = new List<bool>();
        List<bool> prodBoxes = new List<bool>();
        List<bool> greenBoxes = new List<bool>();
        List<bool> selectionBoxes = new List<bool>();
        public bool cbEmcEut1 { get; set; }
        public bool cbEmcEut2 { get; set; }
        public bool cbEmcEut3 { get; set; }
        public bool cbEmcEut4 { get; set; }
        public bool cbEmcEut5 { get; set; }
        public bool cmEnvironmentalEut1 { get; set; }
        public bool cmEnvironmentalEut2 { get; set; }
        public bool cmEnvironmentalEut3 { get; set; }
        public bool cmEnvironmentalEut4 { get; set; }
        public bool cmEnvironmentalEut5 { get; set; }
        public bool cmGrnCompEut1 { get; set; }
        public bool cmGrnCompEut2 { get; set; }
        public bool cmGrnCompEut3 { get; set; }
        public bool cmGrnCompEut4 { get; set; }
        public bool cmGrnCompEut5 { get; set; }
        public bool cmProdSafetyEut1 { get; set; }
        public bool cmProdSafetyEut2 { get; set; }
        public bool cmProdSafetyEut3 { get; set; }
        public bool cmProdSafetyEut4 { get; set; }
        public bool cmProdSafetyEut5 { get; set; }
        public bool cmRelEut1 { get; set; }
        public bool cmRelEut2 { get; set; }
        public bool cmRelEut3 { get; set; }
        public bool cmRelEut4 { get; set; }
        public bool cmRelEut5 { get; set; }
        public bool cbEmc { get; set; }
        public bool cmEnvironmental { get; set; }
        public bool cmRel { get; set; }
        public bool cmProdSafety { get; set; }
        public bool cmGrnComp { get; set; }
        public ComboBox cmbDivision { get; set; }
        public ComboBox cmbJobNature { get; set; }
        //radio button
        public bool rbtnBatNo { get; set; }
        public bool rbtnBatYes { get; set; }
        public JobRequestViewModel(JobRequest screen)
        {
            CancelCommand = new DelegateCommand(CancelButton);
            SendCommand = new DelegateCommand(SendButton);
            AddCommand = new DelegateCommand(AddButton);
            RemoveCommand = new DelegateCommand(RemoveButton);
            dao = DAO.Instance();
            this.screen = screen;
            dateExpectedEnd = DateTime.Now;
            DatePickerEUT1 = DateTime.Now;
            DatePickerEUT2 = DateTime.Now;
            DatePickerEUT3 = DateTime.Now;
            DatePickerEUT4 = DateTime.Now;
            DatePickerEUT5 = DateTime.Now;
            _err_output = new ObservableCollection<string>();
            txtFunction = GetValues("FUNCTION");
            txtReqInitials = GetInitialsFromReg();
        }
        /// <summary>
        /// Thibaut, jimmy
        /// </summary>
        /// <returns></returns>
        public string GetInitialsFromReg()
        {
            string fullName = GetValues("NAME");
            string FirstName = fullName.Split(" ")[0];
            string LastName = fullName.Split(" ")[1];
            return (FirstName.Substring(0, 2) + LastName.Substring(0, 1)).ToUpper();
        }
        /// <summary>
        /// Thibaut, Jimmy
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        static string GetValues(string Name)
        {
            string userRoot = "HKEY_CURRENT_USER";
            string subkey = "Barco2021";
            string keyName = userRoot + "\\" + subkey;
            return Microsoft.Win32.Registry.GetValue(keyName, Name, "default").ToString();
        }
        /// <summary>
        /// Bianca
        /// </summary>
        //working internally with the binding 
        public Part SelectedPart
        {
            get { return selectedPart; }
            set
            {
                selectedPart = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Bianca 
        /// </summary>
        public void CancelButton()
        {
            if (MessageBox.Show("Are you sure you want to leave this screen without saving?", "Leave", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
              
                HomeScreen home = new HomeScreen();
                screen.Close();
                home.ShowDialog();
            }
        }
        //Thibaut
        public void AddButton()
        {
            try
            {
                if (txtPartNr.Length == 0 || txtNetWeight.Length == 0 || txtGrossWeight.Length == 0)
                {
                    MessageBox.Show("please fill in all values");
                }
                else
                {
                    parts.Add(new Part()
                    {
                        NetWeight = txtNetWeight,
                        GrossWeight = txtGrossWeight,
                        partNo = txtPartNr
                    });
                    request.EutPartnumbers += txtPartNr + " ; ";
                    request.GrossWeight += txtNetWeight + " ; ";
                    request.NetWeight += txtGrossWeight + " ; ";
                    RefreshGUI();
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("please fill in all fields");
            }
        }
        /// <summary>
        /// Jimmy
        /// </summary>
        private void RefreshGUI()
        {
            lstParts.Clear();
            foreach (Part part in parts)
            {
                lstParts.Add(part);
            }
        }
        /// <summary>
        /// Jimmy, Thibaut, Bianca, Laurent
        /// </summary>
        public void RemoveButton()
        {
            if (parts.Contains(selectedPart))
            {
                if (MessageBox.Show("Are you sure you want to delete part " + selectedPart.partNo + "?", "Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    parts.Remove(selectedPart);
                    lstParts.Remove(selectedPart);
                    RefreshGUI();
                    OnPropertyChanged();
                }
                
            }
        }
        /// <summary>
        /// Thibaut, Bianca, Laurent, Stach, Jimmy
        /// </summary>
        public void SendButton()
        {
            try
            {
                //create error sequence
                CreateBoxLists();
                List<string> errors = new List<string>();
                _err_output.Clear();
                //declare var for object
                string input_Abbreviation = txtReqInitials;
                string input_ProjectName = txtEutProjectname;
                bool input_Battery = false;
                DateTime input_EndDate = DateTime.Now;
                if (dateExpectedEnd.Date != null )
                {
                    if (dateExpectedEnd.Date < DateTime.Today)
                    {
                        errors.Add("The end date has to be in the future");
                    }
                    else
                    {
                        input_EndDate = dateExpectedEnd.Date;
                    }
                }
                else
                {
                    errors.Add("please specify a end date");
                }
                string specialRemarks = txtRemark;
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
                if ((bool)rbtnBatNo==true)
                {
                    input_Battery = false;
                }
                else if ((bool)rbtnBatNo==false && (bool)rbtnBatYes==false)
                {
                    errors.Add("please check if batteries are needed");
                }
                else
                {
                    input_Battery = true;
                }
                //check if requester exists
                if (dao.IfPersonExists(input_Abbreviation))
                {
                    errors.Add("the requester initials do not match any employee");
                }
                //check if the job nature is selected
                if (SelectedJobNature == null)
                {
                    errors.Add("select a jobnature");
                }
                if (selectedDivision == null)
                {
                    errors.Add("select a division");
                }
                //checkbox area
                if (!(bool)cbEmc && !(bool)cmEnvironmental && !(bool)cmRel &&
                    !(bool)cmProdSafety && !(bool)cmGrnComp)
                {
                    errors.Add("Please select a test nature");
                }
                else
                {
                    List<string> validation = ValidateCheckboxes();
                    errors.AddRange(validation);
                }
                //check if the dates are set
                List<string> valiDate = CheckDates();
                errors.AddRange(valiDate);
                //check if other fields are empty
                if (txtEutProjectname==null)
                {
                    errors.Add("please fill in a project name");
                }
                //error handling
                if (errors.Count > 0)
                {
                    foreach (string s in errors)
                    {
                        _err_output.Add(s);
                    }
                }
                else
                {
                    //request object 
                    request.Requester = input_Abbreviation;
                    request.BarcoDivision = selectedDivision;
                    request.JobNature = SelectedJobNature;
                    request.RequestDate = DateTime.Now;
                    request.EutProjectname = txtEutProjectname;
                    request.Battery = input_Battery;
                    request.ExpectedEnddate = dateExpectedEnd.Date;
                    request.NetWeight = netWeights;
                    request.GrossWeight = grossWeights;
                    request.EutPartnumbers = partNums;
                    request.HydraProjectNr = "0";
                    request.InternRequest = CheckInternal(input_Abbreviation);//naam moet in lijst zitten om intern rq te maken
                    if (CheckInternal(input_Abbreviation))
                    {
                        dao.ApproveRqRequest(request, CreateJRNumberForInternal());
                    }
                    //optional object
                    optional.Link = txtLinkTestplan;
                    optional.IdRequest = request.IdRequest;
                    optional.Remarks = specialRemarks;
                    //eut objects
                    eutList = CreateEutList();
                    //detail object
                    //Detail.Testdivisie = "eco";  
                    List<RqRequestDetail>detailList = GetRqRequestDetails();
                    try
                    {
                        dao.AddRequest(request, detailList, optional, eutList);
                        MessageBox.Show("Data has been inserted");
                        HomeScreen home = new HomeScreen();
                        screen.Close();
                        home.ShowDialog();

                    }
                    catch (Exception ex)
                    {

                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message.ToString());
                //MessageBox.Show("Please fill in all fields"):
            }
        }
        // to show in the listview
        /// <summary>
        /// Bianca, Laurent
        /// </summary>
        public class Part
        {
            public string partNo { get; set; }
            public string NetWeight { get; set; }
            public string GrossWeight { get; set; }
        }
        /// <summary>
        /// Thibaut, Jimmy, Stach
        /// </summary>
        public void CreateBoxLists()
        {
            emcBoxes.Clear();
            emcBoxes.Add(cbEmcEut1);
            emcBoxes.Add(cbEmcEut2);
            emcBoxes.Add(cbEmcEut3);
            emcBoxes.Add(cbEmcEut4);
            emcBoxes.Add(cbEmcEut5);
            envBoxes.Clear();
            envBoxes.Add(cmEnvironmentalEut1);
            envBoxes.Add(cmEnvironmentalEut2);
            envBoxes.Add(cmEnvironmentalEut3);
            envBoxes.Add(cmEnvironmentalEut4);
            envBoxes.Add(cmEnvironmentalEut5);
            relBoxes.Clear();
            relBoxes.Add(cmRelEut1);
            relBoxes.Add(cmRelEut2);
            relBoxes.Add(cmRelEut3);
            relBoxes.Add(cmRelEut4);
            relBoxes.Add(cmRelEut5);
            prodBoxes.Clear();
            prodBoxes.Add(cmProdSafetyEut1);
            prodBoxes.Add(cmProdSafetyEut2);
            prodBoxes.Add(cmProdSafetyEut3);
            prodBoxes.Add(cmProdSafetyEut4);
            prodBoxes.Add(cmProdSafetyEut5);
            greenBoxes.Clear();
            greenBoxes.Add(cmGrnCompEut1);
            greenBoxes.Add(cmGrnCompEut2);
            greenBoxes.Add(cmGrnCompEut3);
            greenBoxes.Add(cmGrnCompEut4);
            greenBoxes.Add(cmGrnCompEut5);
            selectionBoxes.Clear();
            selectionBoxes.Add(cbEmc);
            selectionBoxes.Add(cmEnvironmental);
            selectionBoxes.Add(cmRel);
            selectionBoxes.Add(cmProdSafety);
            selectionBoxes.Add(cmGrnComp);
        }
        /// <summary>
        /// Thibaut, Laurent
        /// </summary>
        /// <param name="selected"></param>
        private void EnableBoxes(bool selected)
        {
            List<bool> targets = new List<bool>();
            if (cbEmc)
            {
                targets = emcBoxes;
            }
            else if (cmEnvironmental)
            {
                targets = envBoxes;
            }
            else if (cmRel)
            {
                targets = relBoxes;
            }
            else if (cmProdSafety)
            {
                targets = prodBoxes;
            }
            else if (cmGrnComp)
            {
                targets = greenBoxes;
            }
            //foreach (bool b in targets)
            //{
            //    b = true;
            //}
        }
        //laurent - edit thibaut
        private List<string> ValidateCheckboxes()
        {
            List<string> outcome = new List<string>();
            if ((bool)cbEmc)
            {
                int counter = 0;
                foreach (bool b in emcBoxes)
                {
                    if ((bool)b)
                    {
                        counter++;
                    }
                }
                if (counter < 1)
                {
                    outcome.Add("please check emc data");
                }
            }
            if ((bool)cmEnvironmental)
            {
                int counter = 0;
                foreach (bool b in envBoxes)
                {
                    if ((bool)b)
                    {
                        counter++;
                    }
                }
                if (counter < 1)
                {
                    outcome.Add("please check environmental data");
                }
            }
            if ((bool)cmRel)
            {
                int counter = 0;
                foreach (bool b in relBoxes)
                {
                    if ((bool)b)
                    {
                        counter++;
                    }
                }
                if (counter < 1)
                {
                    outcome.Add("please check reliability data");
                }
            }
            if ((bool)cmProdSafety)
            {
                int counter = 0;
                foreach (bool b in prodBoxes)
                {
                    if ((bool)b)
                    {
                        counter++;
                    }
                }
                if (counter < 1)
                {
                    outcome.Add("please check product safety data");
                }
            }
            if ((bool)cmGrnComp)
            {
                int counter = 0;
                foreach (bool b in greenBoxes)
                {
                    if ((bool)b)
                    {
                        counter++;
                    }
                }
                if (counter < 1)
                {
                    outcome.Add("please check green compliance data");
                }
            }
            return outcome;
        }
        /// <summary>
        /// Thibaut, Stach, Jimmy
        /// </summary>
        /// <returns></returns>
        private List<string> CheckDates()
        {
            List<string> result = new List<string>();
            if ((bool)cbEmcEut1 || (bool)cmEnvironmentalEut1 || (bool)cmGrnCompEut1 || (bool)cmProdSafetyEut1 ||
                (bool)cmGrnCompEut1)
            {
                if (DatePickerEUT1.Date == null || DatePickerEUT1.Date<DateTime.Today)
                {
                    result.Add("please provide a  valid date for EUT 1");
                }
            }
            if ((bool)cbEmcEut2 || (bool)cmEnvironmentalEut2 || (bool)cmGrnCompEut2 || (bool)cmProdSafetyEut2 ||
                (bool)cmGrnCompEut2)
            {
                if (DatePickerEUT2.Date == null || DatePickerEUT2.Date < DateTime.Today)
                {
                    result.Add("please provide a valid date for EUT 2");
                }
            }
            if ((bool)cbEmcEut3 || (bool)cmEnvironmentalEut3 || (bool)cmGrnCompEut3 || (bool)cmProdSafetyEut3 ||
                (bool)cmGrnCompEut3)
            {
                if (DatePickerEUT3.Date == null || DatePickerEUT3.Date < DateTime.Today)
                {
                    result.Add("please provide a valid date for EUT 3");
                }
            }
            if ((bool)cbEmcEut4 || (bool)cmEnvironmentalEut4 || (bool)cmGrnCompEut4 || (bool)cmProdSafetyEut4 ||
                (bool)cmGrnCompEut4)
            {
                if (DatePickerEUT4.Date == null || DatePickerEUT4.Date < DateTime.Today)
                {
                    result.Add("please provide a valid date for EUT 4");
                }
            }
            if ((bool)cbEmcEut5 || (bool)cmEnvironmentalEut5 || (bool)cmGrnCompEut5 || (bool)cmProdSafetyEut5 ||
                (bool)cmGrnCompEut5)
            {
                if (DatePickerEUT5.Date == null || DatePickerEUT5.Date < DateTime.Today)
                {
                    result.Add("please provide a valid date for EUT 5");
                }
            }
            return result;
        }
        //thibaut
        private Eut createEut(int detailId, string description, DateTime date)
        {
            return new Eut()
            {
                IdRqDetail = detailId,
                AvailableDate = date,
                OmschrijvingEut = description
            };
        }
        /// <summary>
        /// Bianca
        /// </summary>
        public ObservableCollection<string> err_output
        {
            get { return _err_output; }
            set { _err_output = value; }
        }
        //Bianca - thibaut
        //check if the person is an internal of external based on the initials
        //requester in DB table person --> internal ATM
        private bool CheckInternal(string Name)
        {
            List<string> list = new List<string>();
            foreach(Person p in dao.GetAllPerson())
            {
                list.Add(p.Afkorting);
            }
            //list.Add("THD");//testen of code werkt 
            return list.Contains(Name);
        }
        //thibaut
        private List<Eut> CreateEutList()
        {
            List<Eut> eutList = new List<Eut>();
            List<RqRequestDetail> details = GetRqRequestDetails();
            DateTime date;
            string description = "";
            foreach (RqRequestDetail d in details)
            {
                if ((bool)cbEmcEut1 && d.Testdivisie.Equals("EMC"))
                {
                    date = (DateTime)DatePickerEUT1.Date;
                    description = "EMC - EUT 1";
                    eutList.Add(createEut(d.IdRequest,description, date));                  
                }
                if ((bool)cbEmcEut2 && d.Testdivisie.Equals("EMC"))
                {
                    date = (DateTime)DatePickerEUT2.Date;
                    description = "EMC - EUT 2";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cbEmcEut3 && d.Testdivisie.Equals("EMC"))
                {
                    date = (DateTime)DatePickerEUT3.Date;
                    description = "EMC - EUT 3";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cbEmcEut4 && d.Testdivisie.Equals("EMC"))
                {
                    date = (DateTime)DatePickerEUT4.Date;
                    description = "EMC - EUT 4";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cbEmcEut5 && d.Testdivisie.Equals("EMC"))
                {
                    date = (DateTime)DatePickerEUT5.Date;
                    description = "EMC - EUT 5";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cmEnvironmentalEut1 && d.Testdivisie.Equals("ENV"))
                {
                    date = (DateTime)DatePickerEUT1.Date;
                    description = "ENV - EUT 1";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cmEnvironmentalEut2 && d.Testdivisie.Equals("ENV"))
                {
                    date = (DateTime)DatePickerEUT2.Date;
                    description = "ENV - EUT 2";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cmEnvironmentalEut3 && d.Testdivisie.Equals("ENV"))
                {
                    date = (DateTime)DatePickerEUT3.Date;
                    description = "ENV - EUT 3";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cmEnvironmentalEut4 && d.Testdivisie.Equals("ENV"))
                {
                    date = (DateTime)DatePickerEUT4.Date;
                    description = "ENV - EUT 4";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cmEnvironmentalEut5 && d.Testdivisie.Equals("ENV"))
                {
                    date = (DateTime)DatePickerEUT5.Date;
                    description = "ENV - EUT 5";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cmRelEut1 && d.Testdivisie.Equals("REL"))
                {
                    date = (DateTime)DatePickerEUT1.Date;
                    description = "REL - EUT 1";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cmRelEut2 && d.Testdivisie.Equals("REL"))
                {
                    date = (DateTime)DatePickerEUT2.Date;
                    description = "REL - EUT 2";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cmRelEut3 && d.Testdivisie.Equals("REL"))
                {
                    date = (DateTime)DatePickerEUT3.Date;
                    description = "REL - EUT 3";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cmRelEut4 && d.Testdivisie.Equals("REL"))
                {
                    date = (DateTime)DatePickerEUT4.Date;
                    description = "REL - EUT 4";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cmRelEut5 && d.Testdivisie.Equals("REL"))
                {
                    date = (DateTime)DatePickerEUT5.Date;
                    description = "REL - EUT 5";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cmProdSafetyEut1 && d.Testdivisie.Equals("SAF"))
                {
                    date = (DateTime)DatePickerEUT1.Date;
                    description = "SAF - EUT 1";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cmProdSafetyEut2 && d.Testdivisie.Equals("SAF"))
                {
                    date = (DateTime)DatePickerEUT2.Date;
                    description = "SAF - EUT 2";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cmProdSafetyEut3 && d.Testdivisie.Equals("SAF"))
                {
                    date = (DateTime)DatePickerEUT3.Date;
                    description = "SAF - EUT 3";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cmProdSafetyEut4 && d.Testdivisie.Equals("SAF"))
                {
                    date = (DateTime)DatePickerEUT4.Date;
                    description = "SAF - EUT 4";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cmProdSafetyEut5 && d.Testdivisie.Equals("SAF"))
                {
                    date = (DateTime)DatePickerEUT5.Date;
                    description = "SAF - EUT 5";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cmGrnCompEut1 && d.Testdivisie.Equals("ECO"))
                {
                    date = (DateTime)DatePickerEUT1.Date;
                    description = "ECO - EUT 1";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cmGrnCompEut2 && d.Testdivisie.Equals("ECO"))
                {
                    date = (DateTime)DatePickerEUT2.Date;
                    description = "ECO - EUT 2";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cmGrnCompEut3 && d.Testdivisie.Equals("ECO"))
                {
                    date = (DateTime)DatePickerEUT3.Date;
                    description = "ECO - EUT 3";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cmGrnCompEut4 && d.Testdivisie.Equals("ECO"))
                {
                    date = (DateTime)DatePickerEUT4.Date;
                    description = "ECO - EUT 4";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
                if ((bool)cmGrnCompEut5 && d.Testdivisie.Equals("ECO"))
                {
                    date = (DateTime)DatePickerEUT5.Date;
                    description = "ECO - EUT 5";
                    eutList.Add(createEut(d.IdRequest, description, date));
                }
            }
            return eutList;
        }
        //thibaut
        private List<RqRequestDetail> GetRqRequestDetails()
        {
            List<RqRequestDetail> requestDetails = new List<RqRequestDetail>();
            for(int i = 0; i< selectionBoxes.Count; i++)
            {
                if (selectionBoxes[i])
                {
                    RqRequestDetail rqRequest = new RqRequestDetail();
                    rqRequest.IdRequest = request.IdRequest;
                    if(i == 0)
                    {
                        rqRequest.Testdivisie = "EMC";
                    }
                    else if (i == 1)
                    {
                        rqRequest.Testdivisie = "ENV";
                    }
                    else if (i == 2)
                    {
                        rqRequest.Testdivisie = "REL";
                    }
                    else if (i == 3)
                    {
                        rqRequest.Testdivisie = "SAF";
                    }
                    else if (i == 4)
                    {
                        rqRequest.Testdivisie = "ECO";
                    }
                    requestDetails.Add(rqRequest);
                }
            }
            return requestDetails;
        }
        /// <summary>
        /// Thibaut
        /// </summary>

        private string CreateJRNumberForInternal()
        {
            string result = dao.GetJobNumber(true);
            if (result != null && result != "")
            {
                int value = Convert.ToInt32( result.Substring(2));
                value++;
                result = "IN" + value;
                while(result.Length < 6)
                {
                    result = result.Insert(2, "0");
                }
            }
            else//bij nieuwe DB wordt gereset
            {
                result = "IN0001";
            }
            return result;
        }
    }
}
  