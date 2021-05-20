﻿using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows;
using Barco.Data;

namespace Barco
{//jimmy
   public class JobRequestAanpassenViewModel : ViewModelBase
    {
        private JobRequestAanpassen screen;
        private DAO dao;
        public List<Part> parts = new List<Part>();
        public Part selectedPart { get; set; }
        public string txtPartNumber { get; set; } // EUT Partnumber
        public string txtPartNetWeight { get; set; } //net weight
        public string txtPartGrossWeight { get; set; } //gross weight

        public string txtLinkTestplan { get; set; } // link to testplan
        public string txtReqInitials { get; set; } // requester initials 
        public string txtEutProjectname { get; set; } //EUT Project name
        public string txtRemark { get; set; } // special remarks
        public string txtFunction { get; set; } //function
        public RqRequest CurrentRequest { get; set; }
        public RqOptionel CurrentOptionel { get; set; }
        public RqRequestDetail CurrentRequestDetail { get; set; }
        public DateTime dateExpectedEnd { get; set; }


        public ICommand CancelCommand { get; set; }
        public ICommand SaveChangesCommand { get; set; }
        public ICommand AddCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public RqRequest Request { get; set; }
        public RqOptionel rqOptionel { get; set; }
        public RqRequestDetail rqRequestDetail { get; set; }
        public List<String> ListPartsnumbers { get; set; }
        public List<String> ListPartNet { get; set; }
        public List<String> ListPartGross { get; set; }


        private ObservableCollection<Part> lstParts = new ObservableCollection<Part>(); // for partnumber+ net/gross weight

        public ObservableCollection<Part> listParts
        {
            get { return lstParts; }
        }


        public JobRequestAanpassenViewModel(JobRequestAanpassen screen, int selectedId)
        {
            dao = DAO.Instance();

            this.ListPartsnumbers = new List<string>();
            this.ListPartGross = new List<string>();
            this.ListPartNet = new List<string>();

            CancelCommand = new DelegateCommand(CancelButton);
            SaveChangesCommand = new DelegateCommand(SaveChanges);
            AddCommand = new DelegateCommand(AddPart);
            RemoveCommand = new DelegateCommand(RemovePart);
            this.Request = dao.GetRequest(selectedId);
            this.rqOptionel = dao.GetOptionel(selectedId);
            this.rqRequestDetail = dao.GetRequestDetail(selectedId);

            LoadPartGrossWeight();
            LoadPartNetWeight();
            LoadPartsNumbers();


            this.screen = screen;
            CurrentRequest = dao.GetRequest(selectedId);
            CurrentOptionel = dao.GetOptionel(selectedId);
            CurrentRequestDetail = dao.GetRequestDetail(selectedId);
            FillData();


        }
        //biance
        // Sluit aanpassen en opent overview
        public void CancelButton()
        {
            OverviewJobRequest overview = new OverviewJobRequest();
            screen.Close();
           overview.ShowDialog();
        }
        public class Part
        {
            public string partNo { get; set; }
            public string NetWeight { get; set; }
            public string GrossWeight { get; set; }
        }

        public void SaveChanges()
        {
            
           
        }
        /// <summary>
        /// jimmy
        /// </summary>
        public void RemovePart()
        {

            if (parts.Contains(selectedPart))
            {
                parts.Remove(selectedPart);
                lstParts.Remove(selectedPart);
                RefreshGUI();
                OnPropertyChanged();
            }

        }
        /// <summary>
        /// jimmy
        /// </summary>
        public void AddPart()
        {
            try
            {
                if (txtPartNumber == "" || txtPartNetWeight == "" || txtPartGrossWeight == "")
                {
                    MessageBox.Show("please fill in all values");
                }
                else
                {
                    parts.Add(new Part()
                    {
                        NetWeight = txtPartNetWeight,
                        GrossWeight = txtPartGrossWeight,
                        partNo = txtPartNumber

                    });

                    Request.EutPartnumbers += txtPartNumber + " ; ";
                    Request.GrossWeight += txtPartNetWeight + " ; ";
                    Request.NetWeight += txtPartGrossWeight + " ; ";
                    dao.saveChanges();



                    RefreshGUI();
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("please fill in all fields");
            }

        }
        //Jimmy
        //Laden van Jobrequest Partnumbers in een list
        public void LoadPartsNumbers()
        {
            string Partnumbers = Request.EutPartnumbers.Replace(" ", String.Empty);
            string Partnumber;

            do
            {

                int splitIndex = Partnumbers.IndexOf(";");
                Partnumber = Partnumbers.Substring(0, splitIndex);
                ListPartsnumbers.Add(Partnumber);
                int length = Partnumbers.Length;


                if (splitIndex != length)
                {
                    Partnumbers = Partnumbers.Substring((splitIndex + 1), (Partnumbers.Length - 1 - splitIndex));

                }
            } while (Partnumbers.Contains(";"));



        }
        //Jimmy
        //Laden van Jobrequest Net Weight in een list
        public void LoadPartNetWeight()
        {
            string Partnets = Request.NetWeight.Replace(" ", String.Empty);
            string Partnet;

            do
            {
                int splitIndex = Partnets.IndexOf(";");
                Partnet = Partnets.Substring(0, splitIndex);
                ListPartNet.Add(Partnet);
                int length = Partnets.Length;


                if (splitIndex != length)
                {
                    Partnets = Partnets.Substring((splitIndex + 1), (Partnets.Length - 1 - splitIndex));

                }

            } while (Partnets.Contains(";"));

        }
        //Jimmy
        //Laden van Jobrequest Gross Weight in een list
        public void LoadPartGrossWeight()
        {
            string PartGross = Request.GrossWeight.Replace(" ", String.Empty);
            string GetPartGross;

            do
            {
                int splitIndex = PartGross.IndexOf(";");
                GetPartGross = PartGross.Substring(0, splitIndex);
                ListPartGross.Add(GetPartGross);
                int length = PartGross.Length;


                if (splitIndex != length)
                {
                    PartGross = PartGross.Substring((splitIndex + 1), (PartGross.Length - 1 - splitIndex));

                }

            } while (PartGross.Contains(";"));

        }
        /// <summary>
        /// jimmy
        /// </summary>
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
        /// jimmy
        /// </summary>
        private void RefreshGUI()
        {
            lstParts.Clear();
            foreach (Part part in parts)
            {
                lstParts.Add(part);
            }
        }
        


        private void FillData()
        {
            txtReqInitials = CurrentRequest.Requester;
            txtEutProjectname = CurrentRequest.EutProjectname;
            dateExpectedEnd = CurrentRequest.ExpectedEnddate;
            txtRemark = CurrentOptionel.Remarks;
            txtLinkTestplan = CurrentOptionel.Link;
            

        }

    }
}
