﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Barco.Data;
using Barco.Views;
using Prism.Commands;
namespace Barco.ModelViews
{
    public class TestPlanningViewModel : ViewModelBase
    {
        private DAO dao;
        private TestPlanning screen;
        public ICommand SaveTestCommand { get; set; }
        public ICommand CancelTestCommand { get; set; }
        //for editing inside vModel
        public ObservableCollection<PlResources> lstResources = new ObservableCollection<PlResources>();
        //for iterating and adding
        public List<PlResources> resources = new List<PlResources>();
        //for binding
        public ObservableCollection<PlResources> currentResources
        {
            get
            {
                return lstResources;
            }
        }
        private PlResources _selectedResouce { get; set; }
        public List<PlResources> Resources { get; set; }
        public ICommand AddResourceCommand { get; set; }
        public DateTime dateExpectedStart { get; set; }
        public DateTime dateExpectedEnd { get; set; }
        public string DueDate { get; set; }
        public string Omschrijving { get; set; }
        public TestPlanningViewModel(TestPlanning screen, int selectedId, string testDiv)
        {
            this.screen = screen;
            SaveTestCommand = new DelegateCommand(SaveButton);
            CancelTestCommand = new DelegateCommand(CancelButton);
            AddResourceCommand = new DelegateCommand(AddResourceButton);
            dao= DAO.Instance();
            dateExpectedStart = DateTime.Now;
            dateExpectedEnd = DateTime.Now;
            Resources = new List<PlResources>();
            populateResources(testDiv);
            _selectedResouce = new PlResources();
        }
        /// <summary>
        /// Bianca
        /// </summary>
        public void SaveButton()
        {
            MessageBox.Show("Congratulations, you have submitted a new test planning.");
            OverviewPlannedTests overviewPlannedTests = new OverviewPlannedTests();
            screen.Close();
            overviewPlannedTests.ShowDialog();
        }
        /// <summary>
        /// Bianca
        /// </summary>
        public void CancelButton()
        {
            if (MessageBox.Show("Are you sure you want to leave this screen?", "Leave", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                
                OverviewApprovedRequests overview = new OverviewApprovedRequests();
                screen.Close();
                overview.ShowDialog();
            }
        }
        /// <summary>
        /// Thibaut
        /// </summary>
        /// <param name="testDiv"></param>
        public void populateResources(string testDiv)
        {
            //Resources = dao.GetResource(); //alle resources
            Resources = dao.GetResourcesForTestDiv(testDiv);//resources per testDivision
        }
        /// <summary>
        /// Laurent
        /// </summary>
        public void AddResourceButton()
        {
            if (!String.IsNullOrEmpty(_selectedResouce.Naam) )
            {
                resources.Add(_selectedResouce);
                refresh();
            }
        }
        /// <summary>
        /// Laurent
        /// </summary>
        public void refresh()
        {
            lstResources.Clear();
            foreach (PlResources resource in resources)
            {
                lstResources.Add(resource);
            }
        }
        /// <summary>
        /// Laurent
        /// </summary>
        public PlResources SelectedResource
        {
            get
            {
                return _selectedResouce;
            }
            set
            {
                _selectedResouce = value;
                OnPropertyChanged();
            }
        }
    }
}
