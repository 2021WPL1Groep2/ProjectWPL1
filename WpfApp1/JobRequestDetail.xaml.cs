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

namespace Barco
{
    /// <summary>
    /// Interaction logic for JobRequestDetail.xaml
    /// </summary>
    public partial class JobRequestDetail : Window
    {
        public JobRequestDetail()
        {
            InitializeComponent();
        }

       //bianca
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            OverviewJobRequest overviewJobRequest = new OverviewJobRequest();
            Close();
            overviewJobRequest.Show();
        }

    }
}
