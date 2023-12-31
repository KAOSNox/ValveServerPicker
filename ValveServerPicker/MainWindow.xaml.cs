﻿using MahApps.Metro.Controls;
using Microsoft.Win32;
using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ValveServerPicker.Helpers;
using ValveServerPicker.Models;
using ValveServerPicker.Models.Web;
using ValveServerPicker.Servicesasd;
using ValveServerPicker.Tools;

namespace ValveServerPicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static int menuSelected = 0;
        //img ico TF2
        private readonly BitmapImage imgTF2Selected = new BitmapImage(new Uri("pack://application:,,,/img/tf2-select.png"));
        private readonly BitmapImage imgTF2Unselected = new BitmapImage(new Uri("pack://application:,,,/img/tf2.png"));
        //img ico CSGO
        private readonly BitmapImage imgCSGOSelected = new BitmapImage(new Uri("pack://application:,,,/img/csgo-select.png"));
        private readonly BitmapImage imgCSGOUnselected = new BitmapImage(new Uri("pack://application:,,,/img/csgo.png"));

        //Games Routes
        private string TF2Path = SteamGameFinder.FindExeAsync("Team Fortress 2", "hl2.exe").GetAwaiter().GetResult();
        private string CSGOPath = SteamGameFinder.FindExeAsync("Counter-Strike Global Offensive", "csgo.exe").GetAwaiter().GetResult();

        public RootObject serversTF2;
        public RootObject serversCSGO;

        public MainWindow()
        {
            InitializeComponent();
            LoadServers("TF2").GetAwaiter();
        }

        public async Task LoadServers(string gameContainer)
        {
            try
            {
                serversTF2 = await SteamServersServices.getTF2ServersAsync();
                serversCSGO = await SteamServersServices.getTF2ServersAsync();

                //var Rows = await Task.WhenAll(CreateRowsAsync(serversTF2, "TF2"), CreateRowsAsync(serversCSGO, "CSGO"));

                var RowsTF2 = await CreateRowsAsync(serversTF2, "TF2", TF2Path);
                var RowsCSGO = await CreateRowsAsync(serversCSGO, "CSGO", CSGOPath);

                foreach (var row in RowsTF2)
                {
                    Dispatcher.InvokeAsync(() => ServersTF2Contenedor.Children.Add(row));
                }
                foreach (var row in RowsCSGO)
                {
                    Dispatcher.InvokeAsync(() => ServersCSGOContenedor.Children.Add(row));
                }
                //
                ServerTF2ScrollView.Visibility = Visibility.Visible;
                Loading.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error LoadServers");
            }
        }

        private async Task<List<Grid>> CreateRowsAsync(RootObject servers, string gameServersName, string gamePath)
        {
            var tasks = servers.Pops.Values.Select(server => CustomRows.CreateRowAsync(gameServersName, gamePath, server.Desc, server.Relays));
            var grids = await Task.WhenAll(tasks);
            return grids.ToList();
        }


        #region Custom titleBar actions
        private void btnCerrar_MouseClick(object sender, RoutedEventArgs e)
        { Close(); }
        private void btnMinimizar_MouseClick(object sender, RoutedEventArgs e) { WindowState = WindowState.Minimized; }
        private void titleBar_MouseLeftButtonDown(object sender, MouseEventArgs e) { DragMove(); }
        #endregion

        #region Custom titleBar Hover
        //Hover effect button Min
        private void btnMinimizar_MouseEnter(object sender, MouseEventArgs e) { btnMinimizar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#b8aa02")); }
        private void btnMinimizar_MouseLeave(object sender, MouseEventArgs e) { btnMinimizar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FAE800")); }
        //Hover effect btn Closed
        private void btnCerrar_MouseEnter(object sender, MouseEventArgs e) { btnCerrar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#96262a")); }

        private void btnCerrar_MouseLeave(object sender, MouseEventArgs e) { btnCerrar.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FC4850")); }
        #endregion

        #region efectos de hover y onclick en las opciones del menu
        //Click izquierdo al icono de tf2 en el menu
        private void MenuTF2_MouseClick(object sender, RoutedEventArgs e)
        {

            if (menuSelected == 1)
            {
                menuSelected = 0;
                MenuTF2.Background = new ImageBrush { ImageSource = imgTF2Selected, TileMode = TileMode.None, Stretch = Stretch.Uniform };
                MenuCSGO.Background = new ImageBrush { ImageSource = imgCSGOUnselected, TileMode = TileMode.None, Stretch = Stretch.Uniform };
                ServerTF2ScrollView.Visibility = Visibility.Visible;
                ServerCSGOScrollView.Visibility = Visibility.Hidden;
            }
        }


        //Mouse encima del icono de tf2 en el menu
        private void MenuTF2_MouseEnter(object sender, MouseEventArgs e) { MenuTF2.Background = new ImageBrush { ImageSource = imgTF2Selected, TileMode = TileMode.None, Stretch = Stretch.Uniform }; }

        //Mouse sale del icono de tf2 en el menu
        private void MenuTF2_MouseLeave(object sender, MouseEventArgs e) { MenuTF2.Background = menuSelected == 0 ? new ImageBrush { ImageSource = imgTF2Selected, TileMode = TileMode.None, Stretch = Stretch.Uniform } : new ImageBrush { ImageSource = imgTF2Unselected, TileMode = TileMode.None, Stretch = Stretch.Uniform }; }

        //Click izquierdo al icono de csgo en el menu
        private void MenuCSGO_MouseClick(object sender, RoutedEventArgs e)
        {
            if (menuSelected == 0)
            {
                menuSelected = 1;
                MenuTF2.Background = new ImageBrush { ImageSource = imgTF2Unselected, TileMode = TileMode.None, Stretch = Stretch.Uniform };
                MenuCSGO.Background = new ImageBrush { ImageSource = imgCSGOSelected, TileMode = TileMode.None, Stretch = Stretch.Uniform };
                ServerTF2ScrollView.Visibility = Visibility.Hidden;
                ServerCSGOScrollView.Visibility = Visibility.Visible;
            }
        }


        //Mouse encima del icono de csgo en el menu
        private void MenuCSGO_MouseEnter(object sender, MouseEventArgs e)
        {

            if (menuSelected == 0)
            {
                MenuCSGO.Background = new ImageBrush { ImageSource = imgCSGOSelected, TileMode = TileMode.None, Stretch = Stretch.Uniform };
                MenuCSGO.Foreground = null;
                MenuCSGO.BorderBrush = null;
            }
            else
            {
                MenuCSGO.Background = new ImageBrush { ImageSource = imgCSGOUnselected, TileMode = TileMode.None, Stretch = Stretch.Uniform };
                MenuCSGO.Foreground = null;
                MenuCSGO.BorderBrush = null;
            }
        }

        //Mouse sale del icono de csgo en el menu
        private void MenuCSGO_MouseLeave(object sender, MouseEventArgs e)
        {
            if (menuSelected == 1)
            {
                MenuCSGO.Background = new ImageBrush { ImageSource = imgCSGOSelected, TileMode = TileMode.None, Stretch = Stretch.Uniform };
                MenuCSGO.Foreground = null;
                MenuCSGO.BorderBrush = null;
            }
            else
            {
                MenuCSGO.Background = new ImageBrush { ImageSource = imgCSGOUnselected, TileMode = TileMode.None, Stretch = Stretch.Uniform };
                MenuCSGO.Foreground = null;
                MenuCSGO.BorderBrush = null;
            }
        }

        #endregion

    }
}
